using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.Interface;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 動能取得商品唯一號查詢送出參數格式
    /// </summary>
    public class GetGoodsSerNoRequest : FlapsRequest, IFlapsApiSetRequestMethod
    {
        public List<string> GoodsCode { get; set; }
    }
}