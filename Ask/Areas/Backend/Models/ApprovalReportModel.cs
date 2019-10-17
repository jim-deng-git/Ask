using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ApprovalReportModel
    {
        public long CaseID { get; set; }
        public string CaseNo { get; set; }
        public long SiteID { get; set; }
        public long ReportID { get; set; }
        public string ReportType { get; set; }
        public int Sort { get; set; }
    }
}