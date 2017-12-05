using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetQRcodePacket : OperPacketBase
    {
        public GetQRcodePacket() : base(PacketTypeEnum.GetQRcode)
        {
            BodyLen = OperPacketBodyLen;
        }
    }
}
