using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class OperPacketBase : PacketBase
    {
        public const int OperPacketBodyLen = SerialNumberLen + 4;

        public OperPacketBase() { }

        public OperPacketBase(PacketTypeEnum pte) : base(pte)
        {
            
        }

        private int _oper;
        /// <summary>
        /// 操作序列号
        /// </summary>
        protected int Oper
        {
            get { return _oper; }
            set { _oper = value; }
        }

        public OperTypeEnum OperType
        {
            get
            {
                return (OperTypeEnum)this._oper;
            }
            set {
                this._oper = (int)value;
            }
        }

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            var start = SerialNumberLen;
            byte[] temp = BitConverter.GetBytes(this._oper);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            var start = SerialNumberLen;
            this._oper = BitConverter.ToInt32(buffer, start);
            return this;
        }
    }
}
