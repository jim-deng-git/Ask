using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class PageCommentsModels
    {
        public long ID { get; set; }
        public string CommentID { get; set; }
        public string PageSN { get; set; }
        public long? ParentID { get; set; }
        public string Title { get; set; }
        public string PosterName { get; set; }
        public DateTime PostDate { get; set; }
        public string PostDateDiff { get; set; }
        public bool ShowStatus { get; set; } = true;
        public string MemberType { get; set; }
        public long? MemberShipID { get; set; }
        public string CommentContent { get; set; }
        public string IP { get; set; }
        public string IPNum { get; set; }
        public long GoodCount { get; set; } = 0;
        public long? Modifier { get; set; }
        public bool IsMemberModify { get; set; }
        public DateTime ModifyTime { get; set; } = DateTime.Now;
        public List<PageCommentsModels> ChildCommentList { get; set; }
        public bool IsAddedGood { get; set; } = false;
        public string Photo { get; set; }
    }
}