using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetServicePriceResultPacket : GetElecPriceResultPacket
    {
        public GetServicePriceResultPacket() : base(PacketTypeEnum.GetServicePriceResult)
        {
            BodyLen = OperPacketBodyLen + 4 + 4 + 4 + 4 + 4;
        }
    }
}
