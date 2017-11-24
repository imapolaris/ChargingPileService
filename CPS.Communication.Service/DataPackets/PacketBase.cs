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
        private PacketType _command;

        public PacketBase()
        {
            _header = new PacketHeader(_command);
        }

        public PacketBase(PacketType command) : this()
        {
            _command = command;
        }

        private PacketHeader _header;
        /// <summary>
        /// 报文头
        /// </summary>
        protected PacketHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        private string _serialNumber;
        /// <summary>
        /// 电桩编号
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        public PacketType Command { get { return this._command; } }

        /// <summary>
        /// 报文体长度
        /// </summary>
        protected int BodyLen { get; set; }

        public PacketBase AnalysePacket(byte[] buffer)
        {
            try
            {
                PacketType command = this._header.Decode(buffer);
                this._command = this._header.Command;

                PacketBase packet = null;
                switch (command)
                {
                    case PacketType.Login:
                        packet = new LoginPacket();
                        break;
                    case PacketType.LoginResult:
                        break;
                    default:
                        break;
                }

                byte[] body = new byte[buffer.Length - HeaderLen];
                Array.Copy(buffer, HeaderLen, body, 0, body.Length);
                packet.Decode(body);

                return packet;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public byte[] GeneratePacket()
        {
            byte[] body = this.Encode();
            this._header.BodyLen = body.Length;
            byte[] header = this._header.Encode();

            int len = header.Length + body.Length;
            if (len < HeaderLen)
                throw new ArgumentOutOfRangeException("包格式不正确...");

            byte[] packet = new byte[len];
            Array.Copy(header, 0, packet, 0, header.Length);
            Array.Copy(body, 0, packet, header.Length, body.Length);

            return packet;
        }

        public virtual byte[] Encode()
        {
            byte[] body = new byte[SerialNumberLen];

            byte[] temp = EncodeHelper.GetBytes(this._serialNumber);
            Array.Copy(temp, 0, body, 0, SerialNumberLen);
            return body;
        }

        protected virtual PacketBase Decode(byte[] buffer) {

            byte[] body = new byte[SerialNumberLen];
           
            Array.Copy(buffer, 0, body, 0, SerialNumberLen);
            this._serialNumber = EncodeHelper.GetString(body, 0, SerialNumberLen);
            return this;
        }
    }
}
