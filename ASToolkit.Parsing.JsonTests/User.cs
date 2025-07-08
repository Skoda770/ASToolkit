namespace ASToolkit.Parsing.JsonTests;

public class User
{
    public string Username { get; set; } = default!;
    public string? Description { get; set; }
    public string Email { get; set; } = default!;
    public int? Birthday { get; set; }
}
