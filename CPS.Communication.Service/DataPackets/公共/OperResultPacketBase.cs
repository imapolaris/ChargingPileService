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

        public OperResultPacketBase(PacketTypeEnum pte) : base(pte) { }

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

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            var start = OperPacketBodyLen;
            body[start] = this._result;
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            var start = OperPacketBodyLen;
            this._result = buffer[start];
            return this;
        }

    }
}
