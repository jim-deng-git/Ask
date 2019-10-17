using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 購物車購買項目 - 商家運費
    /// </summary>
    public class CartShippingViewModel
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string OrderShippingID { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ShippingID { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ShippingCode { get; set; }
        /// <summary>
        /// 運費
        /// </summary>
        public float Fee { get; set; }
    }
}