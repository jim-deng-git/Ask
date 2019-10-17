using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class MemberCollectionViewModel
    {
        public long No { get; set; }
        public long MenuID { get; set; }
        public string Title { get; set; }
        public string SN { get; set; }
        public string LinkUrl { get; set; }
        public string ImgSrc { get; set; }
        public string ImgAlt { get; set; }
        public string Summary { get; set; }
    }
}