using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberAnalysisLogViewModel
    {
        #region 搜尋條件
        public DateTime SearchStartDate { get; set; }
        public DateTime SearchEndDate { get; set; }
        public WorkV3.Common.DateRange RangeType { get; set; } = Common.DateRange.DoubleWeeks;
        #endregion
        public bool IsShowCustomLableLine { get; set; }
        public IEnumerable<MemberAnalysisLogCustomLabelLineViewModel> LabelLineList { get; set; }
        /// <summary>
        /// 每日點閱記錄
        /// </summary>
        public IEnumerable<MemberAnalysisDailyLogViewModel> LogDailyList { get; set; }
        /// <summary>
        /// 每月點閱記錄
        /// </summary>
        public IEnumerable<MemberAnalysisMonthlyLogViewModel> LogMonthList { get; set; }
    }
}