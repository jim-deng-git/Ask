using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 購物車購買項目 - 商家運費
    /// </summary>
    public class CartStoreShippingCodeViewModel
    {
        public string ID { get; set; }
        public string StoreID { get; set; }
        public string ShippingCode { get; set; }
        public string ShippingTitle { get; set; }
        public float Fee { get; set; }
    }
}