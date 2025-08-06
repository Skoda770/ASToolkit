using System.Threading.Tasks;
using ASToolkit.Communication.Sms.Enums;
using ASToolkit.Communication.Sms.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Communication.SmsTests.Mocks;

public class SmsServiceTesting : ISmsService
{
    private readonly ILogger _logger;

    public SmsServiceTesting(ILogger logger)
    {
        _logger = logger;
    }

    public ProviderType Type => ProviderType.TestingPurpose;

    public async Task<bool> SendSmsAsync(string message, string phoneNumber)
    {
        _logger.LogInformation("Mock SMS sent to {PhoneNumber}: {Message}", phoneNumber, message);
        
        await Task.Delay(1000); 
        return true;
    }
}