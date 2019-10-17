using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 列表共用的回應 Model
    /// </summary>
    public class ApiResultWithPage<TEntity>
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
        /// 總頁數
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 目前頁數(可為null)
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 回傳內容
        /// </summary>
        public TEntity Content { get; set; }
    }
}