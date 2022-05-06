using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Com.GGIT.Database.Settings;
using Com.GGIT.LogLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Rmq.Core.Common;
using Rmq.Core.Services.MegopolyCashIn.Consumer;
using Rmq.Core.Settings;

namespace Rmq.MegopolyCashIn.Consumer
{
    public class Worker : BackgroundService  //wailiang 20200811 MDT-1582
    {
        private RabbitMQConfig rabbitConfig;
        private readonly IList<IConnection> RmqConnections = new List<IConnection>();
        private readonly string rabbitType = "Consumer";
        private Task serviceWorkerTask;

        public Worker()
        {
            Initialize();
        }

        private void Initialize()
        {
            SingletonLogger.Info("Megopoly CashOut " + rabbitType + " Log initialized.");

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

            for (int i = 1; i <= rabbitConfig.Connections; i++)
            {
                SingletonLogger.Info("Megopoly CashOut " + rabbitType + "[" + i + "] trying to connect to RabbitMQ...");

                IConnection rmqConnection = new RabbitMQManager().GetConnection(rabbitConfig);

                if (rmqConnection == null)
                {
                    SingletonLogger.Error(rabbitType + "[" + i + "] unable to connect to RabbitMQ.");
                    return Task.CompletedTask;
                }

                RmqConnections.Add(rmqConnection);
                var consumer = new RmqMegopolyCashInConsumer(rabbitType + "[" + i + "]", rmqConnection, rabbitConfig);
                CommUtil.PrintLoggerConnection("Megopoly CashOut " + rabbitType + "[" + i + "]", rmqConnection);
                serviceWorkerTask = Task.Run(() => consumer.Run(stoppingToken), stoppingToken);
            }

            return serviceWorkerTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            SingletonLogger.Info("RabbitMQ Megopoly CashOut " + rabbitType + " services stopping...");
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
