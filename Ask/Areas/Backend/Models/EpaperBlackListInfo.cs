using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 電子報黑名單資訊
    /// </summary>
    public class EpaperBlackListInfo
    {
        public long ID { get; set; }
        public long EpaperSubscriber_ID { get; set; }
        public string ApplyUnit { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonGender { get; set; }
        public string ContactPersonMobile { get; set; }
        public string ContactPersonEmail { get; set; }
        public string BlackReason { get; set; }
        public DateTime? ViolationTime { get; set; }
        public string ViolationDesc { get; set; }
    }
}