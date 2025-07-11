using ASToolkit.Communication.Sms.Enums;
using ASToolkit.Communication.Sms.Interfaces;

namespace ASToolkit.Communication.Sms.Infrastructure;

public class SmsProviderFactory(IEnumerable<ISmsService> smsServices)
{
    public ISmsService GetSmsService(ProviderType type)
        => smsServices.FirstOrDefault(provider => provider.Type == type)
           ?? throw new ArgumentException($"Invalid SMS provider type: {type}", nameof(type));
}