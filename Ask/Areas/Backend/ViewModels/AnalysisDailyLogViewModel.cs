using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisDailyLogViewModel
    {
        /// <summary>
        /// 單日日期
        /// </summary>
        public string LogDate { get; set; }
        /// <summary>
        /// 總點閱率
        /// </summary>
        public long TotalViewCount { get; set; }
        /// <summary>
        /// 全站會員瀏覽
        /// </summary>
        public long TotalMemberViewCount { get; set; }
        /// <summary>
        /// 全站瀏覽人次
        /// </summary>
        public long TotalUserCount { get; set; }
    }
}