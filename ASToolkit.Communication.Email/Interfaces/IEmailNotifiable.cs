using ASToolkit.Communication.Interfaces;

namespace ASToolkit.Communication.Email.Interfaces;

public interface IEmailNotifiable : INotifiable
{
    bool AllowEmailNotifications { get; set; }
    string Email { get; set; }
}