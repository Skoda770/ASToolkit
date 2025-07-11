using System.Net.Mail;
using ASToolkit.Communication.Email.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;

namespace ASToolkit.Communication.Email.Services;

public class EmailSender : IEmailSender
{
    private SmtpClient? _smtpClient;
    private IEmailSenderConfig? _emailConfig;
    private readonly AsyncPolicyWrap _asyncPolicy;
    private readonly PolicyWrap _syncPolicy;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
        var asyncRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount) =>
                {
                    logger.LogWarning(
                        $"Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
                });
        _asyncPolicy = Policy.WrapAsync(asyncRetryPolicy, Policy
            .Handle<Exception>()
            .FallbackAsync(_ => throw new ApplicationException("Failed to send email after retries."),
                onFallbackAsync: (ex) =>
                {
                    logger.LogError(ex, "Failed to send email after retries.");
                    return Task.CompletedTask;
                }));
        var syncRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount) =>
                    logger.LogWarning($"Retry {retryCount} after {timeSpan.TotalSeconds}s: {exception.Message}")
            );
        _syncPolicy = Policy.Wrap(syncRetryPolicy,
            Policy.Handle<Exception>()
                .Fallback(() => throw new ApplicationException("Failed to send email after retries."),
                    onFallback: (ex) => { logger.LogError(ex, "Failed to send email after retries."); }));
    }

    public void SetSettings(IEmailSenderConfig emailSenderConfig)
    {
        _emailConfig = emailSenderConfig;
        _smtpClient = new(emailSenderConfig.SmtpServer, emailSenderConfig.SmtpPort)
        {
            Credentials = new System.Net.NetworkCredential(emailSenderConfig.Username, emailSenderConfig.Password),
            EnableSsl = emailSenderConfig.UseSsl
        };
    }

    private void CheckSmtpClient()
    {
        if (_smtpClient is null)
            throw new InvalidOperationException("SMTP client is not configured. Call SetSettings first.");
    }

    private void CheckMailMessage(MailMessage message)
    {
        if (message.From is not null) return;
        _logger.LogWarning("From address is null. Set config email as from address.");
        message.From = _emailConfig!.SenderName is not null ? new MailAddress(_emailConfig.SenderEmail, _emailConfig.SenderName) : new MailAddress(_emailConfig.SenderEmail);
    }

    public void SendEmail(MailMessage message)
    {
        CheckSmtpClient();
        CheckMailMessage(message);
        _syncPolicy.Execute(() => _smtpClient!.Send(message));
    }

    public void SendBulkEmail(IEnumerable<MailMessage> messages)
    {
        CheckSmtpClient();
        foreach (var message in messages)
        {
            CheckMailMessage(message);
            _syncPolicy.Execute(() => _smtpClient!.Send(message));
        }
    }

    public async Task SendEmailAsync(MailMessage message)
    {
        CheckSmtpClient();
        CheckMailMessage(message);
        await _asyncPolicy.ExecuteAsync(async () => await _smtpClient!.SendMailAsync(message));
    }

    public async Task SendBulkEmailAsync(IEnumerable<MailMessage> messages)
    {
        CheckSmtpClient();

        var tasks = messages.Select(message =>
        {
            CheckMailMessage(message);
            return _asyncPolicy.ExecuteAsync(async () => await _smtpClient!.SendMailAsync(message));
        });

        await Task.WhenAll(tasks);
    }
    
}