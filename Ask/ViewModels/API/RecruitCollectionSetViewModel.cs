using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitCollectionSetRequest
    {
        /// <summary>
        /// 職缺ID
        /// </summary>
        public long RecruitID { get; set; }
    }

    public enum RecruitCollectionSetResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("查無職缺")]
        RecruitsNull,
        Exception
    }
}