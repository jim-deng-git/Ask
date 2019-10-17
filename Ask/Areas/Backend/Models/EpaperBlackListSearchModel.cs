using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperBlackListSearchModel
    {
        public string keyword { get; set; }
        public string[] EpaperTypeSelect { get; set; }
        public bool? PauseSendStatus { get; set; }
        public int? FailureTimesStart { get; set; }
        public int? FailureTimesEnd { get; set; }
        public string SubscriberType { get; set; }
        /// <summary>
        /// 會員身份
        /// </summary>
        public string[] identitySelect { get; set; }
        /// <summary>
        /// 喜好
        /// </summary>
        public string[] favoritySelect { get; set; }
    }
}