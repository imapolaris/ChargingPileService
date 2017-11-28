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
        Login =1,
        LoginResult,
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
}
