using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 廣告統計
    /// </summary>
    public class AdsStatisticsLabelsModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public DateTime LabelDate { get; set; }
        public bool ShowStatus { get; set; }
        public string LabelColor { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public bool IsShowCustomLableLine { get; set; }
    }
}