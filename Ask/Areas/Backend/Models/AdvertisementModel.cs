using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 廣告(區)主檔
    /// </summary>
    public class AdvertisementModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        /// <summary>
        /// 廣告主 ID
        /// </summary>
        public long? Advertisers_ID { get; set; }
        /// <summary>
        /// 廣告區名稱
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 使用於電腦版
        /// </summary>
        public bool IsUseForComputer { get; set; }
        /// <summary>
        /// 電腦版尺寸_長
        /// </summary>
        public int? ComputerHeight { get; set; }
        /// <summary>
        /// 電腦版尺寸_寬
        /// </summary>
        public int? ComputerWidth { get; set; }
        /// <summary>
        /// 使用於手機板
        /// </summary>
        public bool IsUseForPhone { get; set; }
        /// <summary>
        /// 手機板尺寸_長
        /// </summary>
        public int? PhoneHeight { get; set; }
        /// <summary>
        /// 手機板尺寸_寬
        /// </summary>
        public int? PhoneWidth { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 電腦版第三方嵌入碼
        /// </summary>
        public string ComputerThirdPartyEmbedCode { get; set; }
        /// <summary>
        /// 手機版第三方嵌入碼
        /// </summary>
        public string PhoneThirdPartyEmbedCode { get; set; }
        /// <summary>
        /// 第三方廣告連結
        /// </summary>
        public string ThirdPartyLink { get; set; }
        /// <summary>
        /// 廣告顯示方式 (不使用)
        /// </summary>
        public DisplayWays DisplayWay { get; set; } = DisplayWays.Random;

        public enum DisplayWays
        {
            Random = 1, 
            Carousel
        }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public long MenuID { get; set; }
    }
}