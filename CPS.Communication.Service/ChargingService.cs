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
    public class ChargingService : IChargingPileService, IDisposable
    {
        //CPS_Entities EntityContext = new CPS_Entities();
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
                case PacketTypeEnum.LoginResult:
                    break;
                case PacketTypeEnum.RebootResult:
                    SessionCompleted(client, packet as OperPacketBase);
                    break;
                default:
                    break;
            }
        }

        public void LoginIn(Client client, LoginPacket args)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                try
                {
                    if (client == null || args == null) return;

                    string sn = args.SerialNumber;
                    string username = args.Username;
                    string pwd = args.Pwd;
                    var result = true;//EntityContext.CPS_ChargingPile.Any(_ => _.SerialNumber == sn && _.Username == username && _.Pwd == pwd);

                    client.SerialNumber = sn;

                    LoginResultPacket packet = new LoginResultPacket();
                    packet.SerialNumber = sn;
                    packet.ResultEnum = LoginResultEnum.Succeed;
                    packet.TimeStamp = DateTime.Now.ConvertToTimeStampX();
                    // 登录成功
                    if (result)
                    {
                        packet.ResultEnum = LoginResultEnum.Succeed;
                    }
                    else
                    {
                        packet.ResultEnum = LoginResultEnum.SecretKeyFailed;
                    }

                    client.HasLogined = packet.HasLogined;

                    if (client.IsConnected)
                    {
                        client.Send(packet);
                    }

                    string loginState = result ? "成功" : "失败";
                    Console.WriteLine($"----客户端：{client.ID} 登录{loginState}!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }));
        }

        public async Task<bool> Reboot(string serialNumber)
        {
            RebootPacket packet = new RebootPacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.Reboot,
            };

            var client = MyServer.FindClientBySerialNumber(serialNumber);
            if (client == null)
                throw new ArgumentNullException("客户端尚未连接...");

            Session session = new Session(client, packet);
            Sessions.AddSession(session);

            var result = MyServer.Send(client, packet);
            if (!result)
                return false;

            var completed = await session.WaitSessionCompleted();
            if (completed)
            {
                var data = session.Result as RebootResultPacket;
                Sessions.RemoveSession(session);
                return data.ResultBoolean;
            }
            else
                return false;
        }

        public void StartSession()
        {

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


        public void SetPriceRateForAll(List<string> sns, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
            if (sns == null || sns.Count <= 0)
                throw new ArgumentNullException("充电桩列表为空...");

            foreach (var item in sns)
            {
                try
                {
                    SetPriceRate(item, sharpRate, peakRate, flatRate, valleyRate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void SetElecPrice()
        {

        }

        public void SetServicePrice()
        {

        }

        /// <summary>
        /// 设置费率（包括电价和服务费）
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="sharpRate"></param>
        /// <param name="peakRate"></param>
        /// <param name="flatRate"></param>
        /// <param name="valleyRate"></param>
        public void SetPriceRate(string serialNumber, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
            SetElecPricePacket packet = new SetElecPricePacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.SetElecPrice,
                SharpRate = sharpRate,
                PeakRate = peakRate,
                FlatRate = flatRate,
                Valleyrate = valleyRate,
            };

            var client = MyServer.FindClientBySerialNumber(serialNumber);
            if (client == null)
                throw new ArgumentNullException("客户端尚未连接...");

            client.Send(packet);
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
