using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Enums
{
    public enum ActionTypeEnum : byte
    {
        Startup = 0x01,
        Shutdown,
        GetChargingPileState,
    }

    public enum ResultTypeEnum : byte
    {
        Succeed = 0x01,
        Failed,
    }
}
