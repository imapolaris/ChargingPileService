using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets.平台_充电桩
{
    public class StopChargingWithCardReplyPacket : StartChargingWithCardResultReplyPacket
    {
        public StopChargingWithCardReplyPacket() : base(PacketTypeEnum.StopChargingWithCardReply)
        {
            BodyLen = 1 + 11 + 8;
        }
    }
}
