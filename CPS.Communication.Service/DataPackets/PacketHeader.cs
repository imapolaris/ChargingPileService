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
        public PacketHeader(PacketType command)
        {
            this._command = command;
        }

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

        private PacketType _command;
        /// <summary>
        /// 消息命令字
        /// </summary>
        public PacketType Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private string _reserveP = "";
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

        private bool VerifyPacket()
        {
            if (this._verifyCode == (short)(PacketBase.HeaderLen - 2))
                return true;
            return false;
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
            Array.Copy(temp, 0, header, start, 2);
            start += 2;
            temp = EncodeHelper.GetBytes(this._reserveP);
            Array.Copy(temp, 0, header, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._bodyLen);
            Array.Copy(temp, 0, header, start, 4);
            start += 4;
            temp = BitConverter.GetBytes(this._verifyCode);
            Array.Copy(temp, 0, header, start, 2);

            return header;
        }

        public PacketType Decode(byte[] buffer)
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
            this._command = (PacketType)Enum.Parse(typeof(PacketType), BitConverter.ToInt16(header, start).ToString());
            start += 2;
            byte[] temp = new byte[4];
            Array.Copy(header, start, temp, 0, 4);
            this._reserveP = EncodeHelper.GetString(temp);
            start += 4;
            this._bodyLen = BitConverter.ToInt32(header, start);
            start += 4;
            this._verifyCode = BitConverter.ToInt16(header, start);

            return this._command;
        }
    }
}
