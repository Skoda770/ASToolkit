using ASToolkit.Communication.Core.Interfaces;

namespace ASToolkit.Communication.Services;

public interface IPublisher
{
    void AddNotifier(INotifier notifier);
    void AddNotifiable(INotifiable notifiable);
    
    void Notify();
    void Notify(IMessage message);
    Task NotifyAsync();
    Task NotifyAsync(IMessage message);
}