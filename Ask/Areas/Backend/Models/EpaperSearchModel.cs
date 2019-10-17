using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSearchModel
    {
        public string keyword { get; set; }
        public DateTime? Publish_Date_Start { get; set; }
        public DateTime? Publish_Date_End { get; set; }
        public string[] EpaperStyle { get; set; }
        public string[] EpaperTypeSelect { get; set; }
        //public string[] AdvertisementType { get; set; }
    }
}