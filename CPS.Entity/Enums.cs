using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    public enum ChargingPileStateEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,
        /// <summary>
        /// 空闲
        /// </summary>
        Unoccupied,
        /// <summary>
        /// 使用中
        /// </summary>
        Using,
        /// <summary>
        /// 已预约
        /// </summary>
        Subscribed,
        /// <summary>
        /// 无法使用（需备注原因）
        /// </summary>
        Unusable,
        /// <summary>
        /// 其他
        /// </summary>
        Other,
    }

    public enum PayWayEnum : short
    {
        /// <summary>
        /// 全部支持
        /// </summary>
        All = 0x00,
        /// <summary>
        /// 支付宝
        /// </summary>
        Alipay = 0x01,
        /// <summary>
        /// 微信
        /// </summary>
        WxPay = 0x02,
        /// <summary>
        /// 线上支付
        /// </summary>
        AliAndWxPay = 0x03,
        /// <summary>
        /// 现金
        /// </summary>
        InCash = 0x04,
        /// <summary>
        /// 线上、线下支付
        /// </summary>
        AliWxPayAndInCash,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 0x08,
    }

    public enum UserTypeEnum
    {
        /// <summary>
        /// 集团用户
        /// </summary>
        GroupUser,
        /// <summary>
        /// 个人用户
        /// </summary>
        PersonalUser,
    }

    public enum CharingPileTypeEnum
    {
        /// <summary>
        /// 直流
        /// </summary>
        DC=0x01,
        /// <summary>
        /// 交流
        /// </summary>
        AC=0x02,
        /// <summary>
        /// 交直流一体
        /// </summary>
        DCAC=0x03,
    }
}
