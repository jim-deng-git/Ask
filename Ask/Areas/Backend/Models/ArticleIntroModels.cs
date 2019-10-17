using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ArticleIntroModels
    {
        public long ID { get; set; }
        public long MenuID { get; set; }
        //public long CardNo { get; set; }
        public string Title { get; set; }
        public DateTime? IssueDate { get; set; }
        public string RemarkText { get; set; }
        public string Icon { get; set; }
        public bool IsIssue { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

    }
}