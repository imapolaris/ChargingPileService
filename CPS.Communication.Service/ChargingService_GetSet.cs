using CPS.Communication.Service.DataPackets;
using CPS.Infrastructure.Models;
using CPS.Infrastructure.Utils;
using Newtonsoft.Json.Linq;
using Soaring.WebMonter.Contract;
using Soaring.WebMonter.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    /// <summary>
    /// 设置查询操作
    /// </summary>
    public partial class ChargingService
    {
        #region ====Set操作====

        public bool SetPriceAll(UniversalData data)
        {
            try
            {
                var stationId = data.GetStringValue("stationId");
                if (string.IsNullOrEmpty(stationId))
                {
                    Logger.Info("无法下发费率");
                    return false;
                }

                var systemDbContext = new SystemDbContext();
                List<string> sns = systemDbContext.ChargingPiles.Where(_ => _.StationId == stationId).Select(_ => _.SerialNumber).ToList();
                int sharpRate = data.GetIntValue("sr");
                int peakRate = data.GetIntValue("pr");
                int flatRate = data.GetIntValue("fr");
                int valleyRate = data.GetIntValue("vr");
                int priceType = data.GetIntValue("priceType");

                if (sns == null || sns.Count <= 0)
                {
                    Logger.Error("充电桩列表为空...");
                    return false;
                }

                OperPacketBase packet = null;
                if (priceType == 0) // 电价
                {
                    packet = new SetElecPricePacket()
                    {
                        OperType = OperTypeEnum.SetElecPriceOper,
                        SharpRate = sharpRate,
                        PeakRate = peakRate,
                        FlatRate = flatRate,
                        ValleyRate = valleyRate,
                    };
                }
                else // 服务费
                {
                    packet = new SetServicePricePacket()
                    {
                        OperType = OperTypeEnum.SetServicePriceOper,
                        SharpRate = sharpRate,
                        PeakRate = peakRate,
                        FlatRate = flatRate,
                        ValleyRate = valleyRate,
                    };
                }

                foreach (var sn in sns)
                {
                    try
                    {
                        packet.SerialNumber = sn;
                        var result = SetParams(Guid.NewGuid().ToString(), sn, packet);
                        if (!result)
                        {
                            Logger.Info($"向电桩{sn}下发费率失败！");
                        }
                        else
                        {
                            Logger.Info($"向电桩{sn}下发费率成功！");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info($"向电桩{sn}下发费率失败！");
                        Logger.Error(ex);
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public bool SetPriceSingle(UniversalData data)
        {
            string id = data.GetStringValue("id");
            string sn = data.GetStringValue("sn");
            int sharpRate = data.GetIntValue("sr");
            int peakRate = data.GetIntValue("pr");
            int flatRate = data.GetIntValue("fr");
            int valleyRate = data.GetIntValue("vr");
            int priceType = data.GetIntValue("priceType");
            if (priceType == 0) // 电费
            {
                return SetElecPrice(id, sn, sharpRate, peakRate, flatRate, valleyRate);
            }
            else // 服务费
            {
                return SetServicePrice(id, sn, sharpRate, peakRate, flatRate, valleyRate);
            }
        }

        private bool SetElecPrice(string id, string sn, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
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

        private bool SetServicePrice(string id, string sn, int sharpRate, int peakRate, int flatRate, int valleyRate)
        {
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
            try
            {
                var stationId = data.GetStringValue("stationId");
                if (string.IsNullOrEmpty(stationId))
                {
                    Logger.Info("无法下发尖峰平谷时间段");
                    return false;
                }
                var systemDbContext = new SystemDbContext();
                List<string> sns = systemDbContext.ChargingPiles.Where(_ => _.StationId == stationId).Select(_ => _.SerialNumber).ToList();

                string id = data.GetStringValue("id");
                byte periodType = data.GetByteValue("periodType");
                byte numberOfSr = data.GetByteValue("sr");
                byte[] srs = new byte[numberOfSr * 2];
                var ja = (JArray)data.GetValue("srs");
                for (int i = 0; i < ja.Count; i++)
                {
                    var jt = ja[i];
                    var jtv = jt.Value<byte>();
                    srs[i] = jtv;
                }
                byte numberOfPr = data.GetByteValue("pr");
                byte[] prs = new byte[numberOfPr * 2];
                ja = (JArray)data.GetValue("prs");
                for (int i = 0; i < ja.Count; i++)
                {
                    var jt = ja[i];
                    var jtv = jt.Value<byte>();
                    prs[i] = jtv;
                }
                byte numberOfFr = data.GetByteValue("fr");
                byte[] frs = new byte[numberOfFr * 2];
                ja = (JArray)data.GetValue("frs");
                for (int i = 0; i < ja.Count; i++)
                {
                    var jt = ja[i];
                    var jtv = jt.Value<byte>();
                    frs[i] = jtv;
                }
                byte numberOfVr = data.GetByteValue("vr");
                byte[] vrs = new byte[numberOfVr * 2];
                ja = (JArray)data.GetValue("vrs");
                for (int i = 0; i < ja.Count; i++)
                {
                    var jt = ja[i];
                    var jtv = jt.Value<byte>();
                    vrs[i] = jtv;
                }

                SetTimePeriodPacket packet = new SetTimePeriodPacket()
                {
                    PeriodType = periodType,
                    NumberOfSharpPeriod = numberOfSr,
                    SharpPeriods = srs,
                    NumberOfPeakPeriod = numberOfPr,
                    PeakPeriods = prs,
                    NumberOfFlatPeriod = numberOfFr,
                    FlatPeriods = frs,
                    NumberOfValleyPeriod = numberOfVr,
                    ValleyPeriods = vrs,
                    OperType = OperTypeEnum.SetTimePeriodOper,
                };

                foreach (var sn in sns)
                {
                    packet.SerialNumber = sn;
                    var result = SetParams(id, sn, packet);
                    if (!result)
                    {
                        Logger.Info($"充电桩{sn}下发尖峰平谷时间失败！");
                    }
                    else
                    {
                        Logger.Info($"充电桩{sn}下发尖峰平谷时间成功！");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
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
                Logger.Error($"{sn}客户端尚未连接...");
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
                Logger.Error($"{sn}客户端尚未连接...");
                return false;
            }

            return StartSession(id, client, packet);
        }

        private void GetSetting(Client client, GetSettingPacket packet)
        {
            if (client == null || packet == null || string.IsNullOrEmpty(packet.SerialNumber))
            {
                Logger.Error("请求费率、时间段等配置的参数错误！");
                return;
            }

            ThreadPool.QueueUserWorkItem((state) =>
            {
                try
                {
                    // 查找所属电站
                    var sn = packet.SerialNumber;
                    var systemDbContext = new SystemDbContext();
                    var cp = systemDbContext.ChargingPiles.Where(_ => _.SerialNumber == sn).FirstOrDefault();
                    if (cp == null)
                    {
                        Logger.Info($"充电桩{sn}不存在，无法查询相关配置");
                        return;
                    }
                    var stationId = cp.StationId;

                    // 下发电价
                    var costDefine = systemDbContext.Sys_CostDefines.Where(x => x.StationId == stationId && x.SettingType == SettingOption.Price).ToList();
                    if (costDefine?.Count < 1)
                    {
                        Logger.Info($"还没有为电站{stationId}配置电价...");
                    }
                    else
                    {
                        var packet1 = new SetElecPricePacket()
                        {
                            SerialNumber = sn,
                            OperType = OperTypeEnum.SetElecPriceOper,
                            SharpRate = (byte)(costDefine.Where(_ => _.Option == CastOption.One).FirstOrDefault()?.Fee ?? 0),
                            PeakRate = (byte)(costDefine.Where(_ => _.Option == CastOption.Two).FirstOrDefault()?.Fee ?? 0),
                            FlatRate = (byte)(costDefine.Where(_ => _.Option == CastOption.Three).FirstOrDefault()?.Fee ?? 0),
                            ValleyRate = (byte)(costDefine.Where(_ => _.Option == CastOption.Four).FirstOrDefault()?.Fee ?? 0),
                        };
                        var result = client.Send(packet1);
                        if (result)
                        {
                            Logger.Info($"为电桩{sn}下发电价成功！");
                        }
                        else
                        {
                            Logger.Info($"为电桩{sn}下发电价失败！");
                        }

                        Thread.Sleep(2500);
                    }


                    // 下发服务费
                    var serviceDefine = systemDbContext.Sys_CostDefines.Where(_ => _.StationId == stationId && _.SettingType == SettingOption.Service).ToList();
                    if (serviceDefine?.Count < 1)
                    {
                        Logger.Info($"还没有为电站{stationId}配置服务费...");
                    }
                    else
                    {
                        var packet2 = new SetServicePricePacket()
                        {
                            SerialNumber = sn,
                            OperType = OperTypeEnum.SetServicePriceOper,
                            SharpRate = (byte)(serviceDefine.Where(_ => _.Option == CastOption.One).FirstOrDefault()?.Fee ?? 0),
                            PeakRate = (byte)(serviceDefine.Where(_ => _.Option == CastOption.Two).FirstOrDefault()?.Fee ?? 0),
                            FlatRate = (byte)(serviceDefine.Where(_ => _.Option == CastOption.Three).FirstOrDefault()?.Fee ?? 0),
                            ValleyRate = (byte)(serviceDefine.Where(_ => _.Option == CastOption.Four).FirstOrDefault()?.Fee ?? 0),
                        };
                        var result = client.Send(packet2);
                        if (result)
                        {
                            Logger.Info($"为电桩{sn}下发服务费成功！");
                        }
                        else
                        {
                            Logger.Info($"为电桩{sn}下发服务费失败！");
                        }

                        Thread.Sleep(2500);
                    }

                    // 下发电价时间段
                    var pricePeriodDefine = systemDbContext.ChargingStandards.Where(_ => _.StationId == stationId && _.IsValid).ToList();
                    if (pricePeriodDefine?.Count < 1)
                    {
                        Logger.Info($"还没有为电站{stationId}配置电价时间段...");
                    }
                    else
                    {
                        var srdata = pricePeriodDefine.Where(_ => _.Catalog == "0")?.Select(_ => _.StartTime.Hours).ToList();
                        var sr = (byte)srdata.Count;
                        var srs = new byte[sr * 2];
                        for (int i = 0; i < sr; i++)
                        {
                            srs[2 * i] = (byte)srdata[i];
                            srs[2 * i + 1] = (byte)(srdata[i] + 1);
                        }
                        var prdata = pricePeriodDefine.Where(_ => _.Catalog == "1")?.Select(_ => _.StartTime.Hours).ToList();
                        var pr = (byte)prdata.Count;
                        var prs = new byte[pr * 2];
                        for (int i = 0; i < pr; i++)
                        {
                            prs[2 * i] = (byte)prdata[i];
                            prs[2 * i + 1] = (byte)(prdata[i] + 1);
                        }
                        var frdata = pricePeriodDefine.Where(_ => _.Catalog == "2")?.Select(_ => _.StartTime.Hours).ToList();
                        var fr = (byte)frdata.Count;
                        var frs = new byte[fr * 2];
                        for (int i = 0; i < fr; i++)
                        {
                            frs[2 * i] = (byte)frdata[i];
                            frs[2 * i + 1] = (byte)(frdata[i] + 1);
                        }
                        var vrdata = pricePeriodDefine.Where(_ => _.Catalog == "3")?.Select(_ => _.StartTime.Hours).ToList();
                        var vr = (byte)vrdata.Count;
                        var vrs = new byte[vr * 2];
                        for (int i = 0; i < vr; i++)
                        {
                            frs[2 * i] = (byte)vrdata[i];
                            frs[2 * i + 1] = (byte)(vrdata[i] + 1);
                        }
                        var packet3 = new SetTimePeriodPacket()
                        {
                            SerialNumber = sn,
                            OperType = OperTypeEnum.SetTimePeriodOper,
                            PeriodType = 0,
                            NumberOfSharpPeriod = sr,
                            SharpPeriods = srs,
                            NumberOfPeakPeriod = pr,
                            PeakPeriods = prs,
                            NumberOfFlatPeriod = fr,
                            FlatPeriods = frs,
                            NumberOfValleyPeriod = vr,
                            ValleyPeriods = vrs,
                        };
                        var result = client.Send(packet3);
                        if (result)
                        {
                            Logger.Info($"为电桩{sn}下发电价时间段成功！");
                        }
                        else
                        {
                            Logger.Info($"为电桩{sn}下发电价时间段失败！");
                        }
                        
                        Thread.Sleep(2500);
                    }

                    // 下发服务费时间段
                    var servicePeriodDefine = systemDbContext.ServiceDefines.Where(_ => _.StationId == stationId && _.IsValid).ToList();
                    if (servicePeriodDefine?.Count < 1)
                    {
                        Logger.Info($"还没有为电站{stationId}配置服务费时间段...");
                    }
                    else
                    {
                        var srdata = servicePeriodDefine.Where(_ => _.Catalog == "0")?.Select(_ => _.StartTime.Hours).ToList();
                        var sr = (byte)srdata.Count;
                        var srs = new byte[sr * 2];
                        for (int i = 0; i < sr; i++)
                        {
                            srs[2 * i] = (byte)srdata[i];
                            srs[2 * i + 1] = (byte)(srdata[i] + 1);
                        }
                        var prdata = servicePeriodDefine.Where(_ => _.Catalog == "1")?.Select(_ => _.StartTime.Hours).ToList();
                        var pr = (byte)prdata.Count;
                        var prs = new byte[pr * 2];
                        for (int i = 0; i < pr; i++)
                        {
                            prs[2 * i] = (byte)prdata[i];
                            prs[2 * i + 1] = (byte)(prdata[i] + 1);
                        }
                        var frdata = servicePeriodDefine.Where(_ => _.Catalog == "2")?.Select(_ => _.StartTime.Hours).ToList();
                        var fr = (byte)frdata.Count;
                        var frs = new byte[fr * 2];
                        for (int i = 0; i < fr; i++)
                        {
                            frs[2 * i] = (byte)frdata[i];
                            frs[2 * i + 1] = (byte)(frdata[i] + 1);
                        }
                        var vrdata = servicePeriodDefine.Where(_ => _.Catalog == "3")?.Select(_ => _.StartTime.Hours).ToList();
                        var vr = (byte)vrdata.Count;
                        var vrs = new byte[vr * 2];
                        for (int i = 0; i < vr; i++)
                        {
                            frs[2 * i] = (byte)vrdata[i];
                            frs[2 * i + 1] = (byte)(vrdata[i] + 1);
                        }
                        var packet4 = new SetTimePeriodPacket()
                        {
                            SerialNumber = sn,
                            OperType = OperTypeEnum.SetTimePeriodOper,
                            PeriodType = 1,
                            NumberOfSharpPeriod = sr,
                            SharpPeriods = srs,
                            NumberOfPeakPeriod = pr,
                            PeakPeriods = prs,
                            NumberOfFlatPeriod = fr,
                            FlatPeriods = frs,
                            NumberOfValleyPeriod = vr,
                            ValleyPeriods = vrs,
                        };
                        var result = client.Send(packet4);
                        if (result)
                        {
                            Logger.Info($"为电桩{sn}下发服务费时间段成功！");
                        }
                        else
                        {
                            Logger.Info($"为电桩{sn}下发服务费时间段失败！");
                        }
                    }

                    Logger.Info($"充电桩{sn}下发配置成功！");
                }
                catch (Exception ex)
                {
                    Logger.Info("下发配置失败！");
                    Logger.Error(ex);
                }
            });
        }

        #endregion  ====Get操作====
    }
}
