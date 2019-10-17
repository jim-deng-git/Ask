using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitSalaryListRequest
    {
        /// <summary>
        /// 第幾頁(一頁20筆)
        /// </summary>
        public int? Index { get; set; }
        public DateTime Date { get; set; }
    }

    public class RecruitSalaryListResult
    {
        /// <summary>
        /// 累積時數
        /// </summary>
        public string TotalHours { get; set; }
        /// <summary>
        /// 累積薪資
        /// </summary>
        public string TotalSalary { get; set; }
        public List<SalaryList> SalaryList { get; set; }
    }

    public class SalaryList
    {
        /// <summary>
        /// 工作(報到)ID
        /// </summary>
        public long WorkID { get; set; }
        /// <summary>
        /// 職缺標題
        /// </summary>
        public string RecruitTitle { get; set; }
        /// <summary>
        /// 報到時間
        /// </summary>
        public DateTime CheckDate { get; set; }
        /// <summary>
        /// 實領薪資
        /// </summary>
        public string FinalSalary { get; set; }
        /// <summary>
        /// 領款人是否確認
        /// </summary>
        public bool IsPayeeCheck { get; set; }
        /// <summary>
        /// 薪資是否確認
        /// </summary>
        public bool IsSalaryCheck { get; set; }
    }
}