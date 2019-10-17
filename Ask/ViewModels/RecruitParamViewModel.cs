using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WorkV3.Common;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    public class RecruitParamViewModel
    {
        public Pagination Pager { get; set; }

        public long ID { get; set; }
        public long SiteID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? StartTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? EndTime { get; set; }

        [DisplayName("刊登")]
        public bool IsIssue { get; set; }

        public string Name { get; set; }

        public long OperatorID { get; set; }

        public long AgentID { get; set; }

        public string Img { get; set; }

        public string Email { get; set; }
        
        //轉型用
        public string IDstr { get; set; }
        public string OperatorIDstr { get; set; }
        public string AgentIDstr { get; set; }
    }

    public class RecruitParamSearch
    {
        public long SiteID { get; set; } = 0;
    }
}