using CPS.Communication.Service.DataPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    /// <summary>
    /// 无卡充电
    /// </summary>
    public partial class ChargingService
    {
        private void ChargingPileState(Client client, PacketBase packet)
        {

        }

        public async Task<bool> SetCharging(string sn, long transSN, byte port, ActionTypeEnum ate, int money)
        {
            SetChargingPacket packet = new SetChargingPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.SetChargingOper,
                TransactionSN = transSN,
                QPort = port,
                ActionEnum = ate,
                Money = money,
            };

            var client = MyServer.FindClientBySerialNumber(sn);
            if (client == null)
            {
                return false;
                //throw new ArgumentNullException("客户端尚未连接...");
            }

            var result = await StartSession(client, packet) as SetChargingResultPacket;
            if (result == null)
                return false;

            if (result.ResultEnum == ResultTypeEnum.Succeed)
                return true;
            else
            {

                return false;
            }
        }

        /// <summary>
        /// 充电桩上报实时充电数据
        /// </summary>
        private void RealDataOfCharging(Client client, PacketBase packet)
        {

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

        public async Task<RecordOfChargingPacket> GetRecordOfCharging(string sn, long transSN)
        {
            GetRecordOfChargingPacket packet = new GetRecordOfChargingPacket()
            {
                SerialNumber = sn,
                TransactionSN = transSN,
            };

            return await GetParams<RecordOfChargingPacket>(sn, packet);
        }
    }
}
