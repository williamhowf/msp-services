using System;
using System.Threading;
using System.Threading.Tasks;
using Com.GGIT.Database.Settings;
using Com.GGIT.LogLib;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Rmq.Core.Common;
using Rmq.Core.Services.PersonalReward.Producer;
using Rmq.Core.Settings;

namespace Rmq.PersonalReward.Producer
{
    public class Worker : BackgroundService   //clement 20200816 MDT-1580
    {
        private RabbitMQConfig rabbitConfig;
        private readonly string rabbitType = "Producer";
        private Task serviceWorkerTask;

        public Worker()
        {
            Initialize();
        }

        private void Initialize()
        {
            SingletonLogger.Info("Personal Reward " + rabbitType + " Log initialized.");

            if (!DataSettingsHelper.DatabaseIsInstalled())
            {
                SingletonLogger.Error("Failed to initialize database connection.");
                throw new Exception("Failed to initialize database connection.");
            }
            SingletonLogger.Info("Database connection initialized.");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            rabbitConfig = RabbitMQSettings.LoadConfig();
            SingletonLogger.Info("RabbitMQ Configuration loaded.");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SingletonLogger.Info("Initialize " + rabbitConfig.Connections + " " + rabbitType + " RabbitMQ connections.");
            SingletonLogger.Info("Personal Reward " + rabbitType + " trying to connect to RabbitMQ...");

            IConnection rmqConnection = new RabbitMQManager().GetConnection(rabbitConfig);

            if (rmqConnection == null)
            {
                SingletonLogger.Error("Publisher unable to connect to RabbitMQ.");
                return Task.CompletedTask;
            }

            var producer = new RmqPersonalRewardProducer(rmqConnection, rabbitConfig);

            serviceWorkerTask = Task.Run(() => producer.Run(stoppingToken), stoppingToken);
            CommUtil.PrintLoggerConnection("Personal Reward " + rabbitType, rmqConnection);

            return serviceWorkerTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            SingletonLogger.Info("RabbitMQ Personal Reward " + rabbitType + " services stopping...");
            MspSettings.ClearVersion();
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            SingletonLogger.Info("Disposing...");
            base.Dispose();
        }
    }
}
