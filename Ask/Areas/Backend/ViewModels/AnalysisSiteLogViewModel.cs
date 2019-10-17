using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisSiteLogViewModel
    {
        #region 搜尋條件
        public string Keywords { get; set; }
        public DateTime SearchStartDate { get; set; }
        public DateTime SearchEndDate { get; set; }
        public string SelectMembers { get; set; }
        public WorkV3.Common.DateRange RangeType { get; set; } = Common.DateRange.DoubleWeeks;
        #endregion
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
        public bool IsShowCustomLableLine { get; set; }
        /// <summary>
        /// 每日點閱記錄
        /// </summary>
        public IEnumerable<AnalysisDailyLogViewModel> LogDailyList { get; set; }
        public IEnumerable<AnalysisPageLogViewModel> LogPageList { get; set; }
        public IEnumerable<AnalysisLogCustomLabelLineViewModel> LabelLineList { get; set; }
        public AnalysisOrderByViewModel OrderBy { get; set; }
    }
}