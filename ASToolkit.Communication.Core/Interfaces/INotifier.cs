namespace ASToolkit.Communication.Core.Interfaces;

public interface INotifier
{
    void SetMessage(IMessage message);
    Task Notify(INotifiable notifiable);

    Task Notify(INotifiable notifiable, IMessage message)
    {
        SetMessage(message);
        return Notify(notifiable);
    }

    Task Notify(IEnumerable<INotifiable> notifiables)
        => Task.WhenAll(notifiables.Select(Notify));
    Task Notify(IEnumerable<INotifiable> notifiables, IMessage message)
    {
        SetMessage(message);
        return Notify(notifiables);
    }
    string ModifyText(string body, Dictionary<string, string> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (string.IsNullOrEmpty(parameter.Key) || string.IsNullOrEmpty(parameter.Value))
                continue;
            
            body = System.Text.RegularExpressions.Regex.Replace(
                body,
                $@"\{{{{{System.Text.RegularExpressions.Regex.Escape(parameter.Key)}}}",
                parameter.Value);
        }

        return body;
    }
}

public interface INotifier<in TNotifiable, in TMessage> : INotifier
    where TNotifiable : INotifiable
    where TMessage : IMessage
{
    void SetMessage(TMessage message);
    Task Notify(TNotifiable notifiable);

    Task Notify(IEnumerable<TNotifiable> notifiables)
        => Task.WhenAll(notifiables.Select(Notify));

}