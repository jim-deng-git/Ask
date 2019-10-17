using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberAnalysisMonthlyLogViewModel
    {
        /// <summary>
        /// 月份
        /// </summary>
        public string LogMonth { get; set; }
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