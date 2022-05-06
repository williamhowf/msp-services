using System;
using Btc.Currency.Rate.Model;
using Com.GGIT.Database;
using Com.GGIT.Enumeration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Rate.Core;
using Rate.Core.Model.CoindeskPlatform;

namespace Btc.Currency.Rate
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
                    services.AddSingleton(hostContext.Configuration.GetSection("RateEndpoints").Get<RateEndpoints>());
                    services.AddHostedService<Worker>();
                    services.AddLogging(builder =>
                    {
                        //builder.ClearProviders(); show logs in console
                        builder.AddNLog(string.Concat(AppDomain.CurrentDomain.BaseDirectory, PathEnum.LogPath));
                    });
                    services.AddTransient<ICurrencyRateServices, CurrencyRateServices>();
                    services.AddTransient<ISessionDB, SessionDB>();
                    services.AddTransient<ICountryExchangeRate, CountryExchangeRate>();
                });
    }
}
