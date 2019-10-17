using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 要求 token 後的結果 Model
    /// </summary>
    public class TokenResult: GetResult
    {
        /// <summary>
        /// 取得的 token 值, 若認證失敗為空白, token 有效期為 10 分鐘
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Token到期時間
        /// </summary>
        public string Expire { get; set; }
    }
    public enum TokenResultCode
    {
        [Message("")]
        Success,
        [Message("Secret錯誤")]
        SecretError,
        [Message("驗證失敗")]
        AuthFail 
    }
}