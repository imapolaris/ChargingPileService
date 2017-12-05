using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CPS.Communication.Service.DataPackets;

namespace CPS.Communication.Service
{
    public class Session
    {
        public Guid SessionId { get; private set; }
        public Client MyClient { get; set; }
        public OperPacketBase MyPacket { get; set; }
        public bool IsCompleted { get; set; }
        public object Result { get; set; }


        /// <summary>
        /// 消息重发次数
        /// </summary>
        public int RetryTimes { get; set; } = 0;

        private Session()
        {
            SessionId = Guid.NewGuid();
            StartDate = DateTime.Now;
        }

        public Session(Client client, OperPacketBase packet)
            : this()
        {
            MyClient = client;
            MyPacket = packet;
        }

        public bool IsMatch(Client client, OperPacketBase packet)
        {
            if (client == null || packet == null)
                return false;

            if (!MyClient.Equals(client))
                return false;

            switch (MyPacket.Command)
            {
                case PacketTypeEnum.Login:
                    break;
                case PacketTypeEnum.LoginResult:
                    break;
                case PacketTypeEnum.Reboot:
                    if (packet.Command == PacketTypeEnum.RebootResult && packet.OperType == MyPacket.OperType)
                        return true;
                    break;
                case PacketTypeEnum.Confirm:
                    break;
                case PacketTypeEnum.Deny:
                    break;
                case PacketTypeEnum.SetElecPrice:
                case PacketTypeEnum.SetServicePrice:
                case PacketTypeEnum.SetReportInterval:
                case PacketTypeEnum.SetTimePeriod:
                case PacketTypeEnum.SetSecretKey:
                    {
                        if (packet.Command == PacketTypeEnum.Confirm || packet.Command == PacketTypeEnum.Deny)
                            if (packet.OperType == MyPacket.OperType)
                                return true;
                    }
                    break;
                case PacketTypeEnum.GetElecPrice:
                    break;
                case PacketTypeEnum.GetElecPriceResult:
                    break;
                case PacketTypeEnum.GetServicePrice:
                    break;
                case PacketTypeEnum.GetServicePriceResult:
                    break;
                case PacketTypeEnum.GetReportInterval:
                    break;
                case PacketTypeEnum.GetReportIntervalResult:
                    break;
                case PacketTypeEnum.GetTimePeriod:
                    break;
                case PacketTypeEnum.GetTimePeriodResult:
                    break;
                case PacketTypeEnum.GetSecretKey:
                    break;
                case PacketTypeEnum.GetSecretKeyResult:
                    break;
                case PacketTypeEnum.GetSoftwareVer:
                    break;
                case PacketTypeEnum.GetSoftwareVerResult:
                    break;
                case PacketTypeEnum.SetQRcode:
                    break;
                case PacketTypeEnum.GetQRcode:
                    break;
                case PacketTypeEnum.GetQRcodeResult:
                    break;
                case PacketTypeEnum.ChargingPileState:
                    break;
                case PacketTypeEnum.GetChargingPileState:
                    break;
                case PacketTypeEnum.SetCharging:
                    break;
                case PacketTypeEnum.SetChargingResult:
                    break;
                case PacketTypeEnum.RealDataOfCharging:
                    break;
                case PacketTypeEnum.RecordOfCharging:
                    break;
                case PacketTypeEnum.ConfirmRecordOfCharging:
                    break;
                case PacketTypeEnum.GetRecordOfCharging:
                    break;
                case PacketTypeEnum.FaultMessage:
                    break;
                case PacketTypeEnum.FaultMessageReply:
                    break;
                case PacketTypeEnum.WarnMessage:
                    break;
                case PacketTypeEnum.WarnMessageReply:
                    break;
                case PacketTypeEnum.StartChargingWithCard:
                    break;
                case PacketTypeEnum.StartChargingWithCardReply:
                    break;
                case PacketTypeEnum.StartChargingWithCardResult:
                    break;
                case PacketTypeEnum.StartChargingWithCardResultReply:
                    break;
                case PacketTypeEnum.RealDataOfChargingWithCard:
                    break;
                case PacketTypeEnum.StopChargingWithCard:
                    break;
                case PacketTypeEnum.StopChargingWithCardReply:
                    break;
                case PacketTypeEnum.SetBlacklist:
                    break;
                case PacketTypeEnum.SetBlacklistResult:
                    break;
                case PacketTypeEnum.SetWhitelist:
                    break;
                case PacketTypeEnum.SetWhitelistResult:
                    break;
                case PacketTypeEnum.GetBlacklist:
                    break;
                case PacketTypeEnum.GetBlacklistResult:
                    break;
                case PacketTypeEnum.GetWhitelist:
                    break;
                case PacketTypeEnum.GetWhitelistResult:
                    break;
                case PacketTypeEnum.UpgradeSoftware:
                    break;
                case PacketTypeEnum.UpgradeSfotwareReply:
                    break;
                case PacketTypeEnum.DownloadFinished:
                    break;
                case PacketTypeEnum.DownloadFinishedReply:
                    break;
                case PacketTypeEnum.InUpgradeState:
                    break;
                case PacketTypeEnum.InUpgradeStateReply:
                    break;
                case PacketTypeEnum.UpgradeResult:
                    break;
                case PacketTypeEnum.UpgradeResultReply:
                    break;
                default:
                    break;
            }
            return false;
        }

        public async Task<bool> WaitSessionCompleted()
        {
            await Task.Run(() =>
            {
                while (!Outdated && !IsCompleted)
                {
                    Task.Delay(10);
                }
            });

            if (IsCompleted)
                return true;
            else
            {
                if (RetryTimes <= 0)
                    return false;
                else
                {
                    MyClient.Send(MyPacket);
                    StartDate = DateTime.Now;
                    RetryTimes--;
                    return await WaitSessionCompleted();
                }
            }
        }

        private DateTime StartDate { get; set; }
        /// <summary>
        /// 超时时间（单位：毫秒）
        /// 默认值：5秒
        /// </summary>
        public int Timeout { get; set; } = 5 * 1000;
        public bool Outdated
        {
            get
            {
                return (DateTime.Now - StartDate).TotalMilliseconds > Timeout;
            }
        }
    }

    public class SessionCollection : IEnumerable<Session>
    {
        private List<Session> _sessions;
        public List<Session> Sessions
        {
            get { return _sessions; }
            set { _sessions = value; }
        }

        private AutoResetEvent _arEvent = new AutoResetEvent(true);

        public SessionCollection()
        {
            _sessions = new List<Session>();
        }

        public int Count
        {
            get
            {
                return this._sessions.Count;
            }
        }

        public void AddSession(Session session)
        {
            _arEvent.Reset();

            this._sessions.Add(session);

            _arEvent.Set();
        }

        public void RemoveSession(Session session)
        {
            _arEvent.Reset();

            this._sessions.Remove(session);

            _arEvent.Set();
        }

        public Session MatchSession(Client client, OperPacketBase packet)
        {
            for (int i = 0; i < _sessions.Count; i++)
            {
                var item = this._sessions[i];
                if (item.IsMatch(client, packet))
                    return item;
            }
            return null;
        }

        public void Clear()
        {
            this._sessions.Clear();
        }

        public IEnumerator<Session> GetEnumerator()
        {
            return _sessions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sessions.GetEnumerator();
        }
    }
}
