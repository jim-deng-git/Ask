using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class EpaperTypeViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Intro { get; set; }
        public int SubscriberCount { get; set; }
        public IEnumerable<EpaperViewModel> EpaperList { get; set; }
    }
}