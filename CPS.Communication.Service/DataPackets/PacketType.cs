using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public enum PacketType : short
    {
        None,
        Login =1,
        LoginResult,
    }
}
