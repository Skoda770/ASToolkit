using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace ASToolkit.Auth.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    [MaxLength(200)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(200)]
    public string LastName { get; set; } = string.Empty;
}