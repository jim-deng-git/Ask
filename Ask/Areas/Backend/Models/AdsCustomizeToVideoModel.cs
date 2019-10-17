using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 自訂廣告管理 點擊事件_影片
    /// </summary>
    public class AdsCustomizeToVideoModel
    {
        public long ID { get; set; }
        /// <summary>
        /// 自訂廣告管理 ID
        /// </summary>
        public long AdsCustomize_ID { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public string PlayMode { get; set; }
        public bool IsAuto { get; set; }
        public bool IsRepeat { get; set; }
        public bool ScreenshotIsCustom { get; set; }
        public long? Screenshot { get; set; }
        /// <summary>
        /// 是否為蓋台廣告
        /// </summary>
        public bool IsCover { get; set; }
        /// <summary>
        /// 蓋台廣告秒數
        /// </summary>
        public int? CoverSecond { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
    }
}