using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitWorkListRequest
    {
        /// <summary>
        /// 第幾頁(一頁20筆)
        /// </summary>
        public int? Index { get; set; }
    }

    public class RecruitWorkListResult
    {
        /// <summary>
        /// 工作ID
        /// </summary>
        public long WorkID { get; set; }
        /// <summary>
        /// 應徵者ID
        /// </summary>
        public long ApplicationID { get; set; }
        /// <summary>
        /// 職缺ID
        /// </summary>
        public long RecruitID { get; set; }
        /// <summary>
        /// 工作類型ID
        /// </summary>
        public long RecruitJobID { get; set; }
        /// <summary>
        /// 職缺標題
        /// </summary>
        public string RecruitTitle { get; set; }
        /// <summary>
        /// 工作類型名稱
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 職缺類別
        /// </summary>
        public string Types { get; set; }
        /// <summary>
        /// 工作區間
        /// </summary>
        public string JobTime { get; set; }
        /// <summary>
        /// 工作日期
        /// </summary>
        public DateTime WorkDate { get; set; }
        /// <summary>
        /// 報到時間
        /// </summary>
        public DateTime CheckInTime { get; set; }
        /// <summary>
        /// 是否已報到
        /// </summary>
        public bool IsCheckIn { get; set; }
        /// <summary>
        /// 是否已簽退
        /// </summary>
        public bool IsCheckOut { get; set; }
    }

    public class RecruitWorkListViewModel
    {
        public long WorkID { get; set; }
        public long ApplicationID { get; set; }
        public DateTime CheckDate { get; set; }
        public long RecruitID { get; set; }
        public string RecruitTitle { get; set; }
        public long RecruitJobID { get; set; }
        public string JobName { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string CheckInStart { get; set; }
    }
}