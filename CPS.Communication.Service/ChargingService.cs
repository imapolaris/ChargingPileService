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
    public partial class ChargingService : IChargingPileService, IDisposable
    {
        CPS_Entities EntityContext = new CPS_Entities();
        public SessionCollection Sessions { get; private set; }

        #region ====会话====
        Thread ThreadSessionStateDetection;
        private bool stopSessionStateDetection = false;
        private int SessionStateDetectionInterval = 60 * 1000;

        #endregion ====会话====


        private ChargingService()
        {
            ThreadPool.SetMaxThreads(200, 500);

            Sessions = new SessionCollection();
            //StartSessionStateDetection();
        }

        private static ChargingService _instance;
        static ChargingService()
        {
            if (_instance == null)
                _instance = new ChargingService();
        }
        public static ChargingService Instance { get { return _instance; } }

        public Server MyServer { get; set; }

        public void StartSessionStateDetection()
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
                default:
                    break;
            }
        }

        private void LoginIn(Client client, LoginPacket args)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                if (client == null || args == null) return;

                string sn = args.SerialNumber;
                string username = args.Username;
                string pwd = args.Pwd;

                if (string.IsNullOrEmpty(sn)
                    || string.IsNullOrEmpty(username)
                    || string.IsNullOrEmpty(pwd))
                    throw new ArgumentNullException("参数不正确");

                LoginResultPacket packet = new LoginResultPacket()
                {
                    SerialNumber = sn,
                };

                try
                {
                    // 已经登录
                    if (client.HasLogined)
                    {
                        packet.ResultEnum = LoginResultTypeEnum.HasLogined;
                    }
                    else
                    {
                        try
                        {
                            var cp = EntityContext.CPS_ChargingPile.Where(_ => _.SerialNumber == sn).First();

                            // 登录成功
                            if (cp.Username == username && cp.Pwd == pwd)
                            {
                                packet.ResultEnum = LoginResultTypeEnum.Succeed;

                                client.SerialNumber = sn;
                                client.HasLogined = packet.HasLogined;
                            }
                            else // 用户名或密码不正确
                                packet.ResultEnum = LoginResultTypeEnum.SecretKeyFailed;
                        }
                        catch (Exception ex)
                        {
                            // 设备不存在
                            packet.ResultEnum = LoginResultTypeEnum.NotExists;
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    // 其他
                    packet.ResultEnum = LoginResultTypeEnum.Others;
                    Console.WriteLine(ex.Message);
                }

                // for test.
                client.SerialNumber = sn;
                client.HasLogined = packet.HasLogined;

                var now = DateTime.Now;
                packet.TimeStamp = now.ConvertToTimeStampX();
                client.Send(packet);

                string loginState = packet.ResultString;
                Console.WriteLine($"----客户端 {client.ID} 于{now} 登录： {loginState}!");
            }));
        }

        public async Task<bool> Reboot(string serialNumber)
        {
            RebootPacket packet = new RebootPacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.RebootOper,
            };

            var client = MyServer.FindClientBySerialNumber(serialNumber);
            if (client == null)
            {
                return false;
                //throw new ArgumentNullException("客户端尚未连接...");
            }

            var result = await StartSession(client, packet);
            if (result == null)
                return false;
            else
            {
                var data = result as RebootResultPacket;
                return data.ResultBoolean;
            }
        }

        public async Task<object> StartSession(Client client, OperPacketBase packet)
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

        public void SessionCompleted(Client client, OperPacketBase packet)
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
