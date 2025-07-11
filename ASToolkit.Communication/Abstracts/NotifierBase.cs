using ASToolkit.Communication.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Communication.Abstracts;

public abstract class NotifierBase<TNotifiable, TMessage> : INotifier<TNotifiable, TMessage>
    where TNotifiable : INotifiable
    where TMessage : IMessage
{
    private readonly ILogger<INotifier> _logger;

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

    public Task Notify(INotifiable notifiable)
    {
        if (notifiable is TNotifiable typedNotifiable) return Notify(typedNotifiable);

        _logger.LogWarning("NotifierBase: Notify called with an incompatible type {Type}. Expected {ExpectedType}.",
            notifiable.GetType().Name, typeof(TNotifiable).Name);
        return Task.CompletedTask;
    }

    public abstract Task Notify(TNotifiable notifiable);
    public abstract void SetMessage(TMessage message);
}