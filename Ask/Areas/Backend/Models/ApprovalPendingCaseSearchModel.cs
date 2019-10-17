using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ApprovalPendingCaseSearchModel
    {
        public string KeyWord { get; set; }
        public DateTime? CreateTimeStart { get; set; }
        public DateTime? CreateTimeEnd { get; set; }
        public long Creator { get; set; }
        public long ApprovalMember { get; set; }
    }
}