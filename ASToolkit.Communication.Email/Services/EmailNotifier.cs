using System.Net.Mail;
using ASToolkit.Communication.Core.Abstracts;
using ASToolkit.Communication.Email.Interfaces;
using ASToolkit.Communication.Email.Models;
using ASToolkit.Communication.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Communication.Email.Services;

public class EmailNotifier(ILogger<INotifier> logger, IEmailSender emailSender)
    : NotifierBase<IEmailNotifiable, Message>(logger)
{
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
            Subject = ((INotifier)this).ModifyText(Message!.Subject, parameters),
            Body = ((INotifier)this).ModifyText(Message!.Body, parameters),
            IsBodyHtml = true
        };
        mailMessage.To.Add(new MailAddress(notifiable.Email));
        foreach (var attachment in Message?.Attachments ?? Enumerable.Empty<Attachment>())
            mailMessage.Attachments.Add(attachment);

        await emailSender.SendEmailAsync(mailMessage);
    }

    private void CheckMessage()
    {
        if (Message == null)
            throw new InvalidOperationException("Message is not set. Please set the message before notifying.");
    }

    public override void SetMessage(Message message)
    {
        Message = message;
    }
}