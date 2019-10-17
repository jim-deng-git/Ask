using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitApplicationListRequest
    {
        /// <summary>
        /// 第幾頁(一頁20筆)
        /// </summary>
        public int? Index { get; set; }
        ///// <summary>
        ///// 篩選狀態
        ///// </summary>
        //public int CheckStatus { get; set; }
    }

    public class RecruitApplicationListResult
    {
        /// <summary>
        /// 職缺ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 職缺標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 職缺薪水
        /// </summary>
        public IEnumerable<string> Salary { get; set; }
        /// <summary>
        /// 職缺類別
        /// </summary>
        public string Types { get; set; }
        public List<RecruitJobList> RecruitJobList { get; set; }
    }

}