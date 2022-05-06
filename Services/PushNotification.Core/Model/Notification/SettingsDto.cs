using PushNotification.Core.Model.Authorization;

namespace PushNotification.Core.Model.Notification
{
    public class SettingsDto : AuthorizationCredentialsRequestDto //wailiang 20200826 MDT-1591
    {
        public bool PushNotificationEnable { get; set; }
        public int SleepTimer { get; set; }
        public int MaxRecord { get; set; }
        public string FinsysAPI { get; set; }
        public int ThreadNumber { get; set; }
    }
}
