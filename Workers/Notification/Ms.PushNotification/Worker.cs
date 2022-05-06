using System;
using System.Threading;
using System.Threading.Tasks;
using Com.GGIT.Common.Util;
using Com.GGIT.Database.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PushNotification.Core;
using PushNotification.Core.Model;

namespace Ms.PushNotification
{
    public class Worker : BackgroundService //wailiang 20200826 MDT-1591
    {
        private readonly ILogger<Worker> Log;
        private readonly WorkerSettings Settings;
        private readonly INotificationServices NotificationServices;

        public Worker(ILogger<Worker> logger, WorkerSettings settings, INotificationServices notificationServices)
        {
            Log = logger;
            Settings = settings;
            NotificationServices = notificationServices;

            if (!DataSettingsHelper.DatabaseIsInstalled()) throw new Exception("Failed to initialize database connection");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    NotificationServices.Run(stoppingToken);
                }
                finally
                {
                    await Task.Delay(DatetimeUtil.ConvertSecondToMilliseconds(Settings.TaskResumeAtSecond), stoppingToken);
                }
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.LogInformation("=====Notification services started=====");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            Log.LogInformation("Services is shutting down...");
            return Task.CompletedTask;
        }
    }
}
