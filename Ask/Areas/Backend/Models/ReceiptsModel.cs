using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ReceiptsModel
    {
        public long ID { get; set; }
        public long ProjectID { get; set; }
        public long SiteID { get; set; }
        /// <summary>
        /// 申請日期
        /// </summary>
        public DateTime ApplyDate { get; set; }
        /// <summary>
        /// 申請人
        /// </summary>
        public string ApplyName { get; set; }
        /// <summary>
        /// 建檔人
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// 發票類型
        /// </summary>
        public int ReceiptType { get; set; }
        /// <summary>
        /// 寄送方式
        /// </summary>
        public int ShippingType { get; set; }
        /// <summary>
        /// 附加回郵
        /// </summary>
        public bool AddReturn { get; set; }
        /// <summary>
        /// 寄送備註
        /// </summary>
        public string ShippingNote { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// 客戶資訊-公司名稱
        /// </summary>
        public string CustCompany { get; set; }
        /// <summary>
        /// 客戶資訊-聯絡人
        /// </summary>
        public string CustName { get; set; }
        /// <summary>
        /// 客戶資訊-聯絡電話
        /// </summary>
        public string CustPhone { get; set; }
        /// <summary>
        /// 客戶資訊-聯絡信箱
        /// </summary>
        public string CustEmail { get; set; }
        public string CustRegions { get; set; }
        public string CustAddress { get; set; }
        /// <summary>
        /// 買受人
        /// </summary>
        public string Buyer { get; set; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxIdNumber { get; set; }
        /// <summary>
        /// 銷售額總計
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
        /// 是否議價
        /// </summary>
        public bool Bargain { get; set; }
        /// <summary>
        /// 議價後總計
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

        [NotMapped]
        public string ProjectNumber { get; set; }
        [NotMapped]
        public string ProjectTitle { get; set; }
        [NotMapped]
        public string ModifierName { get; set; }
        [NotMapped]
        public DateTime DateEnd { get; set; }
    }

    public class ReceiptsSearchModel
    {
        public long? ProjectID { get; set; }
        public long SiteID { get; set; }
        public string Key { get; set; }
        public DateTime? ApplyDateStart { get; set; }
        public DateTime? ApplyDateEnd { get; set; }
        public int? TotalStart { get; set; }
        public int? TotalEnd { get; set; }
        public int[] ReceiptType { get; set; }
        public int[] Status { get; set; }
    }

    public enum ReceiptType
    {
        [Description("二聯式發票")]
        Duplicate = 1,
        [Description("三聯式發票")]
        Triplicate,
        [Description("免開發票")]
        None
    }

    public enum ShippingType
    {
        /// <summary>
        /// 親送
        /// </summary>
        [Description("親送")]
        Personally = 1,

        /// <summary>
        /// 快遞
        /// </summary>
        [Description("快遞")]
        ExpressDelivery,

        /// <summary>
        /// 掛號
        /// </summary>
        [Description("掛號")]
        Registered,

        /// <summary>
        /// 電子發票
        /// </summary>
        [Description("電子發票")]
        Electronic
    }

    public enum PaymentType
    {
        /// <summary>
        /// 現金
        /// </summary>
        [Description("現金")]
        Cash = 1,

        /// <summary>
        /// 匯款
        /// </summary>
        [Description("匯款")]
        Transfer,

        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Check
    }

    public enum ReceiptStatus
    {
        [Description("草稿中")]
        Draft,
        [Description("確認送出")]
        Checked,
        [Description("棄單")]
        Discard
    }
}