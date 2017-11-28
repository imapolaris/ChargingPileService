using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class OperBasePacket
    {
        private byte[] _oper;
        /// <summary>
        /// 操作序列号
        /// </summary>
        public byte[] Oper
        {
            get { return _oper; }
            set { _oper = value; }
        }
            
    }
}
