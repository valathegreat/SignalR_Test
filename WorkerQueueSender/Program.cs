using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Enums;
using RabbitMQ.Client;

namespace WorkerQueueSender
{
    class Program
    {
        static void Main(string[] args)
        {
            AmqpMessagingService messagingService = new AmqpMessagingService
            {
                MessageQueueName = MEP.RemoteProcedureCall_RPC.ToString()
            };
            IConnection connection = messagingService.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            //messagingService.SetUpQueueForWorkerQueueDemo(model);
            messagingService.SetUpQueueForRpcDemo(model);
            messagingService.SetUpExchangeAndQueuesForDemo(model);
            RunPublishSubscribeMessageDemo(model, messagingService);

            //RunWorkerQueueMessageDemo(model, messagingService);


        }
        private static void RunWorkerQueueMessageDemo(IModel model, AmqpMessagingService messagingService)
        {
            Console.WriteLine("Enter your message and press Enter. Quit with 'q'.");
            while (true)
            {
                string message = Console.ReadLine();
                if (message.ToLower() == "q") break;
                messagingService.SendMessageToWorkerQueue(message, model);
            }
        }

        private static void RunPublishSubscribeMessageDemo(IModel model, AmqpMessagingService messagingService)
        {
            Console.WriteLine("Enter your message and press Enter. Quit with 'q'.");
            while (true)
            {
                string message = Console.ReadLine();
                if (message.ToLower() == "q") break;

                String response = messagingService.SendRpcMessageToQueue(message, model, TimeSpan.FromMinutes(1));
                Console.WriteLine("Response: {0}", response);

                // messagingService.SendMessageToPublishSubscribeQueue(message, model);
            }
        }

    }
}
