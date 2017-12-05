using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class RebootPacket : OperPacketBase
    {
        public RebootPacket() : base(PacketTypeEnum.Reboot)
        {
            BodyLen = OperPacketBodyLen;
        }

        public override byte[] EncodeBody()
        {
            return base.EncodeBody();
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            return base.DecodeBody(buffer);
        }

        public override string ToString()
        {
            return $"{nameof(RebootPacket)}: sn:{SerialNumber}, oper:{Oper}";
        }
    }
}
