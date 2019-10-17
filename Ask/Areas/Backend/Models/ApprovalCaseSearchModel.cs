using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ApprovalCaseSearchModel
    {
        public string KeyWord { get; set; }
        public string CaseStatus { get; set; }
        public string CaseProcess { get; set; }
        public DateTime? CreateTimeStart { get; set; }
        public DateTime? CreateTimeEnd { get; set; }
        public long Creator { get; set; }
    }
}