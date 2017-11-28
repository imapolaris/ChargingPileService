using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    public static class DateHelper
    {
        public static int ConvertToTimeStampX(this DateTime now)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            int timeStamp = (int)(now - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }

        public static DateTime ConvertToDateX(int timeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return startTime.AddSeconds(timeStamp);
        }

        public static string ToStringX(this DateTime now)
        {
            return now.ToString("yyyyMMdd HHmmss");
        }
    }
}
