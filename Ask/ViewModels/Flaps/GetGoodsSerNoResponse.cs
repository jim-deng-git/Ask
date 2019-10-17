using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class GetGoodsSerNoResponse
    {
        public IEnumerable<GoodsData> Goods { get; set; }
        public string Message { get; set; }
    }
}