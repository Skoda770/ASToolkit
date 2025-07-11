using System.Net.Mail;

namespace ASToolkit.Communication.Email.Interfaces;

public interface IEmailSender
{
    void SetSettings(IEmailSenderConfig emailSenderConfig);
    void SendEmail(MailMessage message);
    void SendBulkEmail(IEnumerable<MailMessage> messages);
    Task SendEmailAsync(MailMessage message);
    Task SendBulkEmailAsync(IEnumerable<MailMessage> messages);
    
}