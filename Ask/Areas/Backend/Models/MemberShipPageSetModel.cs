using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberShipPageSetModel
    {
        public long SiteID { get; set; }
        public string PageSN { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool IsOpen { get; set; }
        public bool IsSubMenu { get; set; }
        public bool IsDesktopCard { get; set; }
        public int Sort { get; set; }
    }
}