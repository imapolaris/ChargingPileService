using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetChargingPileStatePacket : OperPacketBase
    {
        public GetChargingPileStatePacket() : base(PacketTypeEnum.GetChargingPileState)
        {
            BodyLen = OperPacketBodyLen + 1;
        }

        private byte _qport;

        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }


        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            int start = OperPacketBodyLen;
            body[start] = this._qport;
            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            int start = OperPacketBodyLen;
            this._qport = buffer[start];
            return this;
        }
    }
}
