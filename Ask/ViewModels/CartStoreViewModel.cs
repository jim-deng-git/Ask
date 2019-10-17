using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 購物車購買項目 - 商家模型
    /// </summary>
    public class CartStoreViewModel
    {
        /// <summary>
        /// 商家運費項目ID
        /// </summary>
        public string OrderShippingID { get; set; }
        /// <summary>
        /// 商家ID
        /// </summary>
        public string StoreID { get; set; }
        /// <summary>
        /// 商家 Name
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 選購商品中屬於此商家的品項
        /// </summary>
        public List<CartEshopProductViewModel> EshopProducts { get; set; }
        /// <summary>
        /// 選購商品中屬於此商家的運送方式
        /// </summary>
        public List<CartStoreShippingCodeViewModel> ShippingCodes { get; set; }
        /// <summary>
        /// 選購商品中屬於此商家的使用者運費選項
        /// </summary>
        public CartShippingViewModel EshopShipping { get; set; }
        /// <summary>
        /// 商家 Logo 圖片路徑
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 該商家中所有商品價格
        /// </summary>
        public float TotalPrice { get; set; }
        /// <summary>
        /// 所有此商家的商品中，最晚訂購的時間
        /// </summary>
        public string LatestOrderTime { get; set; }
    }
}