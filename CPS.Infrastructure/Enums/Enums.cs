using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Enums
{
    public enum ActionTypeEnum : byte
    {
        Startup = 0x01,
        Shutdown,
        GetChargingPileState,
        QueryChargingBilling,

        SetElecPrice,
        SetServicePrice,
        SetElecPriceSingle,
        SetServicePriceSingle,

        SetPeriod,
    }

    public enum ResultTypeEnum : byte
    {
        Succeed = 0x01,
        Failed,
    }

    public enum ActionResultTypeEnum : byte
    {
        Succeed = 0x01,
        Failed,
        Timeout,
    }

    #region 【推送消息】

    public enum PlatformTypeEnum
    {
        All = 0x00,
        Android,
        IOS,
    }

    /// <summary>
    /// 富媒体消息类型
    /// </summary>
    public enum RichMediaTypeEnum
    {
        /// <summary>
        /// 模板
        /// </summary>
        Template,
        /// <summary>
        /// URL
        /// </summary>
        Url,
    }

    #endregion 【推送消息】
}
