using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class MemberShipLoginLogModel
    {

        public long ID { get; set; }
        public long MemberShipID { get; set; }
        public string Browser { get; set; }
        public string UserAgent { get; set; }
        public string IP { get; set; }
        public long IPNum { get; set; }
        public DateTime? AddTime { get; set; }

    }
}