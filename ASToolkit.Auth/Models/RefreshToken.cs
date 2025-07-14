namespace ASToolkit.Auth.Models;

public class RefreshToken
{
    public Guid Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public bool IsRevoked { get; set; }
}