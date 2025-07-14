using System.ComponentModel.DataAnnotations;

namespace ASToolkit.Auth;

public class JwtSettings
{
    [MinLength(32, ErrorMessage = "Secret must be at least 32 characters long.")]
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 15;
    
}