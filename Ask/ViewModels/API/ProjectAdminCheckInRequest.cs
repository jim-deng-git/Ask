using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ProjectAdminCheckInRequest
    {
        /// <summary>
        /// 專案管理員ID
        /// </summary>
        public long ProjectAdminID { get; set; }
        /// <summary>
        /// 職缺ID
        /// </summary>
        public long RecruitID { get; set; }
        /// <summary>
        /// 要報到的日期
        /// </summary>
        public DateTime CheckInDate { get; set; }
    }

    public enum ProjectAdminCheckInResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("查無應徵資料")]
        ApplicationNull,
        [Message("已報到過")]
        HasCheckIn,
        [Message("報到失敗")]
        CheckInFailed,
        Exception
    }
}