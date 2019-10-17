using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class GetAuthPostReturn : TSRequest
    {
        public GetAuthPostReturnParams Params { get; set; } = new GetAuthPostReturnParams();
    }
}