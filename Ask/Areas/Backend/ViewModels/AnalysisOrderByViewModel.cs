using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisOrderByViewModel
    {
        #region 排序條件
        public SortColumn SortColumn { get; set; }
        public SortDesc SortDesc { get; set; }
        #endregion
    }

    public enum SortColumn
    {
        TotalViewCount = 0,
        TotalUserCount = 1,
        TotalMemberViewCount =2,
        AddTime =3,
        StaySeconds = 4
    }

    public enum SortDesc
    {
        Asc = 0,
        Desc = 1
    }
}