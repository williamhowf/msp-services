using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotification.Core.Model.Authorization
{
    public class AuthorizationCredentialsRequestDto //wailiang 20200826 MDT-1591
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
    }
}
