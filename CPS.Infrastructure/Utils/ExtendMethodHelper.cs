using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    public static class StringExtend
    {
        public static string TrimEnd0(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return s.TrimEnd(new char[] { '\0' });
            }
            return null;
        }
    }
}
