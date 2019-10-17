using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace WorkV3.Common
{
    public static class BasicClassExt
    {
        #region 將字符串截斷為指定的長度
        /// <summary>
        /// 將字符串截斷為指定的長度
        /// </summary>
        /// <param name="str">需要截短的字符串</param>
        /// <param name="length">長度</param>
        /// <returns>截短後的字符串</returns>
        public static string Truncate(this String str, int length) {
            int len = str.Length;
            int i = 0;
            for (; i < length && i < len; ++i) {
                if ((int)(str[i]) > 0xFF)
                    --length;
            }

            bool isCut = false;
            if (length <= i) {
                length = i - 1;
                isCut = true;
            } else if (length > len)
                length = len;
            return str.Substring(0, length) + (isCut ? "..." : "");
        }
        #endregion

        /// <summary>
        /// 將字符串中的回車換行替換為 &lt;br /&gt;
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>替換後的字符串（若參數 str 為 null，返回 String.Empty）</returns>
        public static string ReplaceEnterToBr(this String str) {
            if (str == null || str.Trim() == String.Empty)
                return String.Empty;

            return str.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }

        /// <summary>
        /// 將字符串中的 &lt;br /&gt; 替換為回車換行
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>替換後的字符串</returns>
        public static string ReplaceBrToEnter(this String str) {
            if (str == null || str.Trim() == String.Empty)
                return String.Empty;

            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"<br\s*/?>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return reg.Replace(str, "\r\n");
        }

        /// <summary>
        /// 刪除字符串中的 HTML Tag
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>刪除 HTML Tag 後的字符串</returns>
        public static string TrimTags(this String str) {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"<[^>]*>|&[a-zA-Z\d#]+;");
            return reg.Replace(str, String.Empty);
        }

        /// <summary>
        /// 獲取字典指定鍵的值，若鍵不存在，則返回默認值
        /// </summary>
        /// <typeparam name="T">鍵的型別</typeparam>
        /// <typeparam name="K">值的型別</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">指定的鍵</param>
        /// <returns>值</returns>
        public static K GetValue<T, K>(this Dictionary<T, K> dictionary, T key) {
            K val;
            if (dictionary.TryGetValue(key, out val))
                return val;
            return default(K);
        }

        public static string ToString(this DateTime? date, string format) {
            if (date == null)
                return string.Empty;
            return ((DateTime)date).ToString(format);
        }

        /// <summary>
        /// 將字符串加密（DES）
        /// </summary>
        /// <param name="str">待加密的字符串</param>
        /// <returns>加密後的字符串（BASE64編碼）</returns>
        public static string Encrypt(this string str) {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = System.Text.Encoding.UTF8.GetBytes("K@dTE#h.");
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            ICryptoTransform desEncrypt = des.CreateEncryptor();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// 將加密後的字符串解密（DES）
        /// </summary>
        /// <param name="str">待解密的字符串（BASE64編碼）</param>
        /// <returns>解密後的字符串</returns>
        public static string Decrypt(this string str) {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = System.Text.Encoding.UTF8.GetBytes("K@dTE#h.");
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            ICryptoTransform desDecrypt = des.CreateDecryptor();
            byte[] buffer = Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length)).TrimEnd('\0');
        }

        public static string MD5(this string str) {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(str + ":C@w0rk");
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string result = string.Empty;
            for (int i = 0; i < md5data.Length - 1; i++) {
                result += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return result;
        }

        public static string Md5(this string str, string salt = ":C@w0rk")
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            byte[] myData = md5Hasher.ComputeHash(Encoding.Default.GetBytes($"{str}{salt}"));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < myData.Length; i++)
            {
                sBuilder.Append(myData[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string FullActionUrl(this System.Web.Mvc.UrlHelper url, string actionName, string controllerName, object routeValues = null) {
            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"].TrimEnd('/');
            return webUrl + url.Action(actionName, controllerName, routeValues);
        }
    }
}