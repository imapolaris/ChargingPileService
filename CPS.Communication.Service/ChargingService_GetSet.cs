using CPS.Communication.Service.DataPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    /// <summary>
    /// 设置查询操作
    /// </summary>
    public partial class ChargingService
    {
        #region ====Set操作====

        public void SetElecPriceForAll(List<string> sns, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
            if (sns == null || sns.Count <= 0)
                throw new ArgumentNullException("充电桩列表为空...");

            foreach (var item in sns)
            {
                try
                {
                    SetElecPrice(item, sharpRate, peakRate, flatRate, valleyRate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task<bool> SetElecPrice(string serialNumber, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
            SetElecPricePacket packet = new SetElecPricePacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.SetElecPriceOper,
                SharpRate = sharpRate,
                PeakRate = peakRate,
                FlatRate = flatRate,
                ValleyRate = valleyRate,
            };

            return await SetParams(serialNumber, packet);
        }

        public void SetServicePriceForAll(List<string> sns, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
            if (sns == null || sns.Count <= 0)
                throw new ArgumentNullException("充电桩列表为空...");

            foreach (var item in sns)
            {
                try
                {
                    SetServicePrice(item, sharpRate, peakRate, flatRate, valleyRate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task<bool> SetServicePrice(string serialNumber, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
            SetServicePricePacket packet = new SetServicePricePacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.SetServicePriceOper,
                SharpRate = sharpRate,
                PeakRate = peakRate,
                FlatRate = flatRate,
                ValleyRate = valleyRate,
            };

            return await SetParams(serialNumber, packet);
        }

        public async Task<bool> SetReportInterval(string serialNumber, byte stateReportInterval, byte rtDataReportInterval)
        {
            SetReportIntervalPacket packet = new SetReportIntervalPacket()
            {
                SerialNumber = serialNumber,
                StateReportInterval = stateReportInterval,
                RealDataReportInterval = rtDataReportInterval,
                OperType = OperTypeEnum.SetReportIntervalOper,
            };

            return await SetParams(serialNumber, packet);
        }

        public async Task<bool> SetTimePeriod(string serialNumber)
        {
            SetTimePeriodPacket packet = new SetTimePeriodPacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.SetTimePeriodOper,
            };

            return await SetParams(serialNumber, packet);
        }

        public async Task<bool> SetSecretKey(string serialNumber, string key, int timestamp)
        {
            SetSecretKeyPacket packet = new SetSecretKeyPacket()
            {
                SerialNumber = serialNumber,
                SecretKey = key,
                TimeStamp = timestamp,
                OperType = OperTypeEnum.SetTimePeriodOper,
            };

            return await SetParams(serialNumber, packet);
        }

        public async Task<bool> SetQRcode(string serialNumber)
        {
            SetQRcodePacket packet = new SetQRcodePacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.SetTimePeriodOper,
            };

            return await SetParams(serialNumber, packet);
        }

        private async Task<bool> SetParams(string serialNumber, OperPacketBase packet)
        {
            var client = MyServer.FindClientBySerialNumber(serialNumber);
            if (client == null)
            {
                return false;
                //throw new ArgumentNullException("客户端尚未连接...");
            }

            var result = await StartSession(client, packet);
            if (result == null)
                return false;
            else
            {
                if (result is ConfirmPacket)
                    return true;
                else
                    return false;
            }
        }

        #endregion ====Set操作====

        #region  ====Get操作====
        public async Task<GetElecPriceResultPacket> GetElecPrice(string serialNumber)
        {
            GetElecPricePacket packet = new GetElecPricePacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.GetElecPriceOper,
            };

            return await GetParams<GetElecPriceResultPacket>(serialNumber, packet);
        }

        public async Task<GetServicePriceResultPacket> GetServicePrice(string serialNumber)
        {
            GetServicePricePacket packet = new GetServicePricePacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.GetServicePriceOper,
            };

            return await GetParams<GetServicePriceResultPacket>(serialNumber, packet);
        }

        public async Task<GetReportIntervalResultPacket> GetReportInterval(string serialNumber)
        {
            GetReportIntervalPacket packet = new GetReportIntervalPacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.GetReportIntervalOper,
            };

            return await GetParams<GetReportIntervalResultPacket>(serialNumber, packet);
        }

        public async Task<GetTimePeriodResultPacket> GetTimePeriod(string serialNumber)
        {
            GetTimePeriodPacket packet = new GetTimePeriodPacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.GetTimePeriodOper,
            };

            return await GetParams<GetTimePeriodResultPacket>(serialNumber, packet);
        }

        public async Task<GetSecretKeyResultPacket> GetSecretKey(string serialNumber)
        {
            GetSecretKeyPacket packet = new GetSecretKeyPacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.GetSecretKeyOper,
            };

            return await GetParams<GetSecretKeyResultPacket>(serialNumber, packet);
        }
        
        public async Task<GetSoftwareVerResultPacket> GetSoftwareVer(string serialNumber)
        {
            GetSoftwareVerPacket packet = new GetSoftwareVerPacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.GetSoftwareVerOper,
            };

            return await GetParams<GetSoftwareVerResultPacket>(serialNumber, packet);
        }

        public async Task<GetQRcodeResultPacket> GetQRcode(string serialNumber)
        {
            GetQRcodePacket packet = new GetQRcodePacket()
            {
                SerialNumber = serialNumber,
                OperType = OperTypeEnum.GetSoftwareVerOper,
            };

            return await GetParams<GetQRcodeResultPacket>(serialNumber, packet);
        }

        private async Task<T> GetParams<T>(string serialNumber, OperPacketBase packet) where T : PacketBase, new()
        {
            var client = MyServer.FindClientBySerialNumber(serialNumber);
            if (client == null)
            {
                return null;
                //throw new ArgumentNullException("客户端尚未连接...");
            }

            return await StartSession(client, packet) as T;
        }

        #endregion  ====Get操作====
    }
}
