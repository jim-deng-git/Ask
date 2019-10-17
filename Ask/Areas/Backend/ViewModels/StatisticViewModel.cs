using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class StatisticViewModel
    {
        #region 搜尋條件
        public DateTime SearchStartDate { get; set; }
        public DateTime SearchEndDate { get; set; }
        public WorkV3.Common.DateRange RangeType { get; set; } = Common.DateRange.DoubleWeeks;
        #endregion
        public bool IsShowCustomLableLine { get; set; }
        /// <summary>
        /// 統計明細內容
        /// </summary>
        //public IEnumerable<AnalysisDailyLogViewModel> LogDailyList { get; set; }
        public IEnumerable<AnalysisLogCustomLabelLineViewModel> LabelLineList { get; set; }
        public IEnumerable<Models.StatisticConditionModels> StatisticConditionList { get; set; }
    }
}