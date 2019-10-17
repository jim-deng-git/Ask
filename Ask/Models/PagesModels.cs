using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class PagesModels
    {

        public long No { get; set; }
        public string Lang { get; set; } = "zh-Hant";
        public int Ver { get; set; } = 1;
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        public string SN { get; set; }
        public string Title { get; set; }
        public string MetaDescriptions { get; set; }
        public string MetaImage { get; set; }
        public string MetaKeywords { get; set; }
        public byte ShowStatus { get; set; } = 1;
        public byte PublishStatus { get; set; } = 1;
        public DateTime PublishTime { get; set; } = DateTime.Today;
        public DateTime StartTime { get; set; } = DateTime.Today;
        public DateTime? EndTime { get; set; }
        public DateTime? ShowTime { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }       
    }
}