using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Enums;
using RabbitMQ.Client;

namespace WorkerQueueReceiverOne
{
    class Program
    {
        static void Main(string[] args)
        {
            AmqpMessagingService messagingService = new AmqpMessagingService
            {
                MessageQueueName = MEP.PublishSubscribe.ToString()
            };
            IConnection connection = messagingService.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            //messagingService.ReceiveWorkerQueueMessages(model);
            messagingService.ReceiveRpcMessage(model);
        }
    }
}
