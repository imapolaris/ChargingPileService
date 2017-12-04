using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetReportIntervalResultPacket : SetReportIntervalPacket
    {
        public GetReportIntervalResultPacket() : base(PacketTypeEnum.GetReportIntervalResult)
        {
            BodyLen = OperPacketBodyLen;
        }
    }
}
