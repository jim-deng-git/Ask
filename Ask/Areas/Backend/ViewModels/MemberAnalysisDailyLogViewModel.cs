using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberAnalysisDailyLogViewModel
    {
        /// <summary>
        /// 單日日期
        /// </summary>
        public string LogDate { get; set; }
        /// <summary>
        /// 登入計數
        /// </summary>
        public long LoginCount { get; set; }
        /// <summary>
        /// 會員計數
        /// </summary>
        public long MemberCount { get; set; }
    }
}