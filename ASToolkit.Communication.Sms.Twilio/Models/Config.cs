namespace ASToolkit.Communication.Sms.Twilio.Models;

public class Config
{
    public string AccountSid { get; set; } = null!;
    public string AuthToken { get; set; } = null!;
    public string SenderPhoneNumber { get; set; } = null!;
}