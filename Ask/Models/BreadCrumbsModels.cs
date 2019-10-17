using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class BreadCrumbsModels
    {
        public string PagesNo { get; set; }
        public string PagesTitle { get; set; }
        public string ParentID { get; set; }
        public string ID { get; set; }
        public string SiteID { get; set; }
        public string Title { get; set; }
        public string DEP_PATH { get; set; }
        public string DEP_SN { get; set; }
        public string DEP_LEVEL { get; set; }

    }
}