using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class PagesView_LogModel
    {

        public long ID { get; set; }
        public long PagesNo { get; set; }
        public string ReferrerUrl { get; set; }
        public string ReferrerUrlTitle { get; set; }
        public string ReferrerUrlPageNo { get; set; }
        public string Lang { get; set; } = "zh-Hant";
        public int Ver { get; set; } = 1;
        public long SiteID { get; set; }
        public long? MemberID { get; set; }
        public string Browser { get; set; }
        public string UserAgent { get; set; }
        public string IP { get; set; }
        public string SessionID { get; set; }
        public DateTime? AddTime { get; set; }
        public long IPNum { get; set; }  
        
    }
}