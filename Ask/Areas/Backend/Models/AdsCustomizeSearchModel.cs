using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class AdsCustomizeSearchModel
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 計價方式篩選
        /// </summary>
        public string[] FilterIssueSetType { get; set; }
        /// <summary>
        /// 刊登期間-開始
        /// </summary>
        public DateTime? IssueTimeStart { get; set; }
        /// <summary>
        /// 刊登期間-結束
        /// </summary>
        public DateTime? IssueTimeEnd { get; set; }
        /// <summary>
        /// 刊登金額-小
        /// </summary>
        public int? CostMin { get; set; }
        /// <summary>
        /// 刊登金額-大
        /// </summary>
        public int? CostMax { get; set; }
        /// <summary>
        /// 刊登金額種類篩選
        /// </summary>
        public string FilterCost { get; set; }
        /// <summary>
        /// 點擊次數-小
        /// </summary>
        public int? ClickCountMin { get; set; }
        /// <summary>
        /// 點擊次數-大
        /// </summary>
        public int? ClickCountMax { get; set; }
        /// <summary>
        /// 瀏覽次數-小
        /// </summary>
        public int? BrowseCountMin { get; set; }
        /// <summary>
        /// 瀏覽次數-大
        /// </summary>
        public int? BrowseCountMax { get; set; }
    }
}