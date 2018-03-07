using CPS.Communication.Service.DataPackets;
using CPS.Infrastructure.Models;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    public partial class ChargingService
    {
        /// <summary>
        /// 登录
        /// </summary>
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
                            var cp = SysDbContext.ChargingPiles.Where(_ => _.SerialNumber == sn).First();

                            // 登录成功
                            if (cp.UserName == username && cp.Pwd == pwd)
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
                            Logger.Error(ex);
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

        /// <summary>
        /// 重启
        /// </summary>
        public bool Reboot(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            RebootPacket packet = new RebootPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.RebootOper,
            };

            var client = MyServer.FindClientBySerialNumber(sn);
            if (client == null)
            {
                return false;
            }

            return StartSession(id, client, packet);
        }
    }
}
