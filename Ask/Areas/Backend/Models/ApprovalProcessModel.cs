using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ApprovalProcessModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Title { get; set; }
        public int TotalLevel { get; set; }
        public int UsingCount { get; set; }
        public bool IsIssue { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}