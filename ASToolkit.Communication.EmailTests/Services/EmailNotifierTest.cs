using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using ASToolkit.Communication.Core.Interfaces;
using ASToolkit.Communication.Email.Interfaces;
using ASToolkit.Communication.Email.Models;
using ASToolkit.Communication.Email.Services;
using ASToolkit.Communication.EmailTests.Mocks;
using ASToolkit.Communication.EmailTests.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASToolkit.Communication.EmailTests.Services;

[TestSubject(typeof(EmailNotifier))]
public class EmailNotifierTest
{
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<ILogger<INotifier>> _loggerMock;
    private readonly EmailNotifier _emailNotifier;

    public EmailNotifierTest()
    {
        _emailSenderMock = new Mock<IEmailSender>();
        _loggerMock = new Mock<ILogger<INotifier>>();
        _emailNotifier = new EmailNotifier(_loggerMock.Object, _emailSenderMock.Object);
    }
    [Fact]
    public async Task Notify_ShouldSendEmail_WhenEmailNotificationsAreAllowed()
    {
        var notifiable = new Notifiable()
        {
            Email = "test@example.com",
            AllowEmailNotifications = true
        };

        var message = new Message
        {
            Subject = "Test Subject",
            Body = "Test Body",
            To = { "test@example.com" }
        };
        _emailNotifier.SetMessage(message);

        await _emailNotifier.Notify(notifiable);
        _emailSenderMock.Verify(es => es.SendEmailAsync(It.Is<MailMessage>(m => true)),Times.Once);
        _emailSenderMock.Verify(es => es.SendEmailAsync(It.Is<MailMessage>(m =>
            m.Subject == "Test Subject" &&
            m.Body == "Test Body")), Times.Once);
    }

    [Fact]
    public async Task Notify_ShouldLogWarning_WhenEmailNotificationsAreDisabled()
    {
        var notifiable = new Notifiable
        {
            AllowEmailNotifications = false,
            Email = "test@example.com"
        };
        var message = new Message
        {
            Subject = "Test Subject",
            Body = "Test Body",
        };
        _emailNotifier.SetMessage(message);

        await _emailNotifier.Notify(notifiable);

        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email notifications are disabled")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

        _emailSenderMock.Verify(es => es.SendEmailAsync(It.IsAny<MailMessage>()), Times.Never);
    }

    [Fact]
    public void Notify_ShouldThrowInvalidOperationException_WhenMessageIsNotSet()
    {
        var loggerMock = new Mock<ILogger<INotifier>>();
        var emailSenderMock = new Mock<IEmailSender>();
        var notifiableMock = new Mock<IEmailNotifiable>();

        var emailNotifier = new EmailNotifier(loggerMock.Object, emailSenderMock.Object);

        Assert.ThrowsAsync<InvalidOperationException>(() => emailNotifier.Notify(notifiableMock.Object));
    }
}