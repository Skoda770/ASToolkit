using ASToolkit.Communication.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Communication.Core.Abstracts;

public abstract class NotifierBase<TNotifiable, TMessage> : INotifier<TNotifiable, TMessage>
    where TNotifiable : INotifiable
    where TMessage : IMessage
{
    private readonly ILogger<INotifier> _logger;
    public TMessage? Message { get; protected set; }

    protected NotifierBase(ILogger<INotifier> logger)
    {
        _logger = logger;
    }

    public void SetMessage(IMessage message)
    {
        if (message is TMessage typedMessage)
        {
            SetMessage(typedMessage);
            return;
        }

        _logger.LogWarning("NotifierBase: SetMessage called with an incompatible type {Type}. Expected {ExpectedType}.",
            message.GetType().Name, typeof(TMessage).Name);
    }

    private bool IsValidMessage()
    {
        if (Message is not null) return true;

        _logger.LogWarning("Message is not set. Please set the message before notifying.");
        return false;
    }

    private bool IsValidNotifiable(INotifiable? notifiable)
    {
        if (notifiable is TNotifiable) return true;

        _logger.LogWarning("Notify called with an incompatible type {Type}. Expected {ExpectedType}.",
            notifiable?.GetType().Name, typeof(TNotifiable).Name);
        return false;
    }

    public Task Notify(INotifiable notifiable)
    {
        if (!IsValidNotifiable(notifiable) || !IsValidMessage())
            return Task.CompletedTask;
        return Notify((TNotifiable)notifiable);
    }

    public abstract Task Notify(TNotifiable notifiable);
    public abstract void SetMessage(TMessage message);
}