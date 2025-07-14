namespace ASToolkit.Auth.DTO;

public record TokenPair(string AccessToken, Guid RefreshToken);