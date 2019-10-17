using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class PageCommentListViewModel
    {
        public int TotalRowCount { get; set; }
        public int PageIndex { get; set; }
        public IEnumerable<Models.PageCommentsModels> CommentList { get; set; }
        
    }
}