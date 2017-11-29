using CPS.Infrastructure.Utils;
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
        private PacketTypeEnum _command;

        public PacketBase() : this(PacketTypeEnum.None)
        {
        }

        public PacketBase(PacketTypeEnum command)
        {
            _command = command;
            _header = new PacketHeader(_command);
        }

        private PacketHeader _header;
        /// <summary>
        /// 报文头
        /// </summary>
        public PacketHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        private string _serialNumber = "";
        /// <summary>
        /// 电桩编号
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        public PacketTypeEnum Command { get { return this.Header == null ? PacketTypeEnum.None : this.Header.Command; } }

        /// <summary>
        /// 报文体长度
        /// </summary>
        protected int BodyLen { get; set; }

        public virtual byte[] Encode()
        {
            byte[] body = new byte[BodyLen];

            byte[] temp = EncodeHelper.GetBytes(this._serialNumber);
            Array.Copy(temp, 0, body, 0, temp.Length);
            return body;
        }

        public virtual PacketBase Decode(byte[] buffer)
        {
            byte[] body = new byte[SerialNumberLen];
           
            Array.Copy(buffer, 0, body, 0, SerialNumberLen);
            this._serialNumber = EncodeHelper.GetString(body, 0, SerialNumberLen);
            return this;
        }
    }
}
