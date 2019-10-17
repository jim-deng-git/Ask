using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class PwdResetRequest
    {
        /// <summary>
        /// 原密碼
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPwd { get; set; }
        /// <summary>
        /// 確認新密碼
        /// </summary>
        public string AgainNewPwd { get; set; }
    }

    public enum PwdResetResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("舊密碼輸入不正確")]
        OldPasswordWrong,
        [Message("二次密碼輸入不相符")]
        AgainPasswordWrong,
        [Message("密碼更新失敗")]
        Failed,
        Exception
    }
}