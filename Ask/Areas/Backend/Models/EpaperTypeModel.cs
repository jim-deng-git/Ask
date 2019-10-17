using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperTypeModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
        public string Picture { get; set; }
        public bool Status { get; set; }
        public string OpenObject { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public long MenuID { get; set; }
    }
}