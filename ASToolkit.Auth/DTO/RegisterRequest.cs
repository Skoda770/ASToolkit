namespace ASToolkit.Auth.DTO;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);