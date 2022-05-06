using System;
using Com.GGIT.Database;
using Com.GGIT.Enumeration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace Rmq.PersonalReward.Consumer
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
                    services.AddHostedService<Worker>();
                    services.AddLogging(builder =>
                    {
                        //builder.ClearProviders(); show logs in console
                        builder.AddNLog(string.Concat(AppDomain.CurrentDomain.BaseDirectory, PathEnum.LogPath));
                    });
                    services.AddTransient<ISessionDB, SessionDB>();
                });
    }
}
