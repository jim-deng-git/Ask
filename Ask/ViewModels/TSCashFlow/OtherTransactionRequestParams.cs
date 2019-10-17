using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class OtherTransactionRequestParams
    {
        public string order_no { get; set; }
        public string amt { get; set; }
        public string result_flag { get; set; } = "1";
    }
}