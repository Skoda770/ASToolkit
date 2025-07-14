using System.Security.Claims;
using ASToolkit.Auth.DTO;
using ASToolkit.Auth.Models;

namespace ASToolkit.Auth.Interfaces;

public interface IJwtTokenService
{
    ClaimsPrincipal? ValidateToken(string token);
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    
    Guid? GetUserIdFromToken(string token);
    Task<Guid?> GetUserIdFromTokenAsync(string token);
    
    Task<TokenPair> GenerateTokensAsync(ApplicationUser user);
    TokenPair GenerateTokens(ApplicationUser user);
    
    bool ValidateRefreshToken(Guid refreshToken);
    Task<bool> ValidateRefreshTokenAsync(Guid refreshToken);
    
    TokenPair RefreshTokens(Guid refreshToken);
    Task<TokenPair> RefreshTokensAsync(Guid refreshToken);
    
    Task ClearUserTokensAsync(Guid userId);
    void ClearUserTokens(Guid userId);
}