using CPS.Communication.Service.DataPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    using Soaring.WebMonter.DB;
    using Soaring.WebMonter.Contract.Manager;
    using Infrastructure.Utils;
    using Soaring.WebMonter.Contract.History;
    using Infrastructure.Enums;
    using System.Threading;

    /// <summary>
    /// 有卡充电
    /// </summary>
    public partial class ChargingService
    {
        private void StartChargingWithCard(Client client, StartChargingWithCardPacket packet)
        {
            var resPacket = new StartChargingWithCardReplyPacket();

            if (packet == null)
            {
                Logger.Info("有卡启动充电报文不正确...");
                resPacket.Result = 2;
                client.Send(resPacket);
                return;
            }

            resPacket.QPort = packet.QPort;
            resPacket.UserName = packet.UserName;
            resPacket.TransactionSN = packet.TransactionSN;
            resPacket.Remaining = 0;

            var username = packet.UserName.TrimEnd0();
            var pwd = packet.Pwd.TrimEnd0();

            ThreadPool.QueueUserWorkItem((state)=>
            {
                try
                {
                    // 检查用户名和密码是否存在
                    var systemDbContext = new SystemDbContext();
                    var customer = systemDbContext.PersonalCustomers.Where(_ => _.Telephone == username && _.Password == pwd).FirstOrDefault();
                    string userId = "";
                    if (customer == null)
                    {
                        var gcustomer = systemDbContext.GroupCustomers.Where(_ => _.Telephone == username && _.Password == pwd).FirstOrDefault();
                        if (gcustomer == null)
                        {
                            resPacket.Result = 2;
                            client.Send(resPacket);
                            Logger.Info("有卡启动充电失败，用户名或密码不正确。");
                            return;
                        }
                        else
                        {
                            resPacket.Result = 1;
                            userId = gcustomer.Id;
                        }
                    }
                    else
                    {
                        resPacket.Result = 1;
                        userId = customer.Id;
                    }

                    // 查询余额
                    var wallet = systemDbContext.Wallets.Where(_ => _.CustomerId == userId).FirstOrDefault();
                    if (wallet != null)
                    {
                        resPacket.Remaining = (int)(wallet.Remaining * 100); // 单位：分
                    }

                    var resp = client.Send(resPacket);
                    if (resp)
                    {
                        Logger.Info($"用户{packet.UserName}在充电桩{packet.SerialNumber} 有卡启动充电成功！");

                        // 新增一条充电记录
                        var hisDbContext = new HistoryDbContext();
                        hisDbContext.ChargingRecords.Add(new ChargRecord
                        {
                            CompanyCode = "CP_001",
                            StationCode = "SY_004",
                            ChargingDate = DateTime.Now,
                            CPPort = packet.QPort,
                            CPSerialNumber = packet.SerialNumber,
                            CustomerId = userId,
                            Transactionsn = packet.TransactionSN,
                            IsSucceed = false,
                        });
                        hisDbContext.SaveChanges();
                    }
                    else
                    {
                        Logger.Info("有卡启动充电失败，客户端无法连接。");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        private void StartChargingWithCardResult(Client client, StartChargingWithCardResultPacket packet)
        {
            var resPacket = new StartChargingWithCardResultReplyPacket();

            var hisDbContext = new HistoryDbContext();
            var record = hisDbContext.ChargingRecords.Where(_ => _.Transactionsn == packet.TransactionSN).FirstOrDefault();
            if (record == null)
                return;

            resPacket.QPort = packet.QPort;
            resPacket.SerialNumber = packet.SerialNumber;
            resPacket.TransactionSN = packet.TransactionSN;
            resPacket.UserName = ""; // ???

            if (packet.ResultEnum == ResultTypeEnum.Failed)
            {
                record.IsSucceed = false;
                record.StopReason = packet.FailReason;
                hisDbContext.SaveChanges();
            }

            // 回复有卡启动充电结果
            client.Send(resPacket);
        }
    }
}
