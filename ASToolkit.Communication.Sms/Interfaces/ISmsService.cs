using ASToolkit.Communication.Sms.Enums;

namespace ASToolkit.Communication.Sms.Interfaces;

public interface ISmsService
{
    ProviderType Type { get; }
    Task<bool> SendSmsAsync(string message, string phoneNumber);
}