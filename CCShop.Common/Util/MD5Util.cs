using System.Security.Cryptography;
using System.Text;

namespace CCShop.Common.Util
{
    public class MD5Util
    {
        private static string Encrypt = "QuWang";

        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="encryptStr">要加密的字符串</param>
        /// <param name="charset">字符编码</param>
        /// <returns></returns>
        public static string GetMD5(string encryptStr, Encoding charset)
        {
            string returnStr = string.Empty;
            //创建md5对象
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
            //加密前字节数组
            byte[] originalByte;
            //加密后字节数组
            byte[] encryptByte;

            if (charset == null) charset = Encoding.UTF8;
            originalByte = charset.GetBytes(encryptStr);
            encryptByte = m5.ComputeHash(originalByte);
            returnStr = System.BitConverter.ToString(encryptByte);
            returnStr = returnStr.Replace("-", "").ToUpper();
            return returnStr;
        }

        public static string MD5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashByte = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            foreach (byte item in hashByte)
                sb.Append(item.ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <returns>加密后的字串</returns>
        public static string MD5Encrypt(string strSource)
        {
            return MD5Encrypt(strSource + Encrypt, 16);
        }

        /// <summary>
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <param name="length">16或32值之一,其它则采用.net默认MD5加密算法</param>
        /// <returns>加密后的字串</returns>
        public static string MD5Encrypt(string strSource, int length)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(strSource);
            byte[] hashValue = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            switch (length)
            {
                case 16:
                    for (int i = 4; i < 12; i++)
                        sb.Append(hashValue[i].ToString("x2"));
                    break;
                case 32:
                    for (int i = 0; i < 16; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
                default:
                    for (int i = 0; i < hashValue.Length; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
            }
            return sb.ToString();
        }
    }
}
