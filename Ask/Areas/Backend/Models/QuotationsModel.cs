using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class QuotationsModel
    {
        public long ID { get; set; }
        /// <summary>
        /// 對應專案
        /// </summary>
        public long ProjectID { get; set; }
        public long SiteID { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// 報價日期
        /// </summary>
        public DateTime QuoteDate { get; set; }
        /// <summary>
        /// 申請日期
        /// </summary>
        public DateTime ApplyDate { get; set; }
        /// <summary>
        /// 申請人
        /// </summary>
        public string ApplyName { get; set; }
        /// <summary>
        /// 申請人公司電話
        /// </summary>
        public string ApplyCompanyPhone { get; set; }
        /// <summary>
        /// 申請人連絡電話
        /// </summary>
        public string ApplyPhone { get; set; }
        /// <summary>
        /// 申請人信箱
        /// </summary>
        public string ApplyEmail { get; set; }
        /// <summary>
        /// 客戶資訊-公司名稱
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 客戶資訊-聯絡人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 客戶資訊-聯絡電話
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// 是否議價
        /// </summary>
        public bool Bargain { get; set; }
        /// <summary>
        /// 未稅總計
        /// </summary>
        public int TaxExcluded { get; set; }
        /// <summary>
        /// 含稅總計
        /// </summary>
        public int TaxIncluded { get; set; }
        /// <summary>
        /// 稅金
        /// </summary>
        public int Tax { get; set; }
        /// <summary>
        /// 議價後未稅總計
        /// </summary>
        public int BargainTaxExcluded { get; set; }
        /// <summary>
        /// 議價後含稅總計
        /// </summary>
        public int BargainTaxIncluded { get; set; }
        /// <summary>
        /// 議價後稅金
        /// </summary>
        public int BargainTax { get; set; }

        public int Status { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        #region 實際成本分析
        /// <summary>
        /// 預估總成本
        /// </summary>
        public int EstimateCostTotal { get; set; }
        /// <summary>
        /// 預估總利潤
        /// </summary>
        public int EstimateProfitTotal { get; set; }
        /// <summary>
        /// 預估毛利率
        /// </summary>
        public int EstimateGrossMarginTotal { get; set; }
        /// <summary>
        /// 實際總成本
        /// </summary>
        public int ActualCostTotal { get; set; }
        /// <summary>
        /// 實際總利潤
        /// </summary>
        public int ActualProfitTotal { get; set; }
        /// <summary>
        /// 實際毛利率
        /// </summary>
        public int ActualGrossMarginTotal { get; set; }
        #endregion

        [NotMapped]
        public string ProjectNumber { get; set; }
        [NotMapped]
        public string ProjectTitle { get; set; }
        [NotMapped]
        public string ModifierName { get; set; }
    }

    public class QuotationSearchModel
    {
        public long? ProjectID { get; set; }
        public long SiteID { get; set; }
        public string Key { get; set; }
        public DateTime? ApplyDateStart { get; set; }
        public DateTime? ApplyDateEnd { get; set; }
        public DateTime? QuoteDateStart { get; set; }
        public DateTime? QuoteDateEnd { get; set; }
        public int? TaxIncludedStart { get; set; }
        public int? TaxIncludedEnd { get; set; }
        public int[] Status { get; set; }
    }

    public enum QuotationStatus
    {
        [Description("草稿中")]
        Draft,
        [Description("確認送出")]
        Checked,
        [Description("棄單")]
        Discard
    }
}