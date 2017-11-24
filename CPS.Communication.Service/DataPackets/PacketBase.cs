using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class PacketBase
    {
        public const int HeaderLen = 14;
        public const int SerialNumberLen = 16;
        public const int MinPacketLen = 14 + 16;
        /// <summary>
        /// 报文头中报文体长度的位置
        /// </summary>
        public const int BodyLenIndex = 8;

        private string _serialNumber;
        /// <summary>
        /// 电桩编号
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        private PacketHeader _header;

        public PacketHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

    }
}
