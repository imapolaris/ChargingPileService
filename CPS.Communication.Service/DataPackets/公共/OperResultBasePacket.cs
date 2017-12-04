using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class OperResultBasePacket : PacketBase
    {
        private OperResultBasePacket() { }

        public OperResultBasePacket(PacketTypeEnum pte) : base(pte)
        {

        }

        private int _oper;
        /// <summary>
        /// 操作序列号
        /// </summary>
        public int Oper
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

        public virtual string ResultString
        {
            get
            {
                return this._result == 1 ? "成功" : "失败";
            }
        }

        public virtual string ResultStringEn
        {
            get
            {
                return this._result == 1 ? "succeed" : "failed";
            }
        }

        public virtual bool ResultBoolean
        {
            get
            {
                return this._result == 1 ? true : false;
            }
        }

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            var start = SerialNumberLen;
            byte[] temp = BitConverter.GetBytes(this._oper);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            body[start] = this._result;
            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            var start = SerialNumberLen;
            this._oper = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._result = buffer[start];
            return this;
        }
    }
}
