using CPS.Communication.Service.DataPackets;
using CPS.Infrastructure.Models;
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
                    //SetElecPrice(item, sharpRate, peakRate, flatRate, valleyRate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public bool SetElecPrice(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            int sharpRate = data.GetIntValue("sr");
            int peakRate = data.GetIntValue("pr");
            int flatRate = data.GetIntValue("fr");
            int valleyRate = data.GetIntValue("vr");
            SetElecPricePacket packet = new SetElecPricePacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.SetElecPriceOper,
                SharpRate = sharpRate,
                PeakRate = peakRate,
                FlatRate = flatRate,
                ValleyRate = valleyRate,
            };

            return SetParams(id, sn, packet);
        }

        public void SetServicePriceForAll(List<string> sns, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
            if (sns == null || sns.Count <= 0)
                throw new ArgumentNullException("充电桩列表为空...");

            foreach (var item in sns)
            {
                try
                {
                    //SetServicePrice(item, sharpRate, peakRate, flatRate, valleyRate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public bool SetServicePrice(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            int sharpRate = data.GetIntValue("sr");
            int peakRate = data.GetIntValue("pr");
            int flatRate = data.GetIntValue("fr");
            int valleyRate = data.GetIntValue("vr");
            SetServicePricePacket packet = new SetServicePricePacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.SetServicePriceOper,
                SharpRate = sharpRate,
                PeakRate = peakRate,
                FlatRate = flatRate,
                ValleyRate = valleyRate,
            };

            return SetParams(id, sn, packet);
        }

        public bool SetReportInterval(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            byte stateReportInterval = data.GetByteValue("ri");
            byte rtDataReportInterval = data.GetByteValue("rtri");
            SetReportIntervalPacket packet = new SetReportIntervalPacket()
            {
                SerialNumber = sn,
                StateReportInterval = stateReportInterval,
                RealDataReportInterval = rtDataReportInterval,
                OperType = OperTypeEnum.SetReportIntervalOper,
            };

            return SetParams(id, sn, packet);
        }

        public bool SetTimePeriod(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            SetTimePeriodPacket packet = new SetTimePeriodPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.SetTimePeriodOper,
            };

            return SetParams(id, sn, packet);
        }

        public bool SetSecretKey(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            string key = data.GetStringValue("key");
            int timestamp = data.GetIntValue("timestamp");
            SetSecretKeyPacket packet = new SetSecretKeyPacket()
            {
                SerialNumber = sn,
                SecretKey = key,
                TimeStamp = timestamp,
                OperType = OperTypeEnum.SetTimePeriodOper,
            };

            return SetParams(id, sn, packet);
        }

        public bool SetQRcode(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            SetQRcodePacket packet = new SetQRcodePacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.SetTimePeriodOper,
            };

            return SetParams(id, sn, packet);
        }

        private bool SetParams(string id, string sn, OperPacketBase packet)
        {
            var client = MyServer.FindClientBySerialNumber(sn);
            if (client == null)
            {
                return false;
            }

            return StartSession(id, client, packet);
        }

        #endregion ====Set操作====

        #region  ====Get操作====
        public bool GetElecPrice(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            GetElecPricePacket packet = new GetElecPricePacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.GetElecPriceOper,
            };

            return GetParams(id, sn, packet);
        }

        public bool GetServicePrice(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            GetServicePricePacket packet = new GetServicePricePacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.GetServicePriceOper,
            };

            return GetParams(id, sn, packet);
        }

        public bool GetReportInterval(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            GetReportIntervalPacket packet = new GetReportIntervalPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.GetReportIntervalOper,
            };

            return GetParams(id, sn, packet);
        }

        public bool GetTimePeriod(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            GetTimePeriodPacket packet = new GetTimePeriodPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.GetTimePeriodOper,
            };

            return GetParams(id, sn, packet);
        }

        public bool GetSecretKey(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            GetSecretKeyPacket packet = new GetSecretKeyPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.GetSecretKeyOper,
            };

            return GetParams(id, sn, packet);
        }
        
        public bool GetSoftwareVer(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            GetSoftwareVerPacket packet = new GetSoftwareVerPacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.GetSoftwareVerOper,
            };

            return GetParams(id, sn, packet);
        }

        public bool GetQRcode(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            GetQRcodePacket packet = new GetQRcodePacket()
            {
                SerialNumber = sn,
                OperType = OperTypeEnum.GetSoftwareVerOper,
            };

            return GetParams(id, sn, packet);
        }

        private bool GetParams(string id, string sn, OperPacketBase packet)
        {
            var client = MyServer.FindClientBySerialNumber(sn);
            if (client == null)
            {
                return false;
            }

            return StartSession(id, client, packet);
        }

        #endregion  ====Get操作====
    }
}
