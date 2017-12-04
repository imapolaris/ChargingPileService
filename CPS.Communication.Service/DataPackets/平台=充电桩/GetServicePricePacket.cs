using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetServicePricePacket : OperPacketBase
    {
        public GetServicePricePacket() : base(PacketTypeEnum.GetServicePrice)
        {
            BodyLen = OperPacketBodyLen;
        }
    }
}
