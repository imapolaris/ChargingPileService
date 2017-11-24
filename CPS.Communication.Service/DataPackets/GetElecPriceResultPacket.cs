using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class GetElecPriceResultPacket : SetElecPricePacket
    {
        private int _timestamp;
        /// <summary>
        /// 充电桩当前时间
        /// </summary>
        public int TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

    }
}
