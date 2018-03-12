using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPS.Infrastructure.Utils;
using CPS.Infrastructure.Enums;

namespace CPS.Communication.Service.DataPackets
{
    public class StartChargingWithCardReplyPacket : PacketBase
    {
        public StartChargingWithCardReplyPacket() : base(PacketTypeEnum.StartChargingWithCardReply)
        {
            BodyLen = 1 + CardNoLen + 8 + 1 + 4;
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

        private long _transactionSN;

        public long TransactionSN
        {
            get { return _transactionSN; }
            set { _transactionSN = value; }
        }

        private byte _result;

        public byte Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public ResultTypeEnum ResultEnum
        {
            get
            {
                return (ResultTypeEnum)this._result;
            }
        }

        public string ResultString
        {
            get
            {
                switch (ResultEnum)
                {
                    case ResultTypeEnum.Succeed:
                        return "成功";
                    case ResultTypeEnum.Failed:
                    default:
                        return "失败";
                }
            }
        }

        private int _remaining;

        public int Remaining
        {
            get { return _remaining; }
            set { _remaining = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = 0;
            body[start] = this._qport;
            start += 1;
            byte[] temp = EncodeHelper.GetBytes(this._cardNo);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += CardNoLen;
            temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            body[start] = this._result;
            start += 1;
            temp = BitConverter.GetBytes(this._remaining);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._qport = buffer[start];
            start += 1;
            this._cardNo = EncodeHelper.GetString(buffer, start, CardNoLen);
            start += CardNoLen;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._result = buffer[start];
            start += 1;
            this._remaining = BitConverter.ToInt32(buffer, start);
            return this;
        }
    }
}
