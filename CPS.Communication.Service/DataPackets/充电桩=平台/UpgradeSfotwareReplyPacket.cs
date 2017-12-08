using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class UpgradeSoftwareReplyPacket : OperResultPacketBase
    {
        public UpgradeSoftwareReplyPacket() : base(PacketTypeEnum.UpgradeSoftwareReply)
        {
            BodyLen = OperPacketBodyLen + 1;
        }
    }
}
