using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class MemberResult:GetResult
    {
        /// <summary>
        /// 欄位設定清單
        /// </summary>
        public List<ColumnSet> ColumnSets { get; set; } 
    }
    public enum MemberResultCode
    {
        [Message("")]
        Success,
        [Message("Secret錯誤")]
        SecretError,
        [Message("Token錯誤")]
        TokenOvertime,
        [Message("Token錯誤")]
        TokenError, 
        [Message("驗證失敗")]
        AuthFail,
        [Message("欄位尚未設定")]
        ColumnNull,
        [Message("網站尚未建立")]
        SiteNull
    }
}