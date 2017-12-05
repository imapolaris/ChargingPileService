using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetSecretKeyResultPacket : SetSecretKeyPacket
    {
        public GetSecretKeyResultPacket() : base(PacketTypeEnum.GetSecretKeyResult)
        {
            BodyLen = OperPacketBodyLen + 16 + 4;
        }
    }
}
