using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets.充电桩_平台
{
    public class StopChargingWithCardPacket : SetChargingPacket// : PacketBase
    {
        public StopChargingWithCardPacket() : base(PacketTypeEnum.StopChargingWithCard)
        {
            //BodyLen = 1 + 11 + 1 + 8 + 1;
        }

        /*
        private byte _qport;

        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }

        /// <summary>
        /// 用户名，11位
        /// </summary>
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private byte _cardState;
        /// <summary>
        /// 卡号状态
        /// </summary>
        public byte CardState
        {
            get { return _cardState; }
            set { _cardState = value; }
        }

        private long _transactionSN;

        public long TransactionSN
        {
            get { return _transactionSN; }
            set { _transactionSN = value; }
        }

        private byte _stopReason;

        public byte StopReason
        {
            get { return _stopReason; }
            set { _stopReason = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = 0;
            body[start] = this._qport;
            start += 1;
            byte[] temp = EncodeHelper.GetBytes(this._userName);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 11;
            body[start] = this._cardState;
            start += 1;
            temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            body[start] = this._stopReason;
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._qport = buffer[start];
            start += 1;
            this._userName = EncodeHelper.GetString(buffer, start, 11);
            start += 11;
            this._cardState = buffer[start];
            start += 1;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._stopReason = buffer[start];
            return this;
        }
        */
    }
}
