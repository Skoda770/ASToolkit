namespace ASToolkit.Communication.Email.Interfaces;

public interface IEmailReceiverConfig
{
    string ImapServer { get; set; }
    int ImapPort { get; set; }
    string ImapUsername { get; set; }
    string ImapPassword { get; set; }
    bool UseSsl { get; set; }
}