using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitSetClothesPhotoRequest
    {
        /// <summary>
        /// 應徵ID
        /// </summary>
        public long ApplicationId { get; set; }
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string[] Photos { get; set; }
    }

    public enum SetClothesPhotoResultCode
    {
        [Message("")]
        Success,

        [Message("網站尚未建立")]
        SiteNull,

        [Message("需要登入")]
        NeedLogin,

        [Message("查無應徵資訊")]
        ApplicationNull,

        [Message("會員與應徵會員不同")]
        NotSameMember,

        Exception
    }
}