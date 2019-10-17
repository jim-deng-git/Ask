using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 自訂廣告管理 點擊事件_連結
    /// </summary>
    public class AdsCustomizeToLinkModel
    {
        public long ID { get; set; }
        /// <summary>
        /// 自訂廣告管理 ID
        /// </summary>
        public long AdsCustomize_ID { get; set; }
        /// <summary>
        /// 網址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 是否另開
        /// </summary>
        public bool IsOpenNew { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}