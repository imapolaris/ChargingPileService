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
                var db = _redis.GetDatabase();
                var sn = p.SerialNumber;
                var data = db.HashGet(ChargingPileContainer, sn);
                if (string.IsNullOrEmpty(data))
                    return;
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
            long transSN = 0;
            byte port = data.GetByteValue("port");
            int money = 0;
            if (ate == ActionTypeEnum.Startup) // 启动充电
            {
                try
                {
                    transSN = CreateTransactionSerialNumber();
                }
                catch (ArgumentException ae)
                {
                    Logger.Error(ae);
                    return false;
                }
                money = data.GetIntValue("money");
            }
            else // 停止充电
            {
                transSN = data.GetLongValue("transSn");
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
                var db = _redis.GetDatabase();
                db.HashSet(Constants.ChargingRealtimeDataContainerKey, packet.SerialNumber, p.GetUniversalData().ToJson());
            });
        }

        /// <summary>
        /// 充电桩上报交易记录
        /// </summary>
        private void RecordOfCharging(Client client, RecordOfChargingPacket packet)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                if (client == null || packet == null)
                    return;

                ConfirmRecordOfChargingPacket confirmPacket = new ConfirmRecordOfChargingPacket()
                {
                    SerialNumber = packet.SerialNumber,
                    HasCard = packet.HasCard,
                    TransactionSN = packet.TransactionSN,
                    QPort = packet.QPort,
                    CardNo = packet.CardNoVal,
                };

                client.Send(confirmPacket);
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
