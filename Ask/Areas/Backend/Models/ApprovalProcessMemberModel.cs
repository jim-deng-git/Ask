using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ApprovalProcessMemberModel
    {
        public long ProcessID { get; set; }
        public int ProcessLevel { get; set; }
        public long Member { get; set; }
    }
}