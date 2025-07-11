using ASToolkit.Communication.Sms.Enums;
using ASToolkit.Communication.Sms.Interfaces;
using ASToolkit.Communication.Sms.Twilio.Models;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace ASToolkit.Communication.Sms.Twilio.Services;

public class SmsService : ISmsService 
{
    private readonly Config _config;

    public SmsService(IOptions<Config> config)
    {
        _config = config.Value;
        TwilioClient.Init(_config.AccountSid, _config.AuthToken);
    }

    public ProviderType Type => ProviderType.Twilio;
    public async Task<bool> SendSmsAsync(string message, string phoneNumber)
    {
        var sms = await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(_config.SenderPhoneNumber),
            to: new PhoneNumber(phoneNumber));
        return sms.Status == MessageResource.StatusEnum.Queued || sms.Status == MessageResource.StatusEnum.Sent;
    }
}