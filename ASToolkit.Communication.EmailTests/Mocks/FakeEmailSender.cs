using ASToolkit.Communication.Email.Interfaces;
using ASToolkit.Communication.Email.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ASToolkit.Communication.EmailTests.Mocks;

public class FakeEmailSender(ILogger<EmailSender> logger) : EmailSender(logger)
{
    public override void SetSettings(IEmailSenderConfig emailSenderConfig)
    {
        SmtpClient = new Mock<ISmtpClient>().Object;
    }
}