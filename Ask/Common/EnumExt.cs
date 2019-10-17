using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute {
        public string Description { get; set; }

        public DescriptionAttribute(string description) {
            Description = description;
        }
    }

    public static class EnumExt {
        private static Dictionary<Enum, string> dic = new Dictionary<Enum, string>();
        public static string Description(this Enum item) {
            string text;
            if(!dic.TryGetValue(item, out text)) {
                System.Reflection.FieldInfo fieldInfo = item.GetType().GetField(item.ToString());
                object[] attrs = fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                if(attrs?.Length > 0) {
                    System.ComponentModel.DescriptionAttribute attr = attrs[0] as System.ComponentModel.DescriptionAttribute;
                    if (attr != null)
                        text = attr.Description;
                }

                if (string.IsNullOrWhiteSpace(text))
                    text = item.ToString();

                dic.Add(item, text);
            }

            return text;
        }        
    }    
}