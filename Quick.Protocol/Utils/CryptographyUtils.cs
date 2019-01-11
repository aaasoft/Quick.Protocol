using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Quick.Protocol.Utils
{
    public class CryptographyUtils
    {
        public static String ComputeMD5Hash(String data)
        {
            var buffer = ComputeMD5Hash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static byte[] ComputeMD5Hash(byte[] data)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(data);
        }

        public static byte[] DesEncrypt(byte[] data, byte[] password)
        {
            var des = DES.Create();
            var enc = des.CreateEncryptor(password, password);
            return enc.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] DesDecrypt(byte[] data, byte[] password)
        {
            var des = DES.Create();
            var dec = des.CreateDecryptor(password, password);
            return dec.TransformFinalBlock(data, 0, data.Length);
        }

        private static byte[] GetDesPassword(String password)
        {
            var pwdMd5 = ComputeMD5Hash(Encoding.UTF8.GetBytes(password));
            var pwdBuffer = pwdMd5.Take(8).ToArray();
            return pwdBuffer;
        }

        public static String DesEncrypt(String data, String password)
        {
            var dataBuffer = Encoding.UTF8.GetBytes(data);
            var pwdBuffer = GetDesPassword(password);
            return BitConverter.ToString(DesEncrypt(dataBuffer, pwdBuffer)).Replace("-", "");
        }

        public static String DesDecrypt(String data, String password)
        {
            byte[] dataBuffer = new byte[data.Length / 2];
            for (var i = 0; i < data.Length / 2; i++)
            {
                var hexString = data.Substring(i * 2, 2);
                dataBuffer[i] = byte.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
            }
            var pwdBuffer = GetDesPassword(password);
            return Encoding.UTF8.GetString(DesDecrypt(dataBuffer, pwdBuffer));
        }
    }
}
