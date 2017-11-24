using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class SetReportIntervalPacket
    {
        private byte _stateReportInterval;
        /// <summary>
        /// 状态数据上报间隔
        /// </summary>
        public byte StateReportInterval
        {
            get { return _stateReportInterval; }
            set { _stateReportInterval = value; }
        }

        private byte _realDataReportInterval;
        /// <summary>
        /// 实时数据上报间隔
        /// </summary>
        public byte RealDataReportInterval
        {
            get { return _realDataReportInterval; }
            set { _realDataReportInterval = value; }
        }

    }
}
