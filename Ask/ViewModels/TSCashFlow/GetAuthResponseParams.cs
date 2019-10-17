using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class GetAuthResponseParams
    {
        /// <summary>
        /// 交易結果回應碼 
        /// </summary>
        public string ret_code { get; set; } = "1";
        /// <summary>
        /// 回傳訊息  
        /// </summary>
        public string ret_msg { get; set; }
        /// <summary>
        /// 付款網頁資訊  若「交易結果回應碼(ret_code)」為 00，則此欄位必有值，且需將持卡人瀏覽器  redirect 至此 hppUrl
        /// </summary>
        public string hpp_url { get; set; }
    }
}