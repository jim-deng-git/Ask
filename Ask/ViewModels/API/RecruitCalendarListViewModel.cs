using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitCalendarListRequest
    {
        /// <summary>
        /// ex: 2019-06
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 應徵狀態(null:全部、0:待審核、1:正取、2:備取、3:保留、4:不通過、5:前台取消、6:後台取消)
        /// </summary>
        public int? CheckStatus { get; set; }
    }

    public class RecruitCalendarListResult
    {
        /// <summary>
        /// 應徵ID
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
        /// 應徵狀態
        /// </summary>
        public string CheckStatus { get; set; }
        /// <summary>
        /// 工作日(開始)
        /// </summary>
        public DateTime JobDateStart { get; set; }
        /// <summary>
        /// 工作日(結束)
        /// </summary>
        public DateTime JobDateEnd { get; set; }
        /// <summary>
        /// 工作日星期
        /// </summary>
        public string JobDateWeek { get; set; }
        /// <summary>
        /// 是否可以取消應徵
        /// </summary>
        public bool IsClientCancel { get; set; }
    }
}