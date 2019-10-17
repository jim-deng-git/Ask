using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class CustomArticleModel {
        public long MenuID { get; set; }
        public long ArticleID { get; set; }
    }

    public class CustomArticleListItemModel : CustomArticleModel {
        public string Title { get; set; }        
        public string Creator { get; set; }             
        public string SiteSN { get; set; }
        public string PageSN { get; set; }
    }
}