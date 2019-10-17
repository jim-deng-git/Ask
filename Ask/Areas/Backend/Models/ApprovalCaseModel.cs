using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ApprovalCaseModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string CaseNo { get; set; }
        public string Title { get; set; }
        public long Creator { get; set; }
        public long Agent { get; set; }
        public DateTime CreateTime { get; set; }
        public int CaseStatus { get; set; }
        public string Intro { get; set; }
        public long Process { get; set; }
        public int ProcessLevel { get; set; }
        public bool IsUrgent { get; set; }
    }

    public enum CaseStatus
    {
        草稿 = 0,
        審核中,
        已通過,
        駁回,
        棄案
    }
}