using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetQRcodeResultPacket : SetQRcodePacket
    {
        public GetQRcodeResultPacket() : base(PacketTypeEnum.GetQRcodeResult)
        {
            //BodyLen = OperPacketBodyLen + 1 + 1 + 1 + x;
        }
    }
}
