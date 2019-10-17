using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitCheckInRequest
    {
        ///// <summary>
        ///// 應徵者ID
        ///// </summary>
        //public long ApplicationID { get; set; }
        ///// <summary>
        ///// 要報到的日期
        ///// </summary>
        //public DateTime CheckInDate { get; set; }
        /// <summary>
        /// 工作ID
        /// </summary>
        public long WorkID { get; set; }
        /// <summary>
        /// 報到圖
        /// </summary>
        public string CheckInPhoto { get; set; }
    }

    public enum RecruitCheckInResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("查無工作名單資料")]
        WorkNull,
        [Message("查無應徵資料")]
        ApplicationNull,
        [Message("查無工作資料")]
        JobNull,
        [Message("此會員與應徵會員不同")]
        DifferentMember,
        [Message("非正取")]
        NotFormal,
        [Message("已報到過")]
        HasCheckIn,
        [Message("報到日期不在工作日期範圍內")]
        CheckInDateError,
        [Message("報到失敗")]
        CheckInFailed,
        Exception
    }
}