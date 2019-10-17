using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 購物車購買項目列表
    /// </summary>
    public class ShopDetailViewModel
    {
        public EshopProductModel EshopProduct { get; set; }
        public IEnumerable<EshopProductSpecModel> Specs { get; set; }
        public IEnumerable<StoreModels> Stores { get; set; }
        public IEnumerable<EshopProductCategoryModel> Categories { get; set; }
        public IEnumerable<EshopProductModel> StoreEshopProducts { get; set; }
    }
}