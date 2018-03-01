using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Enums
{
    /// <summary>
    /// 客户类型
    /// </summary>
    public enum CustomerTypeEnum
    {
        [Display(Description = "未知")]
        Unknown,
        [Display(Description = "个人客户")]
        Personal,
        [Display(Description ="集团客户")]
        Group,
    }
}
