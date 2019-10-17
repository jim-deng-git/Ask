using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class MemberLoginResult:GetResult
    {
        /// <summary>
        /// 會員基本資料
        /// </summary>
        public MemberInfo MemberInfo { get; set; }
        /// <summary>
        /// 取得的 token 值, 若認證失敗為空白, token 有效期為 10 分鐘
        /// </summary>
        public string MemberToken { get; set; }
        /// <summary>
        /// MemberToken到期時間
        /// </summary>
        public string Expire { get; set; }
    }
    public enum MemberLoginResultCode
    {
        [Message("")]
        Success,
        [Message("帳號空白")]
        AccountNull,
        [Message("密碼空白")]
        PasswordNull,
        [Message("帳號或密碼錯誤")]
        LoginFail,
        [Message("Site SN 空白")]
        SiteNull
    }
}