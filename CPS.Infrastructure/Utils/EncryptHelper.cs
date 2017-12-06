using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    public class EncryptHelper
    {
        private static readonly string IV = "r7BXXKkLb8qrSNn05n0qiA==";
        private static readonly string KEY = "tiihtNczf5v6AKRyjwEUhQ==";

        private static readonly byte[] KeyArray;
        private static readonly byte[] IvArray;

        static EncryptHelper()
        {
            KeyArray = Convert.FromBase64String(KEY);
            IvArray = Convert.FromBase64String(IV);
        }

        #region ====AES-128-CBC加密====
        public static byte[] Encrypt(byte[] buffer)
        {
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = KeyArray;
            rm.IV = IvArray;
            rm.Mode = CipherMode.CBC;
            rm.Padding = PaddingMode.PKCS7;

            var cTransform = rm.CreateEncryptor();
            return cTransform.TransformFinalBlock(buffer, 0, buffer.Length);
        }

        public static byte[] Decrypt(byte[] buffer)
        {
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = KeyArray;
            rm.IV = IvArray;
            rm.Mode = CipherMode.CBC;
            rm.Padding = PaddingMode.PKCS7;

            var cTransform = rm.CreateDecryptor();
            return cTransform.TransformFinalBlock(buffer, 0, buffer.Length);
        }

        public static string Encrypt(string encryptStr)
        {
            byte[] encryptArray = EncodeHelper.GetBytes(encryptStr);
            byte[] resultArray = Encrypt(encryptArray);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string decryptStr)
        {
            byte[] decryptArray = Convert.FromBase64String(decryptStr);
            byte[] resultArray = Decrypt(decryptArray);
            return EncodeHelper.GetString(resultArray);
        }
        #endregion ====AES-128-CBC加密====
    }
}
