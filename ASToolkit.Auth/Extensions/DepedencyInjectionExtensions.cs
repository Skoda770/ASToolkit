using ASToolkit.Auth.DAL;
using ASToolkit.Auth.Interfaces;
using ASToolkit.Auth.Models;
using ASToolkit.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASToolkit.Auth.Extensions;

public static class DepedencyInjectionExtensions
{
    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.TryAddScoped<IIdentityService, IdentityService>();
        services.TryAddSingleton<IJwtTokenService, JwtTokenService>();
        return services;
    }
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();
        RegisterServices(services);

        return services;
    }
}