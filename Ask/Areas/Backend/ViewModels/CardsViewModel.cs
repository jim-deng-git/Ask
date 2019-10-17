using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class CardsViewModel
    {
        public long No { get; set; }
        public int Ver { get; set; }
        public long? ZoneNo { get; set; }
        public string Title { get; set; }
        public string SN { get; set; }
        public string Descriptions { get; set; }
        public string CardsType { get; set; }
        public string ViewAction { get; set; }
        public int Color { get; set; }
        public byte Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? PublishTime { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public string TypeTitle { get; set; }
        public string TypeIcon { get; set; }
        public int iFrameH { get; set; }
        public int iFrameW { get; set; }
        public string TypeEditURLAction { get; set; }

        public long MenuID { get; set; }
    }
}