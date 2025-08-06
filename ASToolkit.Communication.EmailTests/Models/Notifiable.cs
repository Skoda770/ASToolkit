using System.Collections.Generic;
using ASToolkit.Communication.Email.Interfaces;

namespace ASToolkit.Communication.EmailTests.Models;

public class Notifiable : IEmailNotifiable
{
    public Dictionary<string, string> GetParameters()
    {
        return new Dictionary<string, string>
        {
            { "@Email", Email ?? "" },
            { "@AllowEmailNotifications", AllowEmailNotifications.ToString() }
        };
    }

    public bool AllowEmailNotifications { get; set; }
    public string Email { get; set; }
}