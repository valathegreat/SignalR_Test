using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common
{
    public class AmqpMessagingService
    {
        private string _hostName = "localhost";
        private string _userName = "guest";
        private string _password = "guest";
        private string _exchangeName = "";
        private string _oneWayMessageQueueName = "OneWayMessageQueue";
        public string MessageQueueName = "OneWayMessageQueue";
        private bool _durable = true;


        public IConnection GetRabbitMqConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = _hostName;
            connectionFactory.UserName = _userName;
            connectionFactory.Password = _password;

            return connectionFactory.CreateConnection();
        }

        public void SetUpQueueForOneWayMessageDemo(IModel model)
        {
            model.QueueDeclare(_oneWayMessageQueueName, _durable, false, false, null);
        }
        public void SendOneWayMessage(string message, IModel model)
        {
            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.Persistent = (_durable);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            model.BasicPublish(_exchangeName, _oneWayMessageQueueName, basicProperties, messageBytes);
        }

        public void ReceiveOneWayMessages(IModel model)
        {
            model.BasicQos(0, 1, false); //basic quality of service
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
            model.BasicConsume(_oneWayMessageQueueName, false, consumer);
            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                String message = Encoding.UTF8.GetString(deliveryArguments.Body);
                Console.WriteLine("Message received: {0}", message);
                model.BasicAck(deliveryArguments.DeliveryTag, false);
            }
        }


        #region WorkerQueueDemoQueue
        public void SetUpQueueForWorkerQueueDemo(IModel model)
        {
            model.QueueDeclare(MessageQueueName, _durable, false, false, null);
        }

        public void SendMessageToWorkerQueue(string message, IModel model)
        {
            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.SetPersistent(_durable);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            model.BasicPublish(_exchangeName, MessageQueueName, basicProperties, messageBytes);
        }

        public void ReceiveWorkerQueueMessages(IModel model)
        {
            model.BasicQos(0, 1, false); //basic quality of service
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
            model.BasicConsume(_publishSubscribeQueueOne, false, consumer);
            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                String message = Encoding.UTF8.GetString(deliveryArguments.Body);
                Console.WriteLine("Message received: {0}", message);
                model.BasicAck(deliveryArguments.DeliveryTag, false);
            }
        }
        public void ReceiveQueueMessagesTwo(IModel model)
        {
            model.BasicQos(0, 1, false); //basic quality of service
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
            model.BasicConsume(_publishSubscribeQueueTwo, false, consumer);
            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                String message = Encoding.UTF8.GetString(deliveryArguments.Body);
                Console.WriteLine("Message received: {0}", message);
                model.BasicAck(deliveryArguments.DeliveryTag, false);
            }
        }
        #endregion

        #region Publish/Subscribe
        /*
         In this MEP a message is sent to an exchange and the exchange distributes the message to all queues bound to it.
         Each queue will have its listener to process the message.
         If you recall the different exchange types then this sounds like the Fan-out type.
         We’ll set up a dedicated exchange for this, i.e. not use the default one in RabbitMQ.
         */

        private string _publishSubscribeExchangeName = "PublishSubscribeExchange";
        private string _publishSubscribeQueueOne = "PublishSubscribeQueueOne";
        private string _publishSubscribeQueueTwo = "PublishSubscribeQueueTwo";

        private QueueingBasicConsumer _rpcConsumer;
        private string _responseQueue;

        public void SetUpExchangeAndQueuesForDemo(IModel model)
        {

            if (MessageQueueName == MEP.RemoteProcedureCall_RPC.ToString())
            {

                if (string.IsNullOrEmpty(_responseQueue))
                {
                    _responseQueue = model.QueueDeclare().QueueName;
                }

                if (_rpcConsumer == null)
                {
                    _rpcConsumer = new QueueingBasicConsumer(model);
                    model.BasicConsume(_responseQueue, true, _rpcConsumer);
                }
            }
            else
            {

                model.ExchangeDeclare(_publishSubscribeExchangeName, ExchangeType.Fanout, true);
                model.QueueDeclare(_publishSubscribeQueueOne, true, false, false, null);
                model.QueueDeclare(_publishSubscribeQueueTwo, true, false, false, null);
                model.QueueBind(_publishSubscribeQueueOne, _publishSubscribeExchangeName, "");
                model.QueueBind(_publishSubscribeQueueTwo, _publishSubscribeExchangeName, "");
            }
        }
        private string _rpcQueueName = "RpcQueue-27102017";
        public void SetUpQueueForRpcDemo(IModel model)
        {
            model.QueueDeclare(_rpcQueueName, _durable, false, false, null);
        }
        public string SendRpcMessageToQueue(string message, IModel model, TimeSpan timeout)
        {
            string correlationId = Guid.NewGuid().ToString();

            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.ReplyTo = _responseQueue;
            basicProperties.CorrelationId = correlationId;

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            model.BasicPublish("", _rpcQueueName, basicProperties, messageBytes);

            DateTime timeoutDate = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow <= timeoutDate)
            {
                BasicDeliverEventArgs deliveryArguments = (BasicDeliverEventArgs)_rpcConsumer.Queue.Dequeue();
                if (deliveryArguments.BasicProperties != null
                    && deliveryArguments.BasicProperties.CorrelationId == correlationId)
                {
                    string response = Encoding.UTF8.GetString(deliveryArguments.Body);
                    return response;
                }
            }
            throw new TimeoutException("No response before the timeout period.");

        }
        public void SendRpcMessageToQueueSendMessageToPublishSubscribeQueue(string message, IModel model)
        {
            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.SetPersistent(_durable);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            model.BasicPublish(_publishSubscribeExchangeName, "", basicProperties, messageBytes);
        }

        public void ReceiveRpcMessage(IModel model)
        {
            model.BasicQos(0, 1, false);
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
            model.BasicConsume(_rpcQueueName, false, consumer);

            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                string message = Encoding.UTF8.GetString(deliveryArguments.Body);
                Console.WriteLine("Message: {0} ; {1}", message, " Enter your response: ");
                string response = Console.ReadLine();
                IBasicProperties replyBasicProperties = model.CreateBasicProperties();
                replyBasicProperties.CorrelationId = deliveryArguments.BasicProperties.CorrelationId;
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                model.BasicPublish("", deliveryArguments.BasicProperties.ReplyTo, replyBasicProperties, responseBytes);
                model.BasicAck(deliveryArguments.DeliveryTag, false);
            }
        }
        #endregion
    }


}
