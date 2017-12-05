using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class SetChargingResultPacket : OperPacketBase
    {
        public SetChargingResultPacket() : base(PacketTypeEnum.SetChargingResult)
        {
            BodyLen = OperPacketBodyLen + 8 + 1 + 1 + 1 + 1 + 1;
        }

        private long _transactionSN;
        /// <summary>
        /// 交易流水号
        /// </summary>
        public long TransactionSN
        {
            get { return _transactionSN; }
            set { _transactionSN = value; }
        }

        private byte _qport;

        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }

        private byte _action;
        /// <summary>
        /// 操作
        /// </summary>
        public byte Action
        {
            get { return _action; }
            set { _action = value; }
        }

        private byte _result;
        /// <summary>
        /// 启停结果
        /// </summary>
        public byte Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private byte _failReason;

        public byte FailReason
        {
            get { return _failReason; }
            set { _failReason = value; }
        }

        private byte _soc;

        public byte SOC
        {
            get { return _soc; }
            set { _soc = value; }
        }

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            int start = OperPacketBodyLen;
            byte[] temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            body[start] = this._qport;
            start += 1;
            body[start] = this._action;
            start += 1;
            body[start] = this._result;
            start += 1;
            body[start] = this._failReason;
            start += 1;
            body[start] = this._soc;
            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            int start = OperPacketBodyLen;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._qport = buffer[start];
            start += 1;
            this._action = buffer[start];
            start += 1;
            this._result = buffer[start];
            start += 1;
            this._failReason = buffer[start];
            start += 1;
            this._soc = buffer[start];
            return this;
        }
    }
}
