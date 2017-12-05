using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class ConfirmRecordOfChargingPacket : PacketBase
    {
        public ConfirmRecordOfChargingPacket() : base(PacketTypeEnum.ConfirmRecordOfCharging)
        {
            BodyLen = SerialNumberLen + 1 + 8 + 1 + 16;
        }

        private byte _hasCard;

        public byte HasCard
        {
            get { return _hasCard; }
            set { _hasCard = value; }
        }

        private long _transactionSN;

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

        private string _cardNo;

        public string CardNo
        {
            get { return _cardNo; }
            set { _cardNo = value; }
        }

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            int start = SerialNumberLen;
            body[start] = this._hasCard;
            start += 1;
            byte[] temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            body[start] = this._qport;
            start += 1;
            temp = EncodeHelper.GetBytes(this._cardNo);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            int start = SerialNumberLen;
            this._hasCard = buffer[start];
            start += 1;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._qport = buffer[start];
            start += 1;
            this._cardNo = EncodeHelper.GetString(buffer, start, 16);
            return this;
        }
    }
}
