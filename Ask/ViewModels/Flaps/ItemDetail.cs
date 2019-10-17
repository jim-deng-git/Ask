using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class ItemDetail
    {
        /// <summary>
        /// 商品唯一號 [陣列最大長度 100]
        /// </summary>
        public IEnumerable<string> SerNo { get; set; }
        /// <summary>
        /// 單品數量
        /// </summary>
        public IEnumerable<string> Qty { get; set; }
        /// <summary>
        /// 單品定價
        /// </summary>
        public IEnumerable<string> Price { get; set; }
        /// <summary>
        /// 原始未稅單品售價
        /// </summary>
        public IEnumerable<string> OriginalSalePrice { get; set; }
        /// <summary>
        /// 原始含稅單品售價
        /// </summary>
        public IEnumerable<string> OriginalTaxedSalePrice { get; set; }
        /// <summary>
        /// 原始未稅單品總金額
        /// </summary>
        public IEnumerable<string> OriginalTotal { get; set; }
        /// <summary>
        /// 原始含稅單品總金額
        /// </summary>
        public IEnumerable<string> OriginalTaxedTotal { get; set; }
        /// <summary>
        /// 折扣後未稅單品售價
        /// </summary>
        public IEnumerable<string> RealSalePrice { get; set; }
        /// <summary>
        /// 折扣後含稅單品售價
        /// </summary>
        public IEnumerable<string> RealTaxedSalePrice { get; set; }
        /// <summary>
        /// 折扣後未稅單品總金額
        /// </summary>
        public IEnumerable<string> RealTotal { get; set; }
        /// <summary>
        /// 折扣後含稅單品總金額
        /// </summary>
        public IEnumerable<string> RealTaxedTotal { get; set; }
    }
}