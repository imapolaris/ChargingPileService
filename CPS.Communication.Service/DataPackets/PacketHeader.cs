using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class PacketHeader
    {
        private byte _ver;
        /// <summary>
        /// 协议版本号
        /// </summary>
        public byte Ver
        {
            get { return _ver; }
            set { _ver = value; }
        }

        private byte _attr;
        /// <summary>
        /// 消息包属性
        /// </summary>
        public byte Attr
        {
            get { return _attr; }
            set { _attr = value; }
        }

        private short _command;
        /// <summary>
        /// 消息命令字
        /// </summary>
        public short Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private string _reserveP;
        /// <summary>
        /// 预留字段
        /// </summary>
        public string ReserveP
        {
            get { return _reserveP; }
            set { _reserveP = value; }
        }

        private int _bodyLen;
        /// <summary>
        /// 消息体长度
        /// </summary>
        public int BodyLen
        {
            get { return _bodyLen; }
            set { _bodyLen = value; }
        }

        private short _verifyCode;
        /// <summary>
        /// 消息校验码
        /// </summary>
        public short VerifyCode
        {
            get { return _verifyCode; }
            set { _verifyCode = value; }
        }
    }
}
