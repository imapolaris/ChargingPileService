using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    class RebootResultPacket : OperBasePacket
    {
        private byte _result;
        /// <summary>
        /// 结果信息
        /// 1：成功；2：失败
        /// </summary>
        public byte Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}
