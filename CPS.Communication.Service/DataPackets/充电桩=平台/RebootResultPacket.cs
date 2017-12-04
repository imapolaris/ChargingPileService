using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class RebootResultPacket : OperResultPacketBase
    {
        public RebootResultPacket() : base(PacketTypeEnum.RebootResult)
        {
            BodyLen = SerialNumberLen + 4 + 1;
        }

        public override string ToString()
        {
            return $"{nameof(RebootResultPacket)}: sn:{SerialNumber}, oper:{Oper}, result:{ResultString}";
        }
    }
}
