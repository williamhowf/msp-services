using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Rmq.Core.Settings
{
    public class RabbitMQManager
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        public IConnection GetConnection(RabbitMQConfig rmqConfig)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = rmqConfig.HostName,
                UserName = rmqConfig.UserName,
                Password = rmqConfig.Password,
                VirtualHost = rmqConfig.VirtualHost,
                Port = rmqConfig.Port
            };
            try
            {
                return connectionFactory.CreateConnection();
            }
            catch(BrokerUnreachableException ex)
            {
                Log.Error(ex);
            }
            return null;
        }
    }
}
