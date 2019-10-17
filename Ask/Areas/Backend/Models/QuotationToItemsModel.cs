using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class QuotationToItemsModel
    {
        public long ID { get; set; }
        public long QuotationID { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int UnitPrice { get; set; }
        public int? People { get; set; }
        public int? Hours { get; set; }
        public int? Session { get; set; }
        public int? Discount { get; set; }
        public int Total { get; set; }
        public int Sort { get; set; }

        #region 實際成本分析
        /// <summary>
        /// 預估成本
        /// </summary>
        public int EstimateCost { get; set; }
        /// <summary>
        /// 預估利潤
        /// </summary>
        public int EstimateProfit { get; set; }
        /// <summary>
        /// 預估毛利率
        /// </summary>
        public int EstimateGrossMargin { get; set; }
        /// <summary>
        /// 實際成本
        /// </summary>
        public int ActualCost { get; set; }
        /// <summary>
        /// 實際利潤
        /// </summary>
        public int ActualProfit { get; set; }
        /// <summary>
        /// 實際毛利率
        /// </summary>
        public int ActualGrossMargin { get; set; }
        #endregion
    }

    public enum QuotationItemType
    {
        Human = 1,
        Service
    }
}