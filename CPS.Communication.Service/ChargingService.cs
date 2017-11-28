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
    public class ChargingService : IChargingPileService
    {
        //CPS_Entities EntityContext = new CPS_Entities();

        private ChargingService()
        {
            ThreadPool.SetMaxThreads(200, 500);
        }

        private static ChargingService _instance;
        static ChargingService()
        {
            if (_instance == null)
                _instance = new ChargingService();
        }
        public static ChargingService Instance { get { return _instance; } }

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

        public void ServiceFactory(object sender, ReceiveCompletedEventArgs args)
        {
            // 解析数据包
            PacketBase packet = PacketAnalyzer.AnalysePacket(args.ReceivedBytes);
            Server.Client client = (Server.Client)sender;
            switch (packet.Command)
            {
                case PacketTypeEnum.None:
                    break;
                case PacketTypeEnum.Login:
                    LoginIn(client, packet as LoginPacket);
                    break;
                case PacketTypeEnum.LoginResult:
                    break;
                default:
                    break;
            }
        }

        public void LoginIn(Server.Client client, LoginPacket args)
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

                    string loginState = result ? "succeed" : "failed";
                    Console.WriteLine($"----Client {client.ID} login {loginState}!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }), null);
        }
    }
}
