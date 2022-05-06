using System;
using Com.GGIT.Database;
using Com.GGIT.Enumeration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using PushNotification.Core;
using PushNotification.Core.Model;

namespace Ms.PushNotification
{
    public class Program
    {
        public static void Main()
        {
            CreateHostBuilder().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                //.UseSystemd()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(hostContext.Configuration.GetSection("WorkerSettings").Get<WorkerSettings>());
                    services.AddHostedService<Worker>();
                    services.AddLogging(builder =>
                    {
                        builder.AddNLog(string.Concat(AppDomain.CurrentDomain.BaseDirectory, PathEnum.LogPath));
                    });
                    services.AddTransient<INotificationServices, NotificationServices>();
                    services.AddTransient<ISessionDB, SessionDB>();
                });
    }
}
