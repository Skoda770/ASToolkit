namespace ASToolkit.Communication.Interfaces;

public interface INotifier
{
    void SetMessage(IMessage message);
    Task Notify(INotifiable notifiable);

    Task Notify(IEnumerable<INotifiable> notifiables)
        => Task.WhenAll(notifiables.Select(Notify));
    string ModifyText(string body, Dictionary<string, string> parameters)
    {
        foreach (var parameter in parameters)
        {
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