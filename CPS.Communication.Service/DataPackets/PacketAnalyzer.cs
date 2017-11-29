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
                PacketTypeEnum command = header.Decode(buffer);
                if (!header.VerifyPacket())
                    throw new ArgumentException("报文异常...");

                PacketBase packet = null;
                switch (command)
                {
                    case PacketTypeEnum.None:
                        break;
                    case PacketTypeEnum.Login:
                        packet = new LoginPacket();
                        break;
                    case PacketTypeEnum.LoginResult:
                        packet = new LoginResultPacket();
                        break;
                    case PacketTypeEnum.HeartBeatClient:
                        packet = new HeartBeatPacket(PacketTypeEnum.HeartBeatClient);
                        break;
                    case PacketTypeEnum.HeartBeatServer:
                        break;
                    case PacketTypeEnum.Reboot:
                        break;
                    case PacketTypeEnum.RebootResult:
                        break;
                    case PacketTypeEnum.Confirm:
                        break;
                    case PacketTypeEnum.Deny:
                        break;
                    case PacketTypeEnum.SetElecPrice:
                        break;
                    case PacketTypeEnum.SetServicePrice:
                        break;
                    case PacketTypeEnum.SetReportInterval:
                        break;
                    case PacketTypeEnum.SetTimePeriod:
                        break;
                    case PacketTypeEnum.ChangeSecretKey:
                        break;
                    case PacketTypeEnum.GetElecPrice:
                        break;
                    case PacketTypeEnum.GetElecPriceResult:
                        break;
                    case PacketTypeEnum.GetServicePrice:
                        break;
                    case PacketTypeEnum.GetServicePriceResult:
                        break;
                    case PacketTypeEnum.GetReportInterval:
                        break;
                    case PacketTypeEnum.GetReportIntervalResult:
                        break;
                    case PacketTypeEnum.GetTimePeriod:
                        break;
                    case PacketTypeEnum.GetTimePeriodResult:
                        break;
                    case PacketTypeEnum.GetSecretKey:
                        break;
                    case PacketTypeEnum.GetSecretKeyResult:
                        break;
                    case PacketTypeEnum.GetSoftwareVer:
                        break;
                    case PacketTypeEnum.GetSoftwareVerResult:
                        break;
                    case PacketTypeEnum.SetQRcode:
                        break;
                    case PacketTypeEnum.GetQRcode:
                        break;
                    case PacketTypeEnum.GetQRcodeResult:
                        break;
                    case PacketTypeEnum.ChargingPileState:
                        break;
                    case PacketTypeEnum.GetChargingPileState:
                        break;
                    case PacketTypeEnum.SetCharging:
                        break;
                    case PacketTypeEnum.GetChargingResult:
                        break;
                    case PacketTypeEnum.RealDataOfCharging:
                        break;
                    case PacketTypeEnum.RecordOfCharging:
                        break;
                    case PacketTypeEnum.ConfirmOfCharging:
                        break;
                    case PacketTypeEnum.GetRecordOfCharging:
                        break;
                    case PacketTypeEnum.FaultMessage:
                        break;
                    case PacketTypeEnum.FaultMessageReply:
                        break;
                    case PacketTypeEnum.WarnMessage:
                        break;
                    case PacketTypeEnum.WarnMessageReply:
                        break;
                    case PacketTypeEnum.StartChargingWithCard:
                        break;
                    case PacketTypeEnum.StartChargingWithCardReply:
                        break;
                    case PacketTypeEnum.StartChargingWithCardResult:
                        break;
                    case PacketTypeEnum.StartChargingWithCardResultReply:
                        break;
                    case PacketTypeEnum.RealDataOfChargingWithCard:
                        break;
                    case PacketTypeEnum.StopChargingWithCard:
                        break;
                    case PacketTypeEnum.StopChargingWithCardReply:
                        break;
                    case PacketTypeEnum.SetBlacklist:
                        break;
                    case PacketTypeEnum.SetBlacklistResult:
                        break;
                    case PacketTypeEnum.SetWhitelist:
                        break;
                    case PacketTypeEnum.SetWhitelistResult:
                        break;
                    case PacketTypeEnum.GetBlacklist:
                        break;
                    case PacketTypeEnum.GetBlacklistResult:
                        break;
                    case PacketTypeEnum.GetWhitelist:
                        break;
                    case PacketTypeEnum.GetWhitelistResult:
                        break;
                    case PacketTypeEnum.UpgradeSoftware:
                        break;
                    case PacketTypeEnum.UpgradeSfotwareReply:
                        break;
                    case PacketTypeEnum.DownloadFinished:
                        break;
                    case PacketTypeEnum.DownloadFinishedReply:
                        break;
                    case PacketTypeEnum.InUpgradeState:
                        break;
                    case PacketTypeEnum.InUpgradeStateReply:
                        break;
                    case PacketTypeEnum.UpgradeResult:
                        break;
                    case PacketTypeEnum.UpgradeResultReply:
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
