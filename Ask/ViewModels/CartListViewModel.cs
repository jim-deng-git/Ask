using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 購物車購買項目列表
    /// </summary>
    public class CartListViewModel
    {
        /// <summary>
        /// 購買清單中所有商家列表
        /// </summary>
        public List<CartStoreViewModel> Stores { get; set; } = new List<CartStoreViewModel>();
        /// <summary>
        /// 購物車中所有商品總價
        /// </summary>
        public float TotalPrice { get; set; }
        /// <summary>
        /// 目前會員已有的點數
        /// </summary>
        public int Point { get; set; }
    }
}