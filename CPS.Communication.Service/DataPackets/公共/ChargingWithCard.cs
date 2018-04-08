using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets.公共
{
    /// <summary>
    /// 有卡充电
    /// </summary>
    public class ChargingWithCard : PacketBase
    {
        public ChargingWithCard(PacketTypeEnum pte) : base(pte)
        {

        }
    }
}
