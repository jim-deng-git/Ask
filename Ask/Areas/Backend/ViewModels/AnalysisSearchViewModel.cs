using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisSearchViewModel
    {
        #region 搜尋條件
        public string SearchStartDate { get; set; }
        public string Keywords { get; set; }
        public string SearchEndDate { get; set; }
        public string SelectMembers { get; set; }
        public WorkV3.Common.DateRange RangeType { get; set; } = Common.DateRange.DoubleWeeks;
        public bool? IsShowCustomLableLine { get; set; }
        #endregion
    }
}