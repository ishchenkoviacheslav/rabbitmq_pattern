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
        public event Action<string> BackError;
        /// <summary>
        /// Error's message in paremeter
        /// </summary>
        public event Action<string> RegistrationError;
        /// <summary>
        /// after register you should to call login function!
        /// </summary>
        public event Action<string> RegistrationResponse;
        /// <summary>
        /// sessionId in parameter
        /// </summary>
        public event Action<string> LoginResponse;
        /// <summary>
        /// Can be login or password incorrect
        /// </summary>
        public event Action<string> LoginError;



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
                    case RegistrationResponse r:
                        RegistrationResponse("after register you should to call login function");
                        break;
                    case LoginResponse lr:
                        LoginResponse(lr.SessionId);
                        break;
                    case BackError er:
                        BackErrorHandler(er);
                        break;
                    default:
                        throw new Exception("Type if different! Server sent unknown type!");
                }
            };
            FirstPing();
        }

        private void BackErrorHandler(BackError er)
        {
            switch (er)
            {
                case RegistrationError re:
                    RegistrationError(er.ErrorDescription);
                    break;
                case LoginError le:
                    LoginError(le.ErrorDescription);
                    break;
                case BackError be:
                    BackError(be.ErrorDescription);
                    break;
                
                default:
                    throw new Exception($"Unknown error type!");
                    break;
            }
        }

        public void Dispose()
        {
            connection.Close();
        }
        //need only for test or just for first ping(in production first connect will by some another reason)
        private void FirstPing()
        {
            PingPeer ping = new PingPeer();
            channel.BasicPublish(exchange: "", routingKey: "MasterClient", basicProperties: props, body: ping.Serializer());
        }
        /// <summary>
        /// Can be exception(internal validation for Email expression)
        /// </summary>
        /// <param name="userData"></param>
        public void Register(RegistrationRequest userData)
        {
            channel.BasicPublish(exchange: "", routingKey: "MasterClient", basicProperties: props, body: userData.Serializer());
        }
        public void Login(LoginRequest logRequest)
        {
            channel.BasicPublish(exchange: "", routingKey: "MasterClient", basicProperties: props, body: logRequest.Serializer());
        }
    }
}
