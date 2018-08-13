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
using MyPattern_MasterClient.Repositories;
using MyPattern_MasterClient.Entities;
using System.Net.Mail;
using System.IO;
using MyPattern_MasterClient.Serialization;
using Newtonsoft.Json;

namespace MyPattern_MasterClient
{
    class MasterClient : IDisposable
    {
        private List<ClientPeer> ClientsList = new List<ClientPeer>();//make this collection as multithead?(use another type of collection)
        public IConnection connection;
        IModel channel;
        public CancellationTokenSource tokenSource = null;
        public static JsonSerializeModel ConfigurationData { get; private set; } = null;

        public MasterClient()
        {
            Logger.Info("Server started");
            ReadJsonData();
            ConnectionFactory factory = new ConnectionFactory() { UserName = "client", Password = "123456", HostName = "192.168.21.130" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "MasterClient", durable: false, exclusive: false, autoDelete: false, arguments: null);
            //channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "MasterClient", autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                //to do: new thread will be here?
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
                    case RegistrationRequest r:
                        RegisterUser(r, props.ReplyTo);
                        break;
                    case LoginRequest lr:
                        Login(lr, props.ReplyTo);
                        break;
                    case LogoutRequest loutr:
                        Logout(loutr.SessionId, props.ReplyTo);
                        break;
                    case LoginBySessionRequest lbsr:
                        LoginBySession(lbsr.Session, props.ReplyTo);
                        break;
                    default:
                        Logger.Error("Type is different!");
                        break;
                }//switch
            };
            tokenSource = new CancellationTokenSource();
            PingToAll();
        }//ctor

        private void LoginBySession(string session, string replyTo)
        {
            Guid sessionGuid = Guid.Empty;
            try
            {
                sessionGuid = new Guid(session);
            }
            catch (Exception ex)
            {
                Logger.Error($"{nameof(LoginBySession)}: Session id is incorrect!");
                LoginBySessionError logBySession = new LoginBySessionError("Session is incorrect!");
                channel.BasicPublish(exchange: "", routingKey: replyTo, basicProperties: null, body: logBySession.Serializer());
                return;
            }
            User user = SessionRepository.GetUserBySession(sessionGuid);
            if(user == null)
            {
                Logger.Error($"{nameof(LoginBySession)}: Session not found!");
                LoginBySessionError logBySession = new LoginBySessionError("Session not found!");
                channel.BasicPublish(exchange: "", routingKey: replyTo, basicProperties: null, body: logBySession.Serializer());
            }
            else
            {
                Logger.Info($"User id: {user.Id}, name: {user.Email} - logged by session: {sessionGuid.ToString()}");
                LoginBySessionResponse logBySessionResp = new LoginBySessionResponse();
                channel.BasicPublish(exchange: "", routingKey: replyTo, basicProperties: null, body: logBySessionResp.Serializer());
            }
        }

        private void Logout(string sessionId, string queueName)
        {
            Guid session;
            try
            {
                session = Guid.Parse(sessionId);
                SessionRepository.DeleteUserSession(session);
            }
            catch (Exception ex)
            {
                Logger.Error("Client sent bad guid to server" + ex.Message);
            }
            LogoutResponse loutResp = new LogoutResponse();
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: loutResp.Serializer());
        }

        //set new session to db
        private void Login(LoginRequest lr, string queueName)
        {
            if (string.IsNullOrEmpty(lr.Email) || string.IsNullOrEmpty(lr.Password))
            {
                BackError error = new BackError("Email or password is empty!");
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: error.Serializer());
                return;
            }
            User user = UserRepository.GetUserByEmail(lr.Email);
            if (user == null)
            {
                LoginError regError = new LoginError("Incorrect login or password!");//incorrect login
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: regError.Serializer());
                return;
            }
            else
            {
                if (UserRepository.HashCode(lr.Password+user.Salt) == user.Password)
                {
                    Guid NewSessionId = Guid.NewGuid();
                    SessionRepository.SetUserSession(user.Id, NewSessionId);
                    LoginResponse logResponse = new LoginResponse() {SessionId = NewSessionId.ToString() };
                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: logResponse.Serializer());
                    return;
                }
                else
                {
                    LoginError regError = new LoginError("Incorrect login or password!");//incorrect password
                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: regError.Serializer());
                    return;
                }
            }
        }

        private void RegisterUser(RegistrationRequest r, string queueName)
        {
            if (string.IsNullOrEmpty(r.Email) || string.IsNullOrEmpty(r.Password))
            {
                BackError error = new BackError("Email or password is empty!");
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: error.Serializer());
                return;
            }

            if (UserRepository.IsExistEmail(r.Email))
            {
                RegistrationError regError = new RegistrationError("Email address is already used!");
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: regError.Serializer());
            }
            Guid key = Guid.NewGuid();
            User newUser = new User()
            {
                Email = r.Email,
                QueueName = queueName,
                Salt = key.ToString(),
                Password = UserRepository.HashCode(r.Password + key)
            };
            RegistrationResponse regResponse = new RegistrationResponse();
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: regResponse.Serializer());
        }

        //private Guid SendEmailToConfirm(string email)
        //{
        //    Guid guid = Guid.NewGuid();
        //    string linkText = string.Format(@"Hi, 
        //     This is to inform you that you've created a TileJong user profile with this email account on {0}. TileJong is an iOS and Android mobile gaming app that pits two players in a round-by-round word guessing puzzle challenge. If however, you don't recall ever having downloaded the TileJong app or creating the user profile in the TileJong app, please reply using this email immediately to confirm this and we will suspend the account, pending an investigation. Once our investigation is complete and we've verified that the email account has been entered either accidentally or intentionally, we will remove the email account from our records and you will not need to do anything further on your part. Thank you. 
        //     The TileJong team", DateTime.UtcNow);
        //    //string linkText = "Hello, \n Welcome to TileJong. You have confirmed your email. \n To continue the registration, you must go by this link: \n" 
        //    //                + TJongServer.ConfigurationData.EmailServerIPorDomenAndPort + guid.ToString() +
        //    //                "\n Best regards.";

        //    SmtpClient client = new SmtpClient();
        //    client.Port = 587;
        //    client.Host = "smtp.gmail.com";
        //    client.Timeout = 10000;
        //    client.EnableSsl = true;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.UseDefaultCredentials = false;
        //    client.Credentials = new System.Net.NetworkCredential(TJongServer.ConfigurationData.Email, TJongServer.ConfigurationData.EmailPassword);

        //    MailMessage objeto_mail = new MailMessage();
        //    objeto_mail.From = new MailAddress(TJongServer.ConfigurationData.Email);
        //    objeto_mail.To.Add(new MailAddress(email));
        //    objeto_mail.Subject = "Confirmation of registration";
        //    objeto_mail.Body = linkText;
        //    client.Send(objeto_mail);
        //    return guid;
        //}
        static void ReadJsonData()
        {
            string json;
            //open file stream
            if (File.Exists(Environment.CurrentDirectory + "\\configuration.json"))
            {
                using (StreamReader r = new StreamReader(Environment.CurrentDirectory + "\\configuration.json"))
                {
                    json = r.ReadToEnd();
                    ConfigurationData = JsonConvert.DeserializeObject<JsonSerializeModel>(json);
                }
            }
            else
            {
                //if no data than create a file
                ConfigurationData = new JsonSerializeModel() { DbServerDomenOrIP = @"127.0.0.1", DbName = @"localhost", DbUserName = @"root", DbUserPassword = @"MySuperPass123" };
                string jsonSerialized = JsonConvert.SerializeObject(ConfigurationData, Formatting.Indented);
                //write string to file
                File.WriteAllText(Environment.CurrentDirectory + "\\configuration.json", jsonSerialized);
            }
        }
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
