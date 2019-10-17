using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class MemberEventViewModel
    {
        public int Type { get; set; } = 1;
        public string No { get; set; }
        public string FormNo { get; set; }
        public string Token { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string images { get; set; }
        public string location { get; set; }
        public string googlemap { get; set; }
        public string organizer { get; set; }
        public string summary { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string cancellink { get; set; }
        public string color { get; set; }
        public string className { get; set; }
    }
}