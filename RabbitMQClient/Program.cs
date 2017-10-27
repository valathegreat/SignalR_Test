using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using RabbitMQ.Client;

namespace RabbitMQClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("RabbitMQ Starting Service... \n");
            RabbitMqService rabbitMqService = new RabbitMqService();
            IConnection connection = rabbitMqService.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            RabbitMqService.SetupDurableElements(model);
            var range = Enumerable.Range(1, 10);
            foreach (var i in range)
            {
                Thread.Sleep(5000);
                RabbitMqService.SendDurableMessageToDurableQueue(model);


                Console.Write("RabbitMQ : Published => Done \n");
                Console.Write($"RabbitMQ : {Thread.CurrentThread.Name}  \n");
            }


        }
    }
}
