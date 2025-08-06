using ASToolkit.Communication.Core.Interfaces;
using ASToolkit.Communication.Sms.Enums;

namespace ASToolkit.Communication.Sms.Models;

public class Message : IMessage
{
    public string Text { get; set; } = null!;
    public ProviderType Provider { get; set; }
}