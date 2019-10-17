using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitPayeeCheckRequest
    {
        public long WorkID { get; set; }
    }

    public enum RecruitPayeeCheckCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("查無工作")]
        WorkNull,
        Exception
    }
}