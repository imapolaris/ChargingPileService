using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class PacketHeader
    {
        public PacketHeader()
        {
        }

        public PacketHeader(PacketTypeEnum command) : this()
        {
            this._command = command;
        }

        private byte _ver = 0x68;
        /// <summary>
        /// 协议版本号
        /// </summary>
        public byte Ver
        {
            get { return _ver; }
            set { _ver = value; }
        }

        private byte _attr = 0x00;
        /// <summary>
        /// 消息包属性
        /// </summary>
        public byte Attr
        {
            get { return _attr; }
            set { _attr = value; }
        }

        /// <summary>
        /// 是否为系统消息
        /// </summary>
        public bool IsSysMessage
        {
            get
            {
                return this._attr == 0x01 || this._attr == 0x03;
            }
        }

        /// <summary>
        /// 数据是否压缩
        /// </summary>
        public bool Compressed
        {
            get
            {
                return this._attr == 0x02 || this._attr == 0x03;
            }
        }

        private PacketTypeEnum _command;
        /// <summary>
        /// 消息命令字
        /// </summary>
        public PacketTypeEnum Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private int _reserveP = 0x00;
        /// <summary>
        /// 预留字段
        /// </summary>
        public int ReserveP
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

        public bool VerifyPacket()
        {
            if (this._verifyCode == GetVerifyCode())
                return true;
            return false;
        }

        private short GetVerifyCode()
        {
            return (short)(this._ver + this._attr + (int)this._command + this._reserveP + this._bodyLen);
        }

        public byte[] Encode()
        {
            byte[] header = new byte[PacketBase.HeaderLen];
            int start = 0;
            header[start] = this._ver;
            ++start;
            header[start] = this._attr;
            ++start;
            byte[] temp = BitConverter.GetBytes((short)this._command);
            Array.Copy(temp, 0, header, start, temp.Length);
            start += 2;
            temp = BitConverter.GetBytes(this._reserveP);
            Array.Copy(temp, 0, header, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._bodyLen);
            Array.Copy(temp, 0, header, start, temp.Length);
            start += 4;
            this._verifyCode = GetVerifyCode();
            temp = BitConverter.GetBytes(this._verifyCode);
            Array.Copy(temp, 0, header, start, temp.Length);

            return header;
        }

        public PacketTypeEnum Decode(byte[] buffer)
        {
            if (buffer == null || buffer.Length < PacketBase.HeaderLen)
                throw new ArgumentOutOfRangeException("数据包格式不正确...");

            byte[] header = new byte[PacketBase.HeaderLen];
            Array.Copy(buffer, 0, header, 0, PacketBase.HeaderLen);

            int start = 0;
            this._ver = header[start];
            ++start;
            this._attr = header[start];
            ++start;
            this._command = (PacketTypeEnum)Enum.Parse(typeof(PacketTypeEnum), BitConverter.ToInt16(header, start).ToString());
            start += 2;
            byte[] temp = new byte[4];
            Array.Copy(header, start, temp, 0, 4);
            this._reserveP = BitConverter.ToInt32(header, start);
            start += 4;
            this._bodyLen = BitConverter.ToInt32(header, start);
            start += 4;
            this._verifyCode = BitConverter.ToInt16(header, start);

            return this._command;
        }
    }
}
