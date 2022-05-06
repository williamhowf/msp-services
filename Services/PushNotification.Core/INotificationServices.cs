using PushNotification.Core.Model.Notification;
using System.Threading;

namespace PushNotification.Core
{
    public interface INotificationServices //wailiang 20200826 MDT-1591
    {
        void Run(CancellationToken stoppingToken);

        SettingsDto GetSettingValue();
    }
}
