using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class CardsModels
    {
        public long No { get; set; }
        public string Lang { get; set; } = "zh-Hant";
        public int Ver { get; set; }
        public long? ZoneNo { get; set; }
        public string Title { get; set; }
        public string SN { get; set; }
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

        //廣告
        public long SiteID { get; set; }
        public long PageNo { get; set; }
        public long MenuID { get; set; }
        public long AreaSetID { get; set; }
        public long AdvertisementMenuID { get; set; }
        public string GroupPosition { get; set; }
        public double TempSort { get; set; }

    }
}