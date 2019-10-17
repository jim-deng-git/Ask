using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class SiteSeoSettingModels
    {

        public long SiteID { get; set; }
        public bool Robot { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public string Keywords { get; set; }
        public string GA { get; set; }
        public string GTM { get; set; }
        public string Baidu { get; set; }
        public string Alexa { get; set; }
        public string GoogleSearch { get; set; }
        public string BaiduMA { get; set; }
        public string Bing { get; set; }
        public bool IsExtraCode { get; set; }
        public string ExtraCode { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}