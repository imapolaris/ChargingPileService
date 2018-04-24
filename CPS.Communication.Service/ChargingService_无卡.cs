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
    using Infrastructure.Redis;
    using Soaring.WebMonter.Contract.Cache;
    using Infrastructure.Enums;
    using Soaring.WebMonter.DB;
    using Soaring.WebMonter.Contract.History;
    using Soaring.WebMonter.Contract.Manager;

    /// <summary>
    /// 无卡充电
    /// </summary>
    public partial class ChargingService
    {
        private static readonly string ChargingPileContainer = Constants.ChargingPileContainerKey;

        private async void ChargingPileState(Client client, PacketBase packet)
        {
            var p = packet as ChargingPileStatePacket;
            if (p == null) return;

            await Task.Run(() =>
            {
                try
                {
                    var db = _redis.GetDatabase();
                    var sn = p.SerialNumber;
                    var data = db.HashGet(ChargingPileContainer, sn);
                    if (string.IsNullOrEmpty(data))
                    {
                        return;
                    }
                    else
                    {
                        var cache = JsonHelper.Deserialize<ChargingPileCache>(data);
                        if (cache == null) return;

                        cache.AA = p.APhaseA;
                        cache.BA = p.BPhaseA;
                        cache.CA = p.CPhaseA;
                        cache.AV = p.APhaseV;
                        cache.BV = p.BPhaseV;
                        cache.CV = p.CPhaseV;
                        cache.CarPortStatus = p.CarPortState;
                        cache.CurrentTime = p.TimeStamp;
                        cache.EMP = p.EMP;
                        cache.EMQ = p.EMQ;
                        cache.FaultCode = p.FaultCode;
                        cache.OutputA = p.OutputA;
                        cache.OutputV = p.OutputV;
                        cache.OutputRelayStatus = p.OutputRelayState;
                        cache.P = p.P;
                        cache.Q = p.Q;
                        cache.PortConnectStatus = p.ConnectState;
                        cache.PortWorkStatus = p.WorkingState;
                        cache.SOC = p.SOC;
                        cache.RTTemp = p.RtTemp;
                        cache.WPGWorkStatus = p.WpgWorkingState;

                        db.HashSet(ChargingPileContainer, sn, JsonHelper.Serialize(cache));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        private bool GetChargingPileState(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            byte port = data.GetByteValue("port");
            GetChargingPileStatePacket packet = new GetChargingPileStatePacket()
            {
                SerialNumber = sn,
                QPort = port,
                OperType = OperTypeEnum.GetChargingPileStateOper,
            };

            var client = MyServer.FindClientBySerialNumber(sn);
            if (client == null)
            {
                Logger.Error($"{sn}客户端尚未连接...");
                return false;
            }

            return StartSession(id, client, packet);
        }

        private bool SetCharging(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            ActionTypeEnum ate = (ActionTypeEnum)data.GetIntValue("oper");
            long transSN = data.GetLongValue("transSn");
            byte port = data.GetByteValue("port");
            int money = 0;
            if (ate == ActionTypeEnum.Startup) // 启动充电
            {
                money = data.GetIntValue("money");
            }
            SetChargingPacket packet = new SetChargingPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.SetChargingOper,
                TransactionSN = transSN,
                QPort = port,
                Action = (byte)ate,
                Money = money,
            };

            var client = MyServer.FindClientBySerialNumber(sn);
            if (client == null)
            {
                Logger.Error($"{sn}客户端尚未连接...");
                return false;
            }

            Logger.Info($"启停充电桩：{packet}");
            return StartSession(id, client, packet);
        }

        /// <summary>
        /// 充电桩上报实时充电数据
        /// </summary>
        private async void RealDataOfCharging(Client client, PacketBase packet)
        {
            var p = packet as RealDataOfChargingPacket;
            if (p == null) return;

            await Task.Run(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                var db = _redis.GetDatabase();
                db.HashSet(Constants.ChargingRealtimeDataContainerKey, packet.SerialNumber, p.GetUniversalData().ToJson());

                var hisDbContext = new HistoryDbContext();
                // 保存充电明细账到数据库
                try
                {
                    hisDbContext.ChargingDetailedRecords.Add(new ChargingDetailedRecord
                    {
                        TransactionSN = p.TransactionSN,
                        CostTime = p.CostTime,
                        CpState = p.CpState,
                        ElecMoney = p.ElecMoney,
                        FlatElec = p.FlatElec,
                        MaxTemp = p.MaxTemp,
                        MinTemp = p.MinTemp,
                        OutputA = p.OutputA,
                        OutputV = p.OutputV,
                        PeakElec = p.PeakElec,
                        QPort = p.QPort,
                        RestTime = p.SurplusTime,
                        ServiceMoney = p.ServiceMoney,
                        SharpElec = p.SharpElec,
                        SOC = p.SOC,
                        TimeStamp = p.TimeStamp,
                        TotalElec = p.TotalElec,
                        ValleyElec = p.ValleyElec,
                        Vin = p.Vin,
                    });
                    hisDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                // 检查充电金额是否超出钱包余额                
                var costMoney = p.ElecMoney + p.ServiceMoney;
                var record = hisDbContext.ChargingRecords.Where(_ => _.Transactionsn == p.TransactionSN).FirstOrDefault();
                if (record != null)
                {
                    var customerId = record.CustomerId;
                    var SysDbContext = new SystemDbContext();
                    var wallet = SysDbContext.Wallets.Where(_ => _.CustomerId == customerId).FirstOrDefault();
                    if (wallet != null)
                    {
                        // 钱包余额不足时，停止充电。
                        if (wallet.Remaining <= costMoney / 100.0)
                        {
                            SetChargingPacket stopPacket = new SetChargingPacket()
                            {
                                SerialNumber = p.SerialNumber,
                                OperType = OperTypeEnum.SetChargingOper,
                                TransactionSN = p.TransactionSN,
                                QPort = 0,
                                Action = (byte)ActionTypeEnum.Shutdown,
                                Money = 0,
                            };
                            client.Send(stopPacket);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 充电桩上报交易记录
        /// </summary>
        private void RecordOfCharging(Client client, RecordOfChargingPacket packet)
        {
            if (client == null || packet == null)
                return;

            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                try
                {
                    var hisDbContext = new HistoryDbContext();

                    // 结账计费
                    var record = hisDbContext.ChargingRecords.Where(_ => _.Transactionsn == packet.TransactionSN).FirstOrDefault();
                    if (record == null)
                    {
                        Logger.Error($"{packet.SerialNumber}，TSN：{packet.TransactionSN}充电记录不存在！");
                        return;
                    }

                    // 钱包中扣除充电费用
                    var costMoney = packet.CostMoney + packet.ServiceMoney;
                    var customerId = record.CustomerId;
                    var SysDbContext = new SystemDbContext();
                    var wallet = SysDbContext.Wallets.Where(_ => _.CustomerId == customerId).FirstOrDefault();
                    if (wallet == null)
                    {
                        Logger.Error($"客户Id：{customerId} 钱包信息不存在！");
                        return;
                    }
                    else
                    {
                        wallet.Remaining -= costMoney / 100.0; // 转换成单位：元
                    }
                    // 保存钱包信息
                    SysDbContext.SaveChanges();

                    record.CPPort = packet.QPort;
                    record.CPSerialNumber = packet.SerialNumber;
                    record.BeforeElec = packet.BeforeElec;
                    record.AfterElec = packet.AfterElec;
                    record.CostMoney = packet.CostMoney;
                    record.Duration = packet.CostTime;
                    record.Kwhs = packet.AfterElec - packet.BeforeElec;
                    record.ServiceMoney = packet.ServiceMoney;
                    record.StopReason = packet.StopReason;
                    //EndDate = packet.StopTime;
                    record.SOC = packet.SOC;
                    record.Transactionsn = packet.TransactionSN;
                    record.IsSucceed = true;

                    int result = hisDbContext.SaveChanges();

                    
                    if (result > 0) // 保存账单信息
                    {
                        // 回复账单确认信息
                        var sn = packet.SerialNumber;
                        ConfirmRecordOfChargingPacket confirmPacket = new ConfirmRecordOfChargingPacket()
                        {
                            SerialNumber = sn,
                            HasCard = packet.HasCard,
                            TransactionSN = packet.TransactionSN,
                            QPort = packet.QPort,
                            CardNo = packet.CardNoVal,
                        };
                        client.Send(confirmPacket);

                        Logger.Info($"SN：{packet.SerialNumber}， TSN：{packet.TransactionSN} 保存交易记录成功！");
                    }
                    else
                    {
                        Logger.Error($"SN：{packet.SerialNumber}， TSN：{packet.TransactionSN} 保存交易记录失败！");
                    }            
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }));
        }

        public bool GetRecordOfCharging(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            long transSN = data.GetLongValue("transSn");
            GetRecordOfChargingPacket packet = new GetRecordOfChargingPacket()
            {
                SerialNumber = sn,
                TransactionSN = transSN,
            };

            return GetParams(id, sn, packet);
        }
    }
}
