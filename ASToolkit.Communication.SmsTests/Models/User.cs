using System.Collections.Generic;
using ASToolkit.Communication.Sms.Interfaces;

namespace ASToolkit.Communication.SmsTests.Models;

public class User : ISmsNotifiable
{
    public Dictionary<string, string> GetParameters()
    {
        return new Dictionary<string, string>
        {
            { "@PhoneNumber", PhoneNumber },
            { "@Username", Username },
            { "@AllowSmsNotifications", AllowSmsNotifications.ToString() }
        };
    }

    public string Username { get; set; }
    public bool AllowSmsNotifications { get; set; }
    public string PhoneNumber { get; set; }
}