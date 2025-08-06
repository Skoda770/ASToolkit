using ASToolkit.Communication.Core.Abstracts;
using ASToolkit.Communication.Core.Interfaces;
using ASToolkit.Communication.Sms.Infrastructure;
using ASToolkit.Communication.Sms.Interfaces;
using ASToolkit.Communication.Sms.Models;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Communication.Sms.Services;

public class SmsNotifier(ILogger<INotifier> logger, SmsProviderFactory smsProviderFactory)
    : NotifierBase<ISmsNotifiable, Message>(logger)
{
    private ISmsService? _smsService;
    private readonly ILogger<INotifier> _logger = logger;

    public override async Task Notify(ISmsNotifiable notifiable)
    {
        if (
            !IsValidMessage() ||
            !IsValidSmsService() ||
            !IsValidNotifiable(notifiable)
        )
            return;

        var text = ((INotifier)this).ModifyText(Message!.Text, notifiable.GetParameters());
        await _smsService!.SendSmsAsync(text, notifiable.PhoneNumber);
    }

    private bool IsValidNotifiable(ISmsNotifiable notifiable)
    {
        if (!notifiable.AllowSmsNotifications)
        {
            _logger.LogWarning("SMS notifications are disabled for {NotifiableType} with ID {PhoneNumber}.",
                notifiable.GetType().Name, notifiable.PhoneNumber);
            return false;
        }

        if (string.IsNullOrEmpty(notifiable.PhoneNumber))
        {
            _logger.LogWarning(
                "Phone number is not set for {NotifiableType}. Please set the phone number before notifying.",
                notifiable.GetType().Name);
            return false;
        }

        return true;
    }

    private bool IsValidMessage()
    {
        if (Message is not null) return true;
        _logger.LogWarning("Message is not set. Please set the message before notifying.");
        return false;
    }

    private bool IsValidSmsService()
    {
        if (_smsService is not null) return true;
        _logger.LogWarning("SMS service is not set. Please set the SMS service before notifying.");
        return false;
    }

    public override void SetMessage(Message message)
    {
        Message = message;
        _smsService = smsProviderFactory.GetSmsService(message.Provider);
    }
}