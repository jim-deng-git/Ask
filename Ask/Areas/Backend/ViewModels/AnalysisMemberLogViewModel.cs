﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisMemberLogViewModel
    {
        #region 搜尋條件
        public DateTime SearchStartDate { get; set; }
        public DateTime SearchEndDate { get; set; }
        public WorkV3.Common.DateRange RangeType { get; set; } = Common.DateRange.DoubleWeeks;
        public string SelectMembers { get; set; }
        public string MemberID { get; set; }
        #endregion
        public string MemberName { get; set; }
        public string MemberAccount { get; set; }
        public string MemberEmail { get; set; }
        public string MemberPhoto { get; set; }
        /// <summary>
        /// 總點閱率
        /// </summary>
        public long TotalViewCount { get; set; }
        public bool IsShowCustomLableLine { get; set; }
        /// <summary>
        /// 每日點閱記錄
        /// </summary>
        public IEnumerable<AnalysisDailyLogViewModel> LogDailyList { get; set; }
        public IEnumerable<AnalysisPageViewSessionViewModel> LogSessionList { get; set; }
        public IEnumerable<AnalysisLogCustomLabelLineViewModel> LabelLineList { get; set; }
        public AnalysisOrderByViewModel OrderBy { get; set; }
    }
}