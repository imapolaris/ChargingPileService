using CPS.Communication.Service.DataPackets;
using CPS.Communication.Service.Events;
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
    using Infrastructure.Enums;
    using Infrastructure.Models;
    using Infrastructure.MQ;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Soaring.WebMonter.DB;
    using CSRedis;
    using Infrastructure.Redis;

    public partial class ChargingService : /*IChargingPileService,*/ IDisposable
    {
        SystemDbContext SysDbContext = new SystemDbContext();
        public Server MyServer { get; set; }

        #region 【singleton】

        private ChargingService()
        {
            ThreadPool.SetMaxThreads(200, 500);

            Sessions = new SessionCollection();

            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        StartMqService();

            //        Thread.Sleep(1);
            //    }
            //})
            //{ IsBackground = true }
            //.Start();

            RegisterMQService();
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

        #region 【会话服务】

        private SessionCollection Sessions { get; set; }
        private static readonly string PubChannel = ConfigHelper.Message_From_Tcp_Channel;

        /// <summary>
        /// 启动会话
        /// </summary>
        protected bool StartSession(string SessionId, Client client, OperPacketBase packet)
        {
            var result = MyServer.Send(client, packet);
            if (!result)
            {
                return false;
            }
            else
            {
                Session session = new Session(SessionId, client, packet);
                Sessions.AddSession(session);
                return true;
            }
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

                    IUniversal data = packet as IUniversal;
                    if (data == null)
                        return;
                    try
                    {
                        using (var redis = RedisManager.GetClient())
                        {
                            redis.Publish(PubChannel, data.GetUniversalData().ToJson());
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }));
        }

        #endregion 【会话】

        #region 【注册消息队列服务】

        IMqManager MqManager = null;
        private static readonly string[] Channels = new string[] { ConfigHelper.Message_From_Http_Channel };
        private void RegisterMQService()
        {
            try
            {
                MqManager = new MqManager_Redis(Channels);
                MqManager.MessageReceived += MqManager_MessageReceived;
                MqManager.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void MqManager_MessageReceived(string msg)
        {
            UniversalData data = new UniversalData();
            data.FromJson(msg);
            var id = data.GetStringValue("id");
            if (string.IsNullOrEmpty(id)) return;
            var oper = (MQMessageType)data.GetIntValue("oper");
            var result = false;
            switch (oper)
            {
                case MQMessageType.StartCharging:
                case MQMessageType.StopCharging:
                    result = SetCharging(data);
                    break;
                case MQMessageType.GetChargingPileState:
                    result = GetChargingPileState(data);
                    break;
                default:
                    break;
            }

            if (!result)
            {
                UniversalData rdata = new UniversalData();
                rdata.SetValue("id", id);
                rdata.SetValue("result", false);
                using (var redis = RedisManager.GetClient())
                {
                    redis.Publish(PubChannel, rdata.ToJson());
                }
            }
        }

        private void UnregisterMQService()
        {
            MqManager?.Stop();
        }

        #endregion

        #region 【消息队列服务（暂时废弃）】

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
                    SessionCompleted(client, packet as OperPacketBase);
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

        #region 【支持dispose】

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
                UnregisterMQService();

                disposed = true;
            }
        }

        #endregion 【支持dispose】
    }
}
