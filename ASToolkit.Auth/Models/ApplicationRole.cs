using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASToolkit.Auth.Models;

public class ApplicationRole : IdentityRole<Guid>
{
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
}