using CPS.Communication.Service.DataPackets;
using CPS.Communication.Service.Events;
using CPS.DB;
using CPS.Infrastructure.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public partial class ChargingService : IChargingPileService, IDisposable
    {
        CPS_Entities EntityContext = new CPS_Entities();

        #region 【singleton】

        private ChargingService()
        {
            StartupServer();

            ThreadPool.SetMaxThreads(200, 500);

            Sessions = new SessionCollection();
            //StartSessionStateDetection();

            new Thread(() =>
            {
                StartMqService();
            })
            { IsBackground = true }
            .Start();
        }

        private static ChargingService _instance;
        public static ChargingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChargingService();
                }
                return _instance;
            }
        }

        #endregion 【singleton】

        #region 【启动TCP服务】

        internal Server MyServer { get; set; }

        private void StartupServer()
        {
            MyServer = new Server(this);
            MyServer.ErrorOccurred += Server_ErrorOccurred;
            MyServer.ClientAccepted += Server_ClientAccepted;
            MyServer.ServerStarted += Server_ServerStarted;
            MyServer.ServerStopped += Server_ServerStopped;
            MyServer.Listen(2222);
        }

        private static void Server_ServerStopped(object sender, Service.Events.ServerStoppedEventArgs args)
        {
            Logger.Info("Server Stopped!");
        }

        private static void Server_ServerStarted(object sender, Service.Events.ServerStartedEventArgs args)
        {
            Logger.Info("Server Started!");
        }

        private static void Server_ClientAccepted(object sender, Service.Events.ClientAcceptedEventArgs args)
        {
            Logger.Info($"----客户端 {args.CurClient.ID} 已连接！");
        }

        private static void Server_ErrorOccurred(object sender, Service.Events.ErrorEventArgs args)
        {
            Logger.Info("Error:" + args.ErrorMessage);
        }

        #endregion 【启动TCP服务】

        #region 【会话服务】

        Thread ThreadSessionStateDetection;
        private bool stopSessionStateDetection = false;
        private int SessionStateDetectionInterval = 60 * 1000;

        private SessionCollection Sessions { get; set; }

        /// <summary>
        /// 轮询会话状态
        /// </summary>
        private void StartSessionStateDetection()
        {
            ThreadSessionStateDetection = new Thread(() =>
            {
                while (true)
                {
                    if (ThreadSessionStateDetection == null
                        || !ThreadSessionStateDetection.IsAlive
                        || stopSessionStateDetection)
                        break;

                    List<Session> outdatedList = new List<Session>();
                    foreach (var item in Sessions)
                    {
                        if (item.Outdated || item.IsCompleted)
                            outdatedList.Add(item);
                    }

                    foreach (var item in outdatedList)
                    {
                        Sessions.RemoveSession(item);
                    }

                    Thread.Sleep(SessionStateDetectionInterval);
                }
            })
            { IsBackground = true };
            ThreadSessionStateDetection.Start();
        }

        /// <summary>
        /// 启动会话
        /// </summary>
        protected async Task<object> StartSession(Client client, OperPacketBase packet)
        {
            Session session = new Session(client, packet);
            Sessions.AddSession(session);

            var result = MyServer.Send(client, packet);
            if (!result)
                return null;

            var completed = await session.WaitSessionCompleted();
            if (completed)
            {
                object obj = session.Result;
                Sessions.RemoveSession(session);
                return obj;
            }
            else
                return null;
        }

        /// <summary>
        /// 会话结束
        /// </summary>
        protected void SessionCompleted(Client client, OperPacketBase packet)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                var matched = Sessions.MatchSession(client, packet);
                if (matched != null)
                {
                    matched.IsCompleted = true;
                    matched.Result = packet;
                }
            }));
        }

        #endregion 【会话】

        #region 【消息队列服务】

        private const string RPC_CHARGING_QUEUE_NAME = @"rpc_charging_queue";

        protected void StartMqService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: RPC_CHARGING_QUEUE_NAME, durable: true,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: RPC_CHARGING_QUEUE_NAME,
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        int n = int.Parse(message);
                        response = "123";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };
            }
        }

        #endregion 【消息队列服务】

        public object getChargingStatus(string sn)
        {
            var data = new JObject();
            data.Add("status", 1);
            data.Add("electric", 30);
            return data;
        }

        public bool startCharging(string sn)
        {
            return true;
        }

        public bool stopCharging(string sn)
        {
            return true;
        }

        public void ServiceFactory(Client client, PacketBase packet)
        {
            if (client == null || packet == null)
                return;

            switch (packet.Command)
            {
                case PacketTypeEnum.None:
                    break;
                case PacketTypeEnum.Login:
                    LoginIn(client, packet as LoginPacket);
                    break;
                case PacketTypeEnum.RebootResult:
                case PacketTypeEnum.Confirm:
                case PacketTypeEnum.Deny:
                case PacketTypeEnum.GetElecPriceResult:
                case PacketTypeEnum.GetServicePriceResult:
                case PacketTypeEnum.GetReportIntervalResult:
                case PacketTypeEnum.GetTimePeriodResult:
                case PacketTypeEnum.GetSecretKeyResult:
                case PacketTypeEnum.GetQRcodeResult:
                case PacketTypeEnum.SetChargingResult:
                    SessionCompleted(client, packet as OperPacketBase);
                    break;
                case PacketTypeEnum.ChargingPileState:
                    ChargingPileState(client, packet);
                    break;
                case PacketTypeEnum.RecordOfCharging:
                    SessionCompleted(client, packet as OperPacketBase);
                    RecordOfCharging(client, packet as RecordOfChargingPacket);
                    break;
                case PacketTypeEnum.StartChargingWithCard:
                    StartChargingWithCardReply(client, packet as StartChargingWithCardPacket);
                    break;
                default:
                    break;
            }
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    stopSessionStateDetection = true;
                    ThreadSessionStateDetection = null;
                }

                disposed = true;
            }
        }
    }
}
