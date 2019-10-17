using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 購物車購買項目 - 商家模型
    /// </summary>
    public class CartEshopProductViewModel
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string EshopProductID { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string EshopProductName { get; set; }
        /// <summary>
        /// 規格ID
        /// </summary>
        public string SpecID { get; set; }
        /// <summary>
        /// 商品規格
        /// </summary>
        public string Spec { get; set; }
        /// <summary>
        /// 價格
        /// </summary>
        public float SinglePrice { get; set; }
        /// <summary>
        /// 單項商品小計
        /// </summary>
        public float TotalPrice { get; set; }
        /// <summary>
        /// 購買商品數
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 庫存數
        /// </summary>
        public int StockQuantity { get; set; }
        /// <summary>
        /// 商品圖片路徑
        /// </summary>
        public string Image { get; set; }
        public DateTime OrderTime { get; set; }
    }
}