using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ForgetPwdRequest
    {
        public string Account { get; set; }
        public string Email { get; set; }
    }

    public enum ForgetPwdResult
    {
        [Message("")]
        Success,
        [Message("帳號或E-Mail錯誤!")]
        AccountOrEmailErrorMessage,
        [Message("會員資料尚未完成電子郵件驗證，請先完成電子郵件驗證流程!")]
        NotVerifiedMessage
    }
}