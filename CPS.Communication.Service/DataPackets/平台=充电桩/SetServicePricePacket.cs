using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class SetServicePricePacket : SetElecPricePacket
    {
        public SetServicePricePacket() : base(PacketTypeEnum.SetServicePrice)
        {
            BodyLen = OperPacketBodyLen + 4 + 4 + 4 + 4;
        }
    }
}
