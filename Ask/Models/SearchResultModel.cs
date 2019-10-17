using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class SearchResultModel
    {
        public long MenuID { get; set; }
        public long CardNo { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }        
        public string Image { get; set; }
    }

    public class SearchMenuResultModel {
        public string Title { get; set; }
        public string Url { get; set; }        
        public string Image { get; set; }
    }

    public class SearchMenuModel
    {
        public long ID { get; set; }
        public long ParentID { get; set; }        
        public string Module { get; set; }
        public string Title { get; set; }
        public string UploadUrl { get; set; }
        public string Icon { get; set; }       
    }
}