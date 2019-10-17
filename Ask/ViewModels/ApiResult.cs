using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 共用的回應 Model
    /// </summary>
    public class ApiResult<TEntity>
    {
        /// <summary>
        /// 回應代碼
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 是否成功, true 成功, false 失敗
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// 回應的訊息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 回傳內容
        /// </summary>
        public TEntity Content { get; set; }
    }

    public enum ResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("Menu尚未建立")]
        ModuleSNNull,
        [Message("Module SN 尚未建立")]
        MenuNull,
        [Message("需要登入")]
        NeedLogin,
        Exception
    }
}