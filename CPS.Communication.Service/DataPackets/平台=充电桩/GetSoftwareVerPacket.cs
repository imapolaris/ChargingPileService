using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetSoftwareVerPacket : OperPacketBase
    {
        public GetSoftwareVerPacket() : base(PacketTypeEnum.GetSoftwareVer)
        {
            BodyLen = OperPacketBodyLen;
        }
    }
}
