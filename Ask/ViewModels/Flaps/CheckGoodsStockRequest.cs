using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.Interface;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 動能取得庫存查詢送出參數格式
    /// </summary>
    public class CheckGoodsStockRequest : FlapsRequest, IFlapsApiSetRequestMethod
    {
        public List<string> WarehouseCode { get; set; }
        public List<string> SerNo { get; set; }
    }
}