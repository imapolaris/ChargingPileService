using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class OperResultPacketBase : OperPacketBase
    {
        private OperResultPacketBase() { }

        public OperResultPacketBase(PacketTypeEnum pte) : base(pte)
        {

        }
    }
}
