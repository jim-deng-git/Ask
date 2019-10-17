using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class CheckGoodsStockResponse
    {
        public string Message { get; set; }
        public IEnumerable<GoodsStock> GoodsStock { get; set; }
    }
}