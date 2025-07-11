namespace ASToolkit.Communication.Email.Interfaces;

public interface IEmailSenderConfig
{
    string SmtpServer { get; set; }
    int SmtpPort { get; set; }
    string SenderEmail { get; set; }
    string? SenderName { get; set; }
    string Username { get; set; }
    string Password { get; set; }
    bool UseSsl { get; set; }
    
}