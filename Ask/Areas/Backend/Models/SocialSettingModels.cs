using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class SocialSettingModels
    {

        public long SiteID { get; set; }
        public bool IsOpen { get; set; }
        public string SocialDefaultImage { get; set; }
        public bool IsHeaderOpenChannel { get; set; }
        public bool IsFooterOpenChannel { get; set; }
        public bool IsEDMOpenChannel { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}