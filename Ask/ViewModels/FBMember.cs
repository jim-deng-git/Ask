using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class FBMember
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string cover { get; set; }
        public string birthday { get; set; }
        public picture picture { get; set; }
    }
    public class picture
    {
        public data data { get; set; }
    }
    public class data
    {
        public string height { get; set; }
        public bool is_silhouette { get; set; }
        public string url { get; set; }
        public string width { get; set; }
    }
}