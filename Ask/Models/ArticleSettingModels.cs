using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WorkV3.Models
{
    public class ArticleSettingModels
    {
        public long MenuID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Types { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PagingMode { get; set; }
        public int PageSize { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string IssueSetting { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SortMode { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SortField { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DefaultImg { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReplyStatus { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReplyTitle { get; set; }

        public int ReplyPageSize { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReplyFBID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReplyFBAccounts { get; set; }

        public bool ExtendReadOpen { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ExtendReadTitle { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ExtendReadMenus { get; set; }

        public byte ExtendReadMode { get; set; }
        public int ExtendReadRowCount { get; set; }

        public byte ReadMode { get; set; }
        public string ReadModeSet { get; set; }

        #region 延伸閱讀區 2
        public bool ExtendReadOpen2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ExtendReadTitle2 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ExtendReadMenus2 { get; set; }

        public byte ExtendReadMode2 { get; set; }
        public int ExtendReadRowCount2 { get; set; }
        #endregion
        public bool ADOpen { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ADTitle { get; set; }

        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public IEnumerable<long> GetTypes() {
            if (string.IsNullOrWhiteSpace(Types))
                return new long[] { };

            return Types.Split(',').Select(t => long.Parse(t));
        }

        public IEnumerable<int> GetIssueSetting() {
            if (string.IsNullOrWhiteSpace(IssueSetting))
                return new int[] { };

            return IssueSetting.Split(',').Select(t => int.Parse(t));
        }

        public IEnumerable<long> GetExtendReadMenus() {
            if (string.IsNullOrWhiteSpace(ExtendReadMenus))
                return new long[] { };

            return ExtendReadMenus.Split(',').Select(t => long.Parse(t));
        }
        public IEnumerable<long> GetExtendReadMenus2()
        {
            if (string.IsNullOrWhiteSpace(ExtendReadMenus2))
                return new long[] { };

            return ExtendReadMenus2.Split(',').Select(t => long.Parse(t));
        }

        public ResourceImagesModels GetDefaultImage() {
            if (string.IsNullOrWhiteSpace(DefaultImg))
                return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(DefaultImg);
        }
    }
}