using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class OtherTransactionRequest : TSRequest
    {
        public OtherTransactionRequestParams Params { get; set; } = new OtherTransactionRequestParams();
    }
}