using System.Net.Mail;
using ASToolkit.Communication.Email.Interfaces;
using ASToolkit.Communication.Email.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;

namespace ASToolkit.Communication.Email.Services;

public class EmailSender : IEmailSender
{
    protected ISmtpClient? SmtpClient;
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

    public virtual void SetSettings(IEmailSenderConfig emailSenderConfig)
    {
        SmtpClient = new SmtpClientWrapper(emailSenderConfig);
    }

    private bool IsValidSmtpClient()
    {
        if (SmtpClient is not null) return true;
        _logger.LogWarning("SMTP client is not set. Please set the SMTP client before sending emails.");
        return false;
    }
    private bool IsValidMailMessage(MailMessage message)
    {
        var isValid = true;
        if (message.From is null)
        {
            _logger.LogWarning("From address is not set.");
            isValid = false;
        }
        if (message.To.Count == 0)
        {
            _logger.LogWarning("At least one To address must be set.");
            isValid = false;
        }
        return isValid;
    }
    public void SendEmail(MailMessage message)
    {
        if (!IsValidSmtpClient() || !IsValidMailMessage(message))
            return;
        _syncPolicy.Execute(() => SmtpClient!.Send(message));
        _logger.LogInformation("Email sent successfully to {ToAddresses}.", string.Join(", ", message.To.Select(t => t.Address)));
    }

    public void SendBulkEmail(IEnumerable<MailMessage> messages)
    {
        if (!IsValidSmtpClient())
            return;
        foreach (var message in messages)
        {
            if (!IsValidMailMessage(message))
                continue;
            _syncPolicy.Execute(() => SmtpClient!.Send(message));
            _logger.LogInformation("Email sent successfully to {ToAddresses}.", string.Join(", ", message.To.Select(t => t.Address)));
        }
    }

    public async Task SendEmailAsync(MailMessage message)
    {
        if (!IsValidSmtpClient() || !IsValidMailMessage(message))
            return;
        await _asyncPolicy.ExecuteAsync(async () => await SmtpClient!.SendMailAsync(message));
        _logger.LogInformation("Email sent successfully to {ToAddresses}.", string.Join(", ", message.To.Select(t => t.Address)));
    }

    public async Task SendBulkEmailAsync(IEnumerable<MailMessage> messages)
    {
        if (!IsValidSmtpClient())
            return;

        var tasks = messages.Select(message =>
        {
            return _asyncPolicy.ExecuteAsync(async () =>
            {
                if (!IsValidMailMessage(message))
                    return;
                await SmtpClient!.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {ToAddresses}.", string.Join(", ", message.To.Select(t => t.Address)));
            });
        });

        await Task.WhenAll(tasks);
    }
    
}