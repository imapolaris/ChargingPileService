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
            BodyLen = SerialNumberLen + 4;
        }

        public override byte[] Encode()
        {
            return base.Encode();
        }

        public override PacketBase Decode(byte[] buffer)
        {
            return base.Decode(buffer);
        }

        public override string ToString()
        {
            return $"{nameof(RebootPacket)}: sn:{SerialNumber}, oper:{Oper}";
        }
    }
}
