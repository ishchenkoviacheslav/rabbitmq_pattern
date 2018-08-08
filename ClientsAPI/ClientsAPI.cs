using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serializator;
using Sharing.DTO;
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
        private DateTime _lastUpDate = DateTime.UtcNow;
        public DateTime LastUpDate { get { return _lastUpDate; } }
        public ClientsAPI()
        {
            ConnectionFactory factory = new ConnectionFactory() { UserName = "client", Password = "123456", HostName = "192.168.21.130" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                _lastUpDate = DateTime.UtcNow;
                Object obtained = ea.Body.Deserializer();
                switch (obtained)
                {
                    case PingPeer p:
                        PingPeer ping = new PingPeer();
                        channel.BasicPublish(exchange: "", routingKey: "MasterClient", basicProperties: props, body: ping.Serializer());
                        break;
                  
                    default:
                        throw new Exception("Type if different! Server sent unknown type!");
                }
            };
            FirstPing();
        }
        
        public void Dispose()
        {
            connection.Close();
        }
        //need only for test or just for first ping(in production first connect will by some another reason)
        public void FirstPing()
        {
            PingPeer ping = new PingPeer();
            channel.BasicPublish(exchange: "", routingKey: "MasterClient", basicProperties: props, body: ping.Serializer());
        }

    }
}
