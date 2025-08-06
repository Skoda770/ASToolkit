using System.Net.Mail;

namespace ASToolkit.Communication.Email.Interfaces;

public interface ISmtpClient : IDisposable
{
    Task SendMailAsync(MailMessage message);
    void Send(MailMessage message);
}