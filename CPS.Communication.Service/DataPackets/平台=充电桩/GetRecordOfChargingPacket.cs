using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetRecordOfChargingPacket : PacketBase
    {
        public GetRecordOfChargingPacket() : base(PacketTypeEnum.GetRecordOfCharging)
        {
            BodyLen = SerialNumberLen + 8;
        }

        private long _transactionSN;

        public long TransactionSN
        {
            get { return _transactionSN; }
            set { _transactionSN = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = SerialNumberLen;
            byte[] temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = SerialNumberLen;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            return this;
        }
    }
}
