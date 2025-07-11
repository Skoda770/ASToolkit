using System.Net.Mail;
using ASToolkit.Communication.Email.Interfaces;
using MailKit;

namespace ASToolkit.Communication.Email.Services;

public class EmailService : IEmailReceiver, IEmailSender
{
    private readonly EmailReceiver _emailReceiver;
    private readonly EmailSender _emailSender;

    public EmailService(EmailReceiver emailReceiver, EmailSender emailSender)
    {
        _emailReceiver = emailReceiver;
        _emailSender = emailSender;
    }

    public void SetSettings(IEmailReceiverConfig emailReceiverConfig)
        => _emailReceiver.SetSettings(emailReceiverConfig);

    public IEnumerable<IMessageSummary> GetEmails()
        => _emailReceiver.GetEmails();

    public Task<IEnumerable<IMessageSummary>> GetEmailsAsync()
        => _emailReceiver.GetEmailsAsync();

    public void DeleteEmail(UniqueId emailId)
        => _emailReceiver.DeleteEmail(emailId);

    public Task DeleteEmailAsync(UniqueId emailId)
        => _emailReceiver.DeleteEmailAsync(emailId);

    public void MarkAsRead(UniqueId emailId)
        => _emailReceiver.MarkAsRead(emailId);

    public Task MarkAsReadAsync(UniqueId emailId)
        => _emailReceiver.MarkAsReadAsync(emailId);

    public IMessageSummary GetEmail(UniqueId emailId)
        => _emailReceiver.GetEmail(emailId);

    public Task<IMessageSummary> GetEmailAsync(UniqueId emailId)
        => _emailReceiver.GetEmailAsync(emailId);

    public void SetSettings(IEmailSenderConfig emailSenderConfig)
        => _emailSender.SetSettings(emailSenderConfig);

    public void SendEmail(MailMessage message)
        => _emailSender.SendEmail(message);

    public void SendBulkEmail(IEnumerable<MailMessage> messages)
        => _emailSender.SendBulkEmail(messages);

    public Task SendEmailAsync(MailMessage message)
        => _emailSender.SendEmailAsync(message);

    public Task SendBulkEmailAsync(IEnumerable<MailMessage> messages)
        => _emailSender.SendBulkEmailAsync(messages);
}