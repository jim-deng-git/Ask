using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisPageStatLogViewModel
    {
        #region 搜尋條件
        public DateTime SearchStartDate { get; set; }
        public DateTime SearchEndDate { get; set; }
        public WorkV3.Common.DateRange RangeType { get; set; } = Common.DateRange.DoubleWeeks;
        #endregion
        public string PageID { get; set; }
        public string Title { get; set; }
        public string MenuTitle { get; set; }
        /// <summary>
        /// 總瀏覽量
        /// </summary>
        public long TotalViewCount { get; set; }
        /// <summary>
        /// 瀏覽人次
        /// </summary>
        public long TotalMemberViewCount { get; set; }
        /// <summary>
        /// 會員瀏覽
        /// </summary>
        public long TotalUserCount { get; set; }
        public bool IsShowCustomLableLine { get; set; }
        public IEnumerable<AnalysisLogCustomLabelLineViewModel> LabelLineList { get; set; }
        public AnalysisOrderByViewModel OrderBy { get; set; }
        public IEnumerable<AnalysisPageViewSessionViewModel> SessionList { get; set; }
        public IEnumerable<AnalysisPageViewSessionViewModel> UserList { get; set; }
        public IEnumerable<AnalysisPageViewSessionViewModel> MemberList { get; set; }
        /// <summary>
        /// 每日點閱記錄
        /// </summary>
        public IEnumerable<AnalysisDailyLogViewModel> LogDailyList { get; set; }
    }
}