using ASToolkit.Communication.Interfaces;

namespace ASToolkit.Communication.Sms.Interfaces;

public interface ISmsNotifiable : INotifiable
{
    bool AllowSmsNotifications { get; set; }
    string PhoneNumber { get; set; }
    
}