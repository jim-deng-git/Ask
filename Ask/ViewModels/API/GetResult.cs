using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 共用的回應 Model
    /// </summary>
    public class GetResult
    {
        /// <summary>
        /// 回應代碼
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 是否成功, true 成功, false 失敗
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// 回應的訊息
        /// </summary>
        public string Message { get; set; } = "";
    }
    public static class EnumExtensions
    {
        private static Dictionary<Enum, string> dic = new Dictionary<Enum, string>();
        public static string GetMessage(this Enum item)
        {
            string text;
            if (!dic.TryGetValue(item, out text))
            {
                System.Reflection.FieldInfo fieldInfo = item.GetType().GetField(item.ToString());
                object[] attrs = fieldInfo.GetCustomAttributes(typeof(MessageAttribute), true);
                if (attrs?.Length > 0)
                {
                    MessageAttribute attr = attrs[0] as MessageAttribute;
                    if (attr != null)
                        text = attr.Value;
                }

                if (string.IsNullOrWhiteSpace(text))
                    text = item.ToString();

                dic.Add(item, text);
            }

            return text;
        }
    }
    public sealed class MessageAttribute : Attribute
    {
        private readonly string value;

        public MessageAttribute(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return this.value; }
        }
    }
}