using CPS.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class SetChargingPacket : OperPacketBase
    {
        public SetChargingPacket() : base(PacketTypeEnum.SetCharging)
        {
            BodyLen = OperPacketBodyLen + 8 + 1 + 1 + 4;
        }

        public SetChargingPacket(PacketTypeEnum pte) : base(pte)
        {
            BodyLen = OperPacketBodyLen + 8 + 1 + 1 + 4;
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

        public ActionTypeEnum ActionEnum
        {
            get
            {
                return (ActionTypeEnum)this._action;
            }
            set
            {
                this._action = (byte)value;
            }
        }

        private int _money=0;
        /// <summary>
        /// 充电金额（单位：分）
        /// 0：表示充满为止，非0：表示充到此金额停止充电
        /// </summary>
        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }

        /// <summary>
        /// 单位：元
        /// </summary>
        public double MoneyVal
        {
            get
            {
                return this._money * 0.01;
            }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            byte[] temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            body[start] = this._qport;
            start += 1;
            body[start] = this._action;
            start += 1;
            temp = BitConverter.GetBytes(this._money);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._qport = buffer[start];
            start += 1;
            this._action = buffer[start];
            start += 1;
            this._money = BitConverter.ToInt32(buffer, start);
            return this;
        }
    }
}
