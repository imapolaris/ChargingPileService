using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetElecPricePacket : OperPacketBase
    {
        public GetElecPricePacket() : base(PacketTypeEnum.GetElecPrice)
        {
            BodyLen = OperPacketBodyLen;
        }
    }
}
