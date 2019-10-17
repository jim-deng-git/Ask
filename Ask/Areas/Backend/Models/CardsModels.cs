using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Areas.Backend.Models
{
    public class CardsModels
    {
        public long No { get; set; }
        public string Lang { get; set; } = "zh-Hant";
        public int Ver { get; set; } = 1;
        public long? ZoneNo { get; set; }
        public string Title { get; set; }
        public string SN { get; set; }

        [AllowHtml]
        public string Descriptions { get; set; }

        public string CardsType { get; set; }
        public string ViewAction { get; set; }
        public int StylesID { get; set; } = 1;

        public int Color { get; set; }
        public byte Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? PublishTime { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public long SiteID { get; set; }
    }
}