using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    public static class EncodeHelper
    {
        public static string GetString(byte[] bytes)
        {
            return GetString(bytes, Encoding.UTF8);
        }

        public static string GetString(byte[] bytes, int startIndex, int len)
        {
            return GetString(bytes, startIndex, len, Encoding.UTF8);
        }

        public static byte[] GetBytes(string content)
        {
            return GetBytes(content, Encoding.UTF8);
        }

        public static string GetString(byte[] bytes, Encoding encoding)
        {
            if (bytes == null)
                return null;
            if (bytes.Length == 0)
                return "";
            return encoding.GetString(bytes);
        }

        public static string GetString(byte[] bytes, int startIndex, int len, Encoding encoding)
        {
            if (bytes == null)
                return null;
            if (bytes.Length == 0)
                return "";
            return encoding.GetString(bytes, startIndex, len);
        }

        public static byte[] GetBytes(string content, Encoding encoding)
        {
            if (content == null)
                return null;
            return encoding.GetBytes(content);
        }
    }
}
