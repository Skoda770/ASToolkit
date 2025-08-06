using ASToolkit.Communication.Core.Interfaces;

namespace ASToolkit.Communication.Services;

public class Publisher : IPublisher
{
    private readonly List<INotifier> _notifiers = [];
    private readonly List<INotifiable> _notifiables = [];
    public void AddNotifier(INotifier notifier) => _notifiers.Add(notifier);
    public void AddNotifiable(INotifiable notifiable) => _notifiables.Add(notifiable);

    public void Notify()
    {
        foreach (var notifier in _notifiers)
            notifier.Notify(_notifiables);
    }
    public void Notify(IMessage message)
    {
        foreach (var notifier in _notifiers)
            notifier.SetMessage(message);
        Notify();
    }
    public async Task NotifyAsync()
    {
        var tasks = _notifiers.Select(notifier => notifier.Notify(_notifiables));
        await Task.WhenAll(tasks);
    }

    public async Task NotifyAsync(IMessage message)
    {
        foreach (var notifier in _notifiers)
            notifier.SetMessage(message);
        await NotifyAsync();
    }
}