using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ApprovalCaseReviewModel
    {
        public long CaseID { get; set; }
        public string CaseNo { get; set; }
        public long ApprovalMember { get; set; }
        public long ApprovalAgent { get; set; }
        public DateTime ApprovalDate { get; set; }
        public long Process { get; set; }
        public int ProcessLevel { get; set; }
        public string Intro { get; set; }
        public bool IsApproval { get; set; }
    }
}