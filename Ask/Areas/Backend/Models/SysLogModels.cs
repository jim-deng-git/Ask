using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Golbal;

namespace WorkV3.Areas.Backend.Models
{

    public class SysLogModels
    {
        public long Id { get; set; }
        public long MemberID { get; set; }
        public string MemberName { get; set; }
        public string MemberLoginID { get; set; }
        public byte? MgrNo { get; set; }
        public string MgrNoName { get; set; }
        public byte? Actions { get; set; }
        public string ActionsName { get; set; }
        public long? SiteID { get; set; }
        public string SiteTitle { get; set; }
        public long? MenuID { get; set; }
        public string MenuTitle { get; set; }
        public long? SourceID { get; set; }
        public string PageTitle { get; set; }
        public string PageUrl { get; set; }
        public string ReMark { get; set; }
        public DateTime? AddTime { get; set; }
        public string IP { get; set; }
        public long IPNum { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SysLogSearchModel
    {
        public string KW { get; set; }
        public int[] Actions { get; set; }
        public string SDate { get; set; }
        public string EDate { get; set; }
        public long MemberId { get; set; } = 0;
    }
}