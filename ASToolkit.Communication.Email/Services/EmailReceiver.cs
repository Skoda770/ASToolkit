using ASToolkit.Communication.Email.Interfaces;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace ASToolkit.Communication.Email.Services;

public class EmailReceiver : IEmailReceiver
{
    private ImapClient? _imapClient;
    private IEmailReceiverConfig? _emailConfig;


    public void SetSettings(IEmailReceiverConfig emailReceiverConfig)
    {
        _emailConfig = emailReceiverConfig;
        _imapClient = new ImapClient();
    }

    private void CheckImapClient()
    {
        if (_imapClient is null)
            throw new InvalidOperationException(
                "IMAP client is not initialized. Please set the configuration using SetSettings method.");
    }

    public IEnumerable<IMessageSummary> GetEmails()
    {
        ConnectAndAuthenticate();
        var folder = _imapClient!.Inbox;
        folder.Open(FolderAccess.ReadOnly);
        var messages = folder.Fetch(folder.Search(SearchQuery.All), MessageSummaryItems.All);
        _imapClient.Disconnect(true);
        return messages;
    }

    private async Task ConnectAndAuthenticateAsync()
    {
        CheckImapClient();
        await _imapClient!.ConnectAsync(_emailConfig!.ImapServer, _emailConfig.ImapPort, _emailConfig.UseSsl);
        await _imapClient.AuthenticateAsync(_emailConfig.ImapUsername, _emailConfig.ImapPassword);
    }

    private void ConnectAndAuthenticate()
    {
        CheckImapClient();
        _imapClient!.Connect(_emailConfig!.ImapServer, _emailConfig.ImapPort, _emailConfig.UseSsl);
        _imapClient.Authenticate(_emailConfig.ImapUsername, _emailConfig.ImapPassword);
    }

    public async Task<IEnumerable<IMessageSummary>> GetEmailsAsync()
    {
        await ConnectAndAuthenticateAsync();
        var folder = _imapClient!.Inbox;
        await folder.OpenAsync(FolderAccess.ReadOnly);
        var messages = await folder.FetchAsync(await folder.SearchAsync(SearchQuery.New), MessageSummaryItems.All);
        await _imapClient.DisconnectAsync(true);
        return messages;
    }

    public void DeleteEmail(UniqueId emailId)
    {
        ConnectAndAuthenticate();
        var folder = _imapClient!.Inbox;
        folder.Open(FolderAccess.ReadWrite);
        folder.SetFlags(emailId, MessageFlags.Deleted, false);
    }

    public async Task DeleteEmailAsync(UniqueId emailId)
    {
        await ConnectAndAuthenticateAsync();
        var folder = _imapClient!.Inbox;
        await folder.OpenAsync(FolderAccess.ReadWrite);
        await folder.SetFlagsAsync(emailId, MessageFlags.Deleted, false);
        await _imapClient.DisconnectAsync(true);
    }

    public void MarkAsRead(UniqueId emailId)
    {
        ConnectAndAuthenticate();
        var folder = _imapClient!.Inbox;
        folder.Open(FolderAccess.ReadWrite);
        folder.SetFlags(emailId, MessageFlags.Seen, false);
        _imapClient.Disconnect(true);
    }

    public async Task MarkAsReadAsync(UniqueId emailId)
    {
        await ConnectAndAuthenticateAsync();
        var folder = _imapClient!.Inbox;
        await folder.OpenAsync(FolderAccess.ReadWrite);
        await folder.SetFlagsAsync(emailId, MessageFlags.Seen, false);
        await _imapClient.DisconnectAsync(true);
    }

    public IMessageSummary GetEmail(UniqueId emailId)
    {
        ConnectAndAuthenticate();
        var folder = _imapClient!.Inbox;
        folder.Open(FolderAccess.ReadOnly);
        var message = folder.Fetch(new List<UniqueId>() { emailId }, MessageSummaryItems.All).First();
        _imapClient.Disconnect(true);
        return message;
    }

    public async Task<IMessageSummary> GetEmailAsync(UniqueId emailId)
    {
        await ConnectAndAuthenticateAsync();
        var folder = _imapClient!.Inbox;
        await folder.OpenAsync(FolderAccess.ReadOnly);
        var message = (await folder.FetchAsync(new List<UniqueId>() { emailId }, MessageSummaryItems.All)).First();

        await _imapClient.DisconnectAsync(true);
        return message;
    }
}