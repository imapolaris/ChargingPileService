using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Enums
{
    public enum MQMessageType
    {
        StartCharging = 0x01,
        StopCharging,

        GetChargingPileState,
    }
}
