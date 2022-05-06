using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotification.Core.Model.Authorization
{
    public class AuthorizationTokenDto //wailiang 20200826 MDT-1591
    {
        public bool GetTokenSuccess { get; set; }
        public string AccessToken { get; set; }
    }
}
