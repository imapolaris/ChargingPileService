using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class PacketAnalyzer
    {
        public static PacketBase AnalysePacket(byte[] buffer)
        {
            try
            {
                PacketHeader header = new PacketHeader();
                PacketType command = header.Decode(buffer);
                if (!header.VerifyPacket())
                    throw new ArgumentException("报文异常...");

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

                if (packet == null)
                    throw new ArgumentOutOfRangeException("报文命令字超出范围...");

                byte[] body = new byte[buffer.Length - PacketBase.HeaderLen];
                Array.Copy(buffer, PacketBase.HeaderLen, body, 0, body.Length);
                packet.Decode(body);
                packet.Header = header;

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

        public static byte[] GeneratePacket(PacketBase packet)
        {
            byte[] body = packet.Encode();
            packet.Header.BodyLen = body.Length;
            byte[] header = packet.Header.Encode();

            int len = header.Length + body.Length;
            if (len < PacketBase.HeaderLen)
                throw new ArgumentOutOfRangeException("包格式不正确...");

            byte[] buffer = new byte[len];
            Array.Copy(header, 0, buffer, 0, header.Length);
            Array.Copy(body, 0, buffer, header.Length, body.Length);

            return buffer;
        }
    }
}
