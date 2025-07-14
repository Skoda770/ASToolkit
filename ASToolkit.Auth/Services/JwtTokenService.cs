using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ASToolkit.Auth.DAL;
using ASToolkit.Auth.DTO;
using ASToolkit.Auth.Interfaces;
using ASToolkit.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ASToolkit.Auth.Services;

public class JwtTokenService(
    IOptions<JwtSettings> jwtSettings,
    IdentityDbContext context,
    ILogger<JwtTokenService> logger)
    : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ConcurrentDictionary<Guid, List<TokenPair>> _userTokensCache = new();

    private string GenerateAccessToken(ApplicationUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        var claims = new List<Claim>
        {
            new Claim("UserIdentifier", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        => Task.FromResult(GenerateAccessToken(user));

    private bool IsValidToken(RefreshToken? token)
    {
        var isValid = true;
        if (token == null)
        {
            logger.LogWarning("Refresh token is null.");
            return false;
        }

        if (token.IsRevoked)
        {
            logger.LogWarning("Refresh token is revoked: {RefreshToken}", token.Token);
            isValid = false;
        }

        if (token.ExpiryDate < DateTime.UtcNow)
        {
            logger.LogWarning("Refresh token expired: {RefreshToken}", token.Token);
            isValid = false;
        }

        return isValid;
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
    {
        var oldTokens = context.RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked);
        context.RefreshTokens.RemoveRange(oldTokens);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Guid.NewGuid(),
            ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes * 2)
        };
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        return await Task.FromResult(refreshToken);
    }

    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        var oldTokens = context.RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked);
        context.RefreshTokens.RemoveRange(oldTokens);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Guid.NewGuid(),
            ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes * 2)
        };
        context.RefreshTokens.Add(refreshToken);
        context.SaveChanges();

        return refreshToken;
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Token validation failed for token: {Token}", token);
            return null;
        }
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        try
        {
            var validationResult = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            });
            if (!validationResult.IsValid)
            {
                logger.LogError(validationResult.Exception, "Token validation failed for token: {Token}", token);
                return null;
            }

            return new ClaimsPrincipal(validationResult.ClaimsIdentity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Token validation failed for token: {Token}", token);
            return null;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        var claim = principal?.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim is not null && Guid.TryParse(claim.Value, out var userId))
            return userId;
        return null;
    }

    public async Task<Guid?> GetUserIdFromTokenAsync(string token)
    {
        var principal = await ValidateTokenAsync(token);
        var claim = principal?.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim is not null && Guid.TryParse(claim.Value, out var userId))
            return userId;
        return null;
    }

    public async Task<TokenPair> GenerateTokensAsync(ApplicationUser user)
    {
        var accessToken = await GenerateAccessTokenAsync(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);
        var result = new TokenPair(accessToken, refreshToken.Token);
        HandleTokensCache(user, result);
        return result;
    }

    private void HandleTokensCache(ApplicationUser user, TokenPair tokens)
    {
        _userTokensCache[user.Id] = [tokens];
    }

    public TokenPair GenerateTokens(ApplicationUser user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user.Id);
        var result = new TokenPair(accessToken, refreshToken.Token);
        HandleTokensCache(user, result);
        return result;
    }

    public bool ValidateRefreshToken(Guid refreshToken)
    {
        var token = context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefault(e => e.Token == refreshToken);
        return IsValidToken(token);
    }

    public async Task<bool> ValidateRefreshTokenAsync(Guid refreshToken)
    {
        var token = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Token == refreshToken);
        return IsValidToken(token);
    }

    public TokenPair RefreshTokens(Guid refreshToken)
    {
        var token = context.RefreshTokens
            .AsNoTracking()
            .Include(e => e.User)
            .FirstOrDefault(e => e.Token == refreshToken);
        if (!IsValidToken(token))
            throw new UnauthorizedAccessException($"Invalid refresh token. \"{refreshToken}\"");

        return GenerateTokens(token!.User);
    }

    public async Task<TokenPair> RefreshTokensAsync(Guid refreshToken)
    {
        var token = await context.RefreshTokens
            .AsNoTracking()
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Token == refreshToken);
        if (!IsValidToken(token))
            throw new UnauthorizedAccessException($"Invalid refresh token. \"{refreshToken}\"");

        return await GenerateTokensAsync(token!.User);
    }

    public Task ClearUserTokensAsync(Guid userId)
    {
        _userTokensCache.TryRemove(userId, out _);
        return Task.CompletedTask;
    }

    public void ClearUserTokens(Guid userId)
    {
        _userTokensCache.TryRemove(userId, out _);
    }
}