using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberShipSettingModel
    {
        public long ID { get; set; }
        public long MemberShipID { get; set; }
        public string Type { get; set; }
        public long CategoryID { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}