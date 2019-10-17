using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.Interface;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 動能取得商品資訊送出參數格式
    /// </summary>
    public class GetGoodsInfoRequest: FlapsRequest, IFlapsApiSetRequestMethod
    {
        public int Records { get; set; } = 1;
        public string DateThreshold { get; set; }
    }
}