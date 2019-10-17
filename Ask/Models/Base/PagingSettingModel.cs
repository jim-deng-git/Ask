using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Base
{
    public class PagingSettingModel
    {
        public long CardNo { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Types { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Menus { get; set; } // 注意：Menus 和 Types，如果文章 Menu 下有類別，則記錄類別，如果無類別，則記錄 MenuID

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

        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public IEnumerable<long> GetMenus()
        {
            if (string.IsNullOrWhiteSpace(Menus))
                return new long[] { };

            return Menus.Split(',').Select(t => long.Parse(t));
        }

        public IEnumerable<long> GetTypes()
        {
            if (string.IsNullOrWhiteSpace(Types))
                return new long[] { };

            return Types.Split(',').Select(t => long.Parse(t));
        }

        public ResourceImagesModels GetDefaultImage()
        {
            if (string.IsNullOrWhiteSpace(DefaultImg))
                return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(DefaultImg);
        }

        public IEnumerable<int> GetIssueSetting()
        {
            if (string.IsNullOrWhiteSpace(IssueSetting))
                return new int[] { };

            return IssueSetting.Split(',').Select(t => int.Parse(t));
        }
    }
}