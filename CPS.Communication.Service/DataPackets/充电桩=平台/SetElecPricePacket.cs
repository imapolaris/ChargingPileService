using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class SetElecPricePacket : OperBasePacket
    {
        private byte[] _sharpRate;
        /// <summary>
        /// 尖费率
        /// </summary>
        public byte[] SharpRate
        {
            get { return _sharpRate; }
            set { _sharpRate = value; }
        }

        private byte[] _peakRate;
        /// <summary>
        /// 峰费率
        /// </summary>
        public byte[] PeakRate
        {
            get { return _peakRate; }
            set { _peakRate = value; }
        }

        private byte[] _flatRate;
        /// <summary>
        /// 平费率
        /// </summary>
        public byte[] FlatRate
        {
            get { return _flatRate; }
            set { _flatRate = value; }
        }

        private byte[] _valleyRate;
        /// <summary>
        /// 谷费率
        /// </summary>
        public byte[] Valleyrate
        {
            get { return _valleyRate; }
            set { _valleyRate = value; }
        }
    }
}
