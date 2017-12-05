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


        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            body[start] = this._qport;
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._qport = buffer[start];
            return this;
        }
    }
}
