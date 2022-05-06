using System;
using System.Threading;
using System.Threading.Tasks;
using Btc.Currency.Rate.Model;
using Com.GGIT.Common.Util;
using Com.GGIT.Database.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rate.Core;

namespace Btc.Currency.Rate
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> Log;
        private readonly WorkerSettings Settings;
        private readonly ICurrencyRateServices CurrencyRateServices;

        public Worker(ILogger<Worker> logger, WorkerSettings settings, ICurrencyRateServices currencyRateServices)
        {
            Log = logger;
            Settings = settings;
            CurrencyRateServices = currencyRateServices;

            if (!DataSettingsHelper.DatabaseIsInstalled()) throw new Exception("Failed to initialize database connection");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Execute task
                CurrencyRateServices.RetrieveRate(stoppingToken);

                // Thread sleep according appsettings
                await Task.Delay(DatetimeUtil.ConvertSecondToMilliseconds(Settings.TaskResumeAtSecond), stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.LogInformation("=====Currency Rate services started=====");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            Log.LogInformation("Services is shutting down...");
            return Task.CompletedTask;
        }
    }
}
