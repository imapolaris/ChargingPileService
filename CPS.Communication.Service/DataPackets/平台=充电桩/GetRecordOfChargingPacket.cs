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

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            int start = SerialNumberLen;
            byte[] temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            int start = SerialNumberLen;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            return this;
        }
    }
}
