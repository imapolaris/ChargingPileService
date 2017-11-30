using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.PushService
{
    public enum PlatformTypeEnum
    {
        All,
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
}
