using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ReimburseToItemsModel
    {
        public long ID { get; set; }
        public long ReimburseID { get; set; }
        /// <summary>
        /// 對應預支單
        /// </summary>
        public long EstimateID { get; set; }
        public long TypeID { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 銷售額
        /// </summary>
        public int TaxExcluded { get; set; }
        /// <summary>
        /// 稅金
        /// </summary>
        public int Tax { get; set; }
        /// <summary>
        /// 含稅總計
        /// </summary>
        public int TaxIncluded { get; set; }
        /// <summary>
        /// 單據類型
        /// </summary>
        public int DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        /// <summary>
        /// 現金支付
        /// </summary>
        public int CashPay { get; set; }
        /// <summary>
        /// 公司支付
        /// </summary>
        public int CompanyPay { get; set; }
        public int Sort { get; set; }

        public string TypeName { get; set; }
        public string TypeColor { get; set; }
    }
}