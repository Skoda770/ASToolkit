using ASToolkit.Communication.Abstracts;
using ASToolkit.Communication.Interfaces;
using ASToolkit.Communication.Sms.Infrastructure;
using ASToolkit.Communication.Sms.Interfaces;
using ASToolkit.Communication.Sms.Models;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Communication.Sms.Services;

public class SmsNotifier(ILogger<INotifier> logger, SmsProviderFactory smsProviderFactory)
    : NotifierBase<ISmsNotifiable, Message>(logger)
{
    private Message? _message;
    private ISmsService? _smsService;
    private readonly ILogger<INotifier> _logger = logger;

    public override async Task Notify(ISmsNotifiable notifiable)
    {
        CheckMessage();
        if (!notifiable.AllowSmsNotifications)
        {
            _logger.LogWarning("SMS notifications are disabled for {NotifiableType} with ID {PhoneNumber}.", notifiable.GetType().Name, notifiable.PhoneNumber);
            return;
        }
        var text = ((INotifier)this).ModifyText(_message!.Text, notifiable.GetParameters());
        await _smsService!.SendSmsAsync(text, notifiable.PhoneNumber);
    }

    private void CheckMessage()
    {
        if (_message == null)
            throw new InvalidOperationException("Message is not set. Please set the message before notifying.");
        if (_smsService == null)
            throw new InvalidOperationException("SMS service is not set. Please set the SMS service before notifying.");
    }

    public override void SetMessage(Message message)
    {
        _message = message;
        _smsService = smsProviderFactory.GetSmsService(message.Provider);
    }
}