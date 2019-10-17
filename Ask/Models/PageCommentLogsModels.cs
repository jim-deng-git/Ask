using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class PageCommentLogsModels
    {
        public long ID { get; set; }
        public long CommentID { get; set; }
        public long? MemberShipID { get; set; }
        public int LogType { get; set; }
        public string Browser { get; set; }
        public string UserAgent { get; set; }
        public string IP { get; set; }
        public string IPNum { get; set; }
        public DateTime AddTime { get; set; }
    }
}