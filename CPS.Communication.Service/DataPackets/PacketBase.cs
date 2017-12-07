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
        protected const double RateCoefficient = 0.0001;
        protected const int CardNoLen = 16;

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

        /// <summary>
        /// 报文名称
        /// </summary>
        public string Name { get; set; }

        public byte[] Encode()
        {
            byte[] sn = new byte[SerialNumberLen];

            byte[] temp = EncodeHelper.GetBytes(this._serialNumber);
            Array.Copy(temp, 0, sn, 0, temp.Length);

            byte[] body = EncodeBody();
            // 对消息内容进行加密
            body = EncryptHelper.Encrypt(body);

            return BytesHelper.Combine(sn, body);
        }

        public PacketBase Decode(byte[] buffer)
        {
            byte[] sn = new byte[SerialNumberLen];

            Array.Copy(buffer, 0, sn, 0, SerialNumberLen);
            this._serialNumber = EncodeHelper.GetString(sn, 0, SerialNumberLen);

            byte[] body = BytesHelper.SubArray(buffer, SerialNumberLen, buffer.Length - SerialNumberLen);
            // 对消息内容进行解密
            body = EncryptHelper.Decrypt(body);
            return DecodeBody(body);
        }

        public virtual byte[] EncodeBody()
        {
            return new byte[BodyLen];
        }

        public virtual PacketBase DecodeBody(byte[] buffer)
        {
            return this;
        }
    }
}
