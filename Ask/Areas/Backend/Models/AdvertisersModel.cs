using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 廣告主
    /// </summary>
    public class AdvertisersModel
    {
        [JsonConverter(typeof(FormatNumbersAsTextConverter))]
        public long ID { get; set; }
        public long SiteID { get; set; }
        /// <summary>
        /// 企業名稱
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string ContactPerson { get; set; }
        /// <summary>
        /// 姓別
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 職稱
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 網址
        /// </summary>
        public string WebSite { get; set; }
        /// <summary>
        /// 相關資訊
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否刊登
        /// </summary>
        public bool IsIssue { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public long MenuID { get; set; }
        /// <summary>
        /// 根據性別回傳先生或女士字串
        /// </summary>
        public string LadyAndGenernal
        {
            get
            {
                if (Gender == "F")
                    return "女士";
                else if (Gender == "M")
                    return "先生";

                return string.Empty;
            }
        }
    }

    internal sealed class FormatNumbersAsTextConverter : JsonConverter
    {
        public override bool CanRead => false;
        public override bool CanWrite => true;
        public override bool CanConvert(Type type) => type == typeof(int);

        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(
            JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}