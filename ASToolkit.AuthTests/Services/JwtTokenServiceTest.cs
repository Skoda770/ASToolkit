using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using ASToolkit.Auth;
using ASToolkit.Auth.DAL;
using ASToolkit.Auth.Models;
using ASToolkit.Auth.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ASToolkit.AuthTests.Services;

[TestSubject(typeof(JwtTokenService))]
public class JwtTokenServiceTest
{
    private readonly JwtTokenService _service;
    private readonly IdentityDbContext _dbContext;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenServiceTest()
    {
        _jwtSettings = new JwtSettings
        {
            Secret = "testsecret1234567890testsecret1234567890testsecret1234567890",
            Issuer = "test",
            Audience = "test",
            ExpiryMinutes = 60
        };
        _dbContext = TestDbContextFactory.CreateDbInMemory();
        _dbContext.SeedUsers();
        _service = new JwtTokenService(Options.Create(_jwtSettings), _dbContext,
            new Mock<ILogger<JwtTokenService>>().Object);
    }

    [Fact]
    public async Task GenerateTokensAsync_ValidUser_ReturnsNonEmptyAccessToken()
    {
        var user = _dbContext.Users.First();
        var tokens = await _service.GenerateTokensAsync(user);
        Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_ValidUser_SavesTokenInDb()
    {
        var user = _dbContext.Users.First();
        await _service.GenerateTokensAsync(user);
        var tokensInDb = await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id).CountAsync();
        Assert.Equal(1, tokensInDb);
    }

    [Fact]
    public async Task ValidateTokenAsync_ValidToken_ReturnsClaimsPrincipal()
    {
        var user = _dbContext.Users.First();
        var tokens = await _service.GenerateTokensAsync(user);
        var principal = await _service.ValidateTokenAsync(tokens.AccessToken);
        Assert.NotNull(principal);
        Assert.True(principal.HasClaim("UserIdentifier", user.Id.ToString()));
    }

    [Fact]
    public async Task ValidateTokenAsync_InvalidToken_ReturnsNull()
    {
        var result = await _service.ValidateTokenAsync("invalid.token.value");
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateTokenAsync_ExpiredToken_ReturnsNull()
    {
        var user = _dbContext.Users.First();
        _jwtSettings.ExpiryMinutes = -1; // Set expiry to a negative value to simulate an expired token
        var expiredService = new JwtTokenService(Options.Create(_jwtSettings), _dbContext,
            new Mock<ILogger<JwtTokenService>>().Object);
        var tokens = await expiredService.GenerateTokensAsync(user);
        var principal = await expiredService.ValidateTokenAsync(tokens.AccessToken);
        Assert.Null(principal);
    }

    [Fact]
    public async Task RefreshTokensAsync_ValidRefreshToken_GeneratesNewTokens()
    {
        var user = _dbContext.Users.First();
        var tokens = await _service.GenerateTokensAsync(user);
        var result = await _service.RefreshTokensAsync(tokens.RefreshToken);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.NotEqual(result.RefreshToken, Guid.Empty);
    }

    [Fact]
    public async Task RefreshTokensAsync_InvalidRefreshToken_ThrowsUnauthorizedAccessException()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.RefreshTokensAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_RemovesOldExpiredTokens()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "test", Email = "test@test.com" };
        var oldToken = new RefreshToken
            { UserId = user.Id, User = user, Token = Guid.NewGuid(), ExpiryDate = DateTime.UtcNow.AddMinutes(-10) };
        await _dbContext.RefreshTokens.AddAsync(oldToken);
        await _dbContext.SaveChangesAsync();

        await _service.GenerateTokensAsync(user);

        var tokens = await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id && rt.Token == oldToken.Token && !rt.IsRevoked)
            .ToListAsync();
        Assert.Empty(tokens);
    }

    [Fact]
    public async Task GenerateTokensAsync_InvalidJwtSettings_ThrowsException()
    {
        var invalidSettings = new JwtSettings
            { Secret = "short", Issuer = "test", Audience = "test", ExpiryMinutes = 60 };
        var service = new JwtTokenService(Options.Create(invalidSettings), _dbContext,
            new Mock<ILogger<JwtTokenService>>().Object);
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "test", Email = "test@test.com" };
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GenerateTokensAsync(user));
    }
}