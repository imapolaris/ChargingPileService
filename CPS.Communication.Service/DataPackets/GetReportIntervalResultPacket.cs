using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class GetReportIntervalResultPacket : SetReportIntervalPacket
    {
        private byte[] _oper;

        public byte[] Oper
        {
            get { return _oper; }
            set { _oper = value; }
        }

    }
}
