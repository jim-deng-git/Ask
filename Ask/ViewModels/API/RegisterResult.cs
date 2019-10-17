using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RegisterResult : GetResult
    {
        /// <summary>
        /// 會員ID
        /// </summary>
        public long MemberID { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string Captcha { get; set; }
    }

    public enum RegisterResultCode
    {
        [Message("成功")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("查無此會員")]
        NoMember,
        [Message("帳號重複")]
        DuplicateAccount,
        [Message("Email重複")]
        DuplicateEmail,
        [Message("未輸入帳號欄位")]
        AccountColNull,
        [Message("未輸入密碼欄位")]
        PasswordColNull,
        [Message("未輸入確認密碼欄位")]
        Password2ColNull,
        [Message("帳號密碼不得為null")]
        AccountOrPasswordIsNull,
        [Message("密碼與確認密碼不相符")]
        Password2Error,
        [Message("手機不得為null")]
        MobileNumIsNull,
        [Message("目前註冊設定非手機驗證")]
        NotMobileVerify,
        [Message("傳送驗證碼失敗")]
        SendMobileVerifyFail,
        [Message("驗證碼錯誤")]
        VerifyError,
        [Message("驗證碼已過期，請重新發送")]
        VerifyExpired,
        [Message("手機驗證已通過 或 註冊設定非手機驗證")]
        ReSendError,
        Exception
    }
}