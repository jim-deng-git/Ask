using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class GoodsInfo
    {
        public string SerNo { get; set; }
        public string StyleCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string SizeCode { get; set; }
        public string Sizename { get; set; }
        public string SellYear { get; set; }
        public string SellSeason { get; set; }
        public string LargeCategoryCode { get; set; }
        public string LargeCategoryName { get; set; }
        public string MiddleCategoryCode { get; set; }
        public string MiddleCategoryName { get; set; }
        public string SmallCategoryCode { get; set; }
        public string SmallCategoryName { get; set; }
        public string Barcode { get; set; }
        public decimal TaxedStdPrice { get; set; }
        public decimal TaxedTagPrice { get; set; }
    }
}