using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.SchedulerSystem.Common
{
    public enum JobTypeEnum
    {
        [Display(Name ="预约任务")]
        Subscribe = 0x00,
    }
}
