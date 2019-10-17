using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class GetAuthPostReturnParams
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
        /// 訂單編號
        /// </summary>
        public string order_no { get; set; }
        public string auth_id_resp { get; set; }
        public string carrierId2 { get; set; }
        public string rrn { get; set; }
    }
}