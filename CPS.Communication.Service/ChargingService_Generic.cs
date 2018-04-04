using CPS.Communication.Service.DataPackets;
using CPS.Infrastructure.Models;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.DB;
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
                // 设置为后台线程
                Thread.CurrentThread.IsBackground = true;

                if (client == null || args == null) return;

                string sn = args.SerialNumber.TrimEnd0();
                string username = args.Username.TrimEnd0();
                string pwd = args.Pwd.TrimEnd0();

                LoginResultPacket packet = new LoginResultPacket();

                // 数据不正确
                if (string.IsNullOrEmpty(sn)
                    || string.IsNullOrEmpty(username)
                    || string.IsNullOrEmpty(pwd))
                {
                    packet.ResultEnum = LoginResultTypeEnum.Others;
                    client.Send(packet);
                    Logger.Info("充电桩登录包数据不正确");
                    return;
                }

                packet.SerialNumber = sn;

                try
                {
                    // 已经登录
                    if (client.HasLogined)
                    {
                        packet.ResultEnum = LoginResultTypeEnum.HasLogined;
                    }
                    else
                    {
                        // 新的客户端，移除旧的连接
                        var c = MyServer.FindClientBySerialNumber(sn);
                        if (c != null)
                        {
                            MyServer.Clients.RemoveClient(c);
                        }

                        try
                        {
                            var SysDbContext = new SystemDbContext();
                            var data = SysDbContext.ChargingPiles.Where(_ => _.SerialNumber == sn);
                            if (data == null || data.Count() <= 0)
                            {
                                packet.ResultEnum = LoginResultTypeEnum.NotExists;
                            }
                            else
                            {
                                var cp = data.First();

                                // 登录成功
                                if (cp.UserName == username && cp.Pwd == pwd)
                                {
                                    packet.ResultEnum = LoginResultTypeEnum.Succeed;

                                    client.SerialNumber = sn;
                                    client.HasLogined = packet.HasLogined;
                                }
                                else // 用户名或密码不正确
                                {
                                    packet.ResultEnum = LoginResultTypeEnum.SecretKeyFailed;
                                }
                            }
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
                    Logger.Error(ex.Message);
                }

                // for test.
                //client.SerialNumber = sn;
                //client.HasLogined = packet.HasLogined;

                var now = DateTime.Now;
                packet.TimeStamp = now.ConvertToTimeStampX();
                client.Send(packet);

                Logger.Info($"----客户端 {client.ID} 于{now} 登录： {packet.ResultString}!");
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
                Logger.Error($"{sn}客户端尚未连接...");
                return false;
            }

            return StartSession(id, client, packet);
        }
    }
}
