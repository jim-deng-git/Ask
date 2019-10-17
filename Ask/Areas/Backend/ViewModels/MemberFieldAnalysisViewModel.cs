using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberFieldAnalysisViewModel
    {
        #region 搜尋條件
        public DateTime SearchStartDate { get; set; }
        public DateTime SearchEndDate { get; set; }
        public WorkV3.Common.DateRange RangeType { get; set; } = Common.DateRange.DoubleWeeks;
        #endregion
        /// <summary>
        /// 欄位記錄
        /// </summary>
        public IEnumerable<MemberFieldViewModel> FieldStatisticList { get; set; }
    }
}