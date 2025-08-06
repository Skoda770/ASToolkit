using System;
using System.Collections.Generic;
using ASToolkit.Communication.Core.Interfaces;
using ASToolkit.Communication.Sms.Infrastructure;
using ASToolkit.Communication.Sms.Models;
using ASToolkit.Communication.Sms.Services;
using ASToolkit.Communication.SmsTests.Mocks;
using ASToolkit.Communication.SmsTests.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Threading.Tasks;
using ASToolkit.Communication.Sms.Enums;
using ASToolkit.Communication.Sms.Interfaces;

namespace ASToolkit.Communication.SmsTests.Services;

[TestSubject(typeof(SmsNotifier))]
public class SmsNotifierTest
{
    private readonly SmsNotifier _smsNotifier;
    private readonly SmsProviderFactory _smsProviderFactory;
    private readonly Mock<ILogger<INotifier>> _loggerMock;

    public SmsNotifierTest()
    {
        _loggerMock = new Mock<ILogger<INotifier>>();
        _smsProviderFactory = new SmsProviderFactory(
            [new SmsServiceTesting(_loggerMock.Object)]);
        _smsNotifier = new SmsNotifier(_loggerMock.Object, _smsProviderFactory);
    }
        


    [Fact]
    public void SetMessage_ShouldSetCorrectMessage()
    {
        var message = new Message { Text = "Test SMS", Provider = ProviderType.TestingPurpose };

        _smsNotifier.SetMessage(message);

        Assert.Equal(message, _smsNotifier.Message);
    }

    [Fact]
    public async Task Notify_ShouldNotSend_WhenMessageIsNull()
    {
        var notifiable = new User();

        await _smsNotifier.Notify(notifiable);
    }

    [Fact]
    public async Task Notify_ShouldNotSend_WhenNotificationsAreDisabled()
    {
        var notifiable = new User { AllowSmsNotifications = false };
        var message = new Message { Text = "Test", Provider = ProviderType.TestingPurpose };
        _smsNotifier.SetMessage(message);

        await _smsNotifier.Notify(notifiable);
    }

    [Fact]
    public async Task Notify_ShouldSend_WhenAllIsValid()
    {
        var notifiable = new User { PhoneNumber = "1234567890", AllowSmsNotifications = true };
        var message = new Message { Text = "Test", Provider = ProviderType.TestingPurpose };
        _smsNotifier.SetMessage(message);

        await _smsNotifier.Notify(notifiable);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Mock SMS sent to")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}