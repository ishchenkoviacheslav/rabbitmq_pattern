using Model;
using MyLogger;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Serializator;
using System.Threading.Tasks;
using System.Threading;
using Sharing.DTO;
using System.Linq;

namespace MyPattern_MasterClient
{
    class MasterClient : IDisposable
    {
        private List<ClientPeer> ClientsList = new List<ClientPeer>();//make this collection as multithead?(use another type of collection)
        public IConnection connection;
        IModel channel;
        public CancellationTokenSource tokenSource = null;

        public MasterClient()
        {
            Logger.Info("Server started");
            ConnectionFactory factory = new ConnectionFactory() { UserName = "client", Password = "123456", HostName = "192.168.21.130" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "MasterClient", durable: false, exclusive: false, autoDelete: false, arguments: null);
            //channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "MasterClient", autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                IBasicProperties props = ea.BasicProperties;
                if (!ClientsList.Exists((c) => c.QeueuName == props.ReplyTo))
                {
                    ClientsList.Add(new ClientPeer() { QeueuName = props.ReplyTo, LastUpTime = DateTime.UtcNow });
                    Logger.Info($"Was added a client: {props.ReplyTo}");
                }
                Object obtained = ea.Body.Deserializer();
                switch (obtained)
                {
                    case PingPeer p:
                        ClientPeer peer = ClientsList.FirstOrDefault((pr) => pr.QeueuName == props.ReplyTo);
                        if (peer != null)
                        {
                            peer.LastUpTime = DateTime.UtcNow;
                        }
                        break;
                    default:
                        Logger.Error("Type is different!");
                        break;
                }//switch
            };
            tokenSource = new CancellationTokenSource();
            PingToAll();
        }//ctor
        public void Dispose()
        {
            tokenSource.Cancel();
            channel.Close();
            connection.Close();
            Logger.Info("Server stoped");
        }

        public void PingToAll()
        {
            Task.Run(() =>
            {
                Logger.Info("Ping process started...");
                while (true)
                {
                    //need do that. If server will be close, must kill that thread
                    if (tokenSource.Token.IsCancellationRequested)
                    {
                        Logger.Info("Cancellation token work!!!!!!!!!!!!!!!!!!!!!!!");
                        tokenSource.Token.ThrowIfCancellationRequested();
                    }

                    //auto delete client after 5 second
                    ClientsList.RemoveAll((c) =>
                    {

                        if (DateTime.UtcNow.Subtract(c.LastUpTime) > new TimeSpan(0, 0, 5))
                        {
                            Logger.Info("Client was deleted");
                            return true;
                        }
                        return false;
                    });
                    foreach (ClientPeer c in ClientsList)
                    {
                        channel.BasicPublish(exchange: "", routingKey: c.QeueuName, basicProperties: null, body: new PingPeer().Serializer());
                    }

                    Thread.Sleep(1000);
                }
            }, tokenSource.Token);
        }
    }
}
