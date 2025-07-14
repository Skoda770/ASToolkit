using ASToolkit.Auth.DTO;
using ASToolkit.Auth.Interfaces;
using ASToolkit.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Auth.Services;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenService jwtTokenService,
    ILogger<IdentityService> logger)
    : IIdentityService
{
    public TokenPair Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        var result = userManager.CreateAsync(user, request.Password).Result;
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => $"{error.Code}: {error.Description}").ToList();
            logger.LogError("User registration failed: {Errors}", string.Join("; ", errors));
            throw new ApplicationException(string.Join("; ", errors));
        }
        return jwtTokenService.GenerateTokens(user);
    }

    public async Task<TokenPair> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => $"{error.Code}: {error.Description}").ToList();
            logger.LogError("User registration failed: {Errors}", string.Join("; ", errors));
            throw new ApplicationException(string.Join("; ", errors));
        }
        return await jwtTokenService.GenerateTokensAsync(user);
    }

    public TokenPair Login(LoginRequest request)
    {
        var user = userManager.FindByNameAsync(request.Username).Result;
        if (user == null)
        {
            logger.LogWarning("Login attempt with invalid username: {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
        
        var result = signInManager.CheckPasswordSignInAsync(user, request.Password, true).Result;
        if (!result.Succeeded)
        {
            logger.LogWarning("Login attempt with invalid password for user: {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
        
        return jwtTokenService.GenerateTokens(user);
    }

    public async Task<TokenPair> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            logger.LogWarning("Login attempt with invalid username: {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
        
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
        {
            logger.LogWarning("Login attempt with invalid password for user: {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
    
        return await jwtTokenService.GenerateTokensAsync(user);
    }
    public void Logout(Guid userId)
    {
        jwtTokenService.ClearUserTokens(userId);
        logger.LogInformation("User logged out successfully.");
    }

    public async Task LogoutAsync(Guid userId)
    {
        await jwtTokenService.ClearUserTokensAsync(userId);
        logger.LogInformation("User logged out successfully.");
    }
}