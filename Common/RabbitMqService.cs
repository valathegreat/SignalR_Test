using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Common
{
    public class RabbitMqService
    {
        public IConnection GetRabbitMqConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            return connectionFactory.CreateConnection();
        }

        private static void SetupInitialTopicQueue(IModel model)
        {
            model.QueueDeclare("queueFromVisualStudio", true, false, false, null);
            model.ExchangeDeclare("exchangeFromVisualStudio", ExchangeType.Topic);
            model.QueueBind("queueFromVisualStudio", "exchangeFromVisualStudio", "superstars");
        }

        public static void SetupDurableElements(IModel model)
        {
            model.QueueDeclare("DurableQueue", true, false, false, null);
            model.ExchangeDeclare("DurableExchange", ExchangeType.Topic, true);
            model.QueueBind("DurableQueue", "DurableExchange", "durable");
        }
        public static void SendDurableMessageToDurableQueue(IModel model)
        {
            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.Persistent = (true);
            byte[] payload = Encoding.UTF8.GetBytes("This is a persistent message from Visual Studio");
            PublicationAddress address = new PublicationAddress(ExchangeType.Topic, "DurableExchange", "durable");

            model.BasicPublish(address, basicProperties, payload);
        }
    }
}
