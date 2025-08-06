using System.Net.Mail;
using ASToolkit.Communication.Email.Interfaces;

namespace ASToolkit.Communication.Email.Models;

public class SmtpClientWrapper(IEmailSenderConfig config) : ISmtpClient
{
    private readonly SmtpClient _smtpClient = new(config.SmtpServer, config.SmtpPort)
    {
        Credentials = new System.Net.NetworkCredential(config.Username, config.Password),
        EnableSsl = config.UseSsl
    };
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
            _smtpClient.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task SendMailAsync(MailMessage message)
        => _smtpClient.SendMailAsync(message);

    public void Send(MailMessage message)
        => _smtpClient.Send(message);
}