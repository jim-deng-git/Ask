using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class PageCommentViewModel
    {
        public string SiteID { get; set; }
        public string PageSN { get; set; }
        public CommentType ReplyType { get; set; }
        public int RowCount { get; set; }
        public string FBID { get; set; }
        public string ReplyTitle { get; set; }
    }
}