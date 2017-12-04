using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public enum PacketTypeEnum : short
    {
        None,
        Login =0x01,
        LoginResult=0x02,
        HeartBeatClient=0x03,
        HeartBeatServer=0x04,
        Reboot=0x05,
        RebootResult=0x06,
        Confirm=0x07,
        Deny=0x08,
        /// <summary>
        /// 设置电价
        /// </summary>
        SetElecPrice=0x0B,
        /// <summary>
        /// 设置服务费
        /// </summary>
        SetServicePrice=0x0C,
        SetReportInterval=0x0D,
        SetTimePeriod=0x0E,
        ChangeSecretKey=0x0F,
        /// <summary>
        /// 查询充电桩当前电费
        /// </summary>
        GetElecPrice=0x15,
        GetElecPriceResult=0x16,
        /// <summary>
        /// 查询充电桩服务费
        /// </summary>
        GetServicePrice=0x17,
        GetServicePriceResult=0x18,
        GetReportInterval=0x19,
        GetReportIntervalResult=0x1A,
        GetTimePeriod=0x1B,
        GetTimePeriodResult=0x1C,
        GetSecretKey=0x1D,
        GetSecretKeyResult=0x1E,
        GetSoftwareVer=0x21,
        GetSoftwareVerResult=0x22,
        /// <summary>
        /// 设置枪口互联互通二维码
        /// </summary>
        SetQRcode=0x23,
        GetQRcode = 0x24,
        GetQRcodeResult = 0x25,
        /// <summary>
        /// 上报充电桩状态
        /// </summary>
        ChargingPileState=0x29,
        /// <summary>
        /// 查询充电桩状态
        /// </summary>
        GetChargingPileState=0x2A,
        /// <summary>
        /// 启停充电（无卡）
        /// </summary>
        SetCharging=0x2B,
        /// <summary>
        /// 启停结果（无卡）
        /// </summary>
        GetChargingResult=0x2C,
        /// <summary>
        /// 充电实时数据
        /// </summary>
        RealDataOfCharging=0x2D,
        /// <summary>
        /// 充电交易记录
        /// </summary>
        RecordOfCharging=0x2E,
        /// <summary>
        /// 确认交易记录
        /// </summary>
        ConfirmOfCharging=0x2F,
        /// <summary>
        /// 查询充电消费记录
        /// </summary>
        GetRecordOfCharging=0x30,

        /// <summary>
        /// 故障信息
        /// </summary>
        FaultMessage=0x46,
        FaultMessageReply=0x47,
        /// <summary>
        /// 告警、保护信息
        /// </summary>
        WarnMessage=0x48,
        WarnMessageReply=0x49,

        /// <summary>
        /// 充电桩请求有卡启动充电
        /// </summary>
        StartChargingWithCard=0x5A,
        StartChargingWithCardReply=0x5B,
        StartChargingWithCardResult=0x5C,
        StartChargingWithCardResultReply=0x5D,
        RealDataOfChargingWithCard=0x5E,

        StopChargingWithCard=0x5F,
        StopChargingWithCardReply=0x60,

        SetBlacklist=0x61,
        SetBlacklistResult=0x62,
        SetWhitelist=0x63,
        SetWhitelistResult=0x64,

        GetBlacklist=0x65,
        GetBlacklistResult=0x66,
        GetWhitelist=0x67,
        GetWhitelistResult=0x68,

        UpgradeSoftware=0x6E,
        UpgradeSfotwareReply=0x6F,

        DownloadFinished=0x70,
        DownloadFinishedReply=0x71,

        InUpgradeState=0x72,
        InUpgradeStateReply=0x73,

        UpgradeResult=0x74,
        UpgradeResultReply=0x75,
    }

    public enum LoginResultEnum : short
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        Succeed,
        /// <summary>
        /// 设备不存在
        /// </summary>
        NotExists,
        /// <summary>
        /// 已经登录
        /// </summary>
        HasLogined,
        /// <summary>
        /// 密钥失效
        /// </summary>
        SecretKeyFailed,
        /// <summary>
        /// 其他错误
        /// </summary>
        Others,
    }

    public enum TerminalTypeEnum : byte
    {
        Client=0x01,
        Server
    }

    public enum OperTypeEnum : int
    {
        Reboot=0x01,
        SetElecPrice=0x02,
    }
}
