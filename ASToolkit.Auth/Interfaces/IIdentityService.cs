using ASToolkit.Auth.DTO;

namespace ASToolkit.Auth.Interfaces;

public interface IIdentityService
{
    TokenPair Register(RegisterRequest request);
    Task<TokenPair> RegisterAsync(RegisterRequest request);
    
    TokenPair Login(LoginRequest request);
    Task<TokenPair> LoginAsync(LoginRequest request);
    
    void Logout(Guid userId);
    Task LogoutAsync(Guid userId);
}