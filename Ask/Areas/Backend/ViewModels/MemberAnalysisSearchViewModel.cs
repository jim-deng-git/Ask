using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberAnalysisSearchViewModel
    {
        #region 搜尋條件
        public string SearchStartDate { get; set; }
        public string SearchEndDate { get; set; }
        public bool? IsShowCustomLableLine { get; set; }
        #endregion
    }
}