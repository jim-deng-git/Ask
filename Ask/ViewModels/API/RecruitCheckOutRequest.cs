using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitCheckOutRequest
    {
        ///// <summary>
        ///// 應徵者ID
        ///// </summary>
        //public long ApplicationID { get; set; }
        ///// <summary>
        ///// 要簽退的日期 ex:2019-06-26
        ///// </summary>
        //public DateTime CheckOutDate { get; set; }
        /// <summary>
        /// 工作ID
        /// </summary>
        public long WorkID { get; set; }
        /// <summary>
        /// 簽退圖
        /// </summary>
        public string CheckOutPhoto { get; set; }
    }

    public enum RecruitCheckOutResultCode
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
        [Message("此會員與應徵會員不同")]
        DifferentMember,
        [Message("未報到過")]
        CheckInNull,
        [Message("已簽退")]
        HasCheckOut,
        [Message("簽退失敗")]
        CheckOutFailed,
        Exception
    }
}