using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class GoodsStock
    {
        public string WarehouseCode { get; set; }
        public string GoodsSerNo { get; set; }
        public string GoodsCode { get; set; }
        public int Qty { get; set; }
    }
}