using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotification.Core.Model.Authorization
{
    public class AuthorizationTokenResponseDto //wailiang 20200826 MDT-1591
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
    }
}
