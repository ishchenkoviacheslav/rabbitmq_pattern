using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serializator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientsAPI
{
    public class ClientsAPI : IDisposable
    {
        private IConnection connection;
        private IModel channel;
        private readonly string replyQueueName;
        private EventingBasicConsumer consumer;
        private IBasicProperties props;

        public ClientsAPI()
        {
            ConnectionFactory factory = new ConnectionFactory() { UserName = "", Password = "", HostName = "" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                Object obtained = ea.Body.Deserializer();
                switch (obtained)
                {
                    case 
                        break;
                  
                    default:
                        throw new Exception("Type if different! Server sent unknown type!");
                }
            };
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
