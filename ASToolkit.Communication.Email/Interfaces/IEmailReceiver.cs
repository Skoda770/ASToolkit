using MailKit;

namespace ASToolkit.Communication.Email.Interfaces;

public interface IEmailReceiver
{
    void SetSettings(IEmailReceiverConfig emailReceiverConfig);
    IEnumerable<IMessageSummary> GetEmails();
    Task<IEnumerable<IMessageSummary>> GetEmailsAsync();
    
    void DeleteEmail(UniqueId emailId);
    Task DeleteEmailAsync(UniqueId emailId);
    void MarkAsRead(UniqueId emailId);
    Task MarkAsReadAsync(UniqueId emailId);
    IMessageSummary GetEmail(UniqueId emailId);
    Task<IMessageSummary> GetEmailAsync(UniqueId emailId);
    
}