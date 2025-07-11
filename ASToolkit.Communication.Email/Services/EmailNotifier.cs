using System.Net.Mail;
using ASToolkit.Communication.Abstracts;
using ASToolkit.Communication.Email.Interfaces;
using ASToolkit.Communication.Email.Models;
using ASToolkit.Communication.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Communication.Email.Services;

public class EmailNotifier(ILogger<INotifier> logger, EmailSender emailSender)
    : NotifierBase<IEmailNotifiable, Message>(logger)
{
    private Message? _message;
    private readonly ILogger<INotifier> _logger = logger;

    public override async Task Notify(IEmailNotifiable notifiable)
    {
        CheckMessage();
        if (!notifiable.AllowEmailNotifications)
        {
            _logger.LogWarning("Email notifications are disabled for {NotifiableType} with ID {Email}.", notifiable.GetType().Name, notifiable.Email);
            return;
        }
        var parameters = notifiable.GetParameters();
        var mailMessage = new MailMessage
        {
            Subject = ((INotifier)this).ModifyText(_message!.Subject, parameters),
            Body = ((INotifier)this).ModifyText(_message!.Body, parameters),
            IsBodyHtml = true
        };
        mailMessage.To.Add(new MailAddress(notifiable.Email));
        foreach (var attachment in _message?.Attachments ?? Enumerable.Empty<Attachment>())
            mailMessage.Attachments.Add(attachment);

        await emailSender.SendEmailAsync(mailMessage);
    }

    private void CheckMessage()
    {
        if (_message == null)
            throw new InvalidOperationException("Message is not set. Please set the message before notifying.");
    }

    public override void SetMessage(Message message)
    {
        _message = message;
    }
}