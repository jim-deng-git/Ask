using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.Interface;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 動能取得轉入訂單資料/發票資訊送出參數格式
    /// </summary>
    public class DoSubmitOrderRequest : FlapsRequest, IFlapsApiSetRequestMethod
    {
        /// <summary>
        /// 識別碼 [不可為空]
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 收銀機代號 [不可為空]
        /// </summary>
        public string CashRegisterCode { get; set; }
        /// <summary>
        /// 店櫃代號 [不可為空]
        /// </summary>
        public string SalePointCode { get; set; }
        /// <summary>
        /// 銷售人員代號 [不可為空]
        /// </summary>
        public string UserIDCode { get; set; }
        /// <summary>
        /// 發票日期、銷貨單日期 [不可為空]
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 會員唯一碼
        /// </summary>
        public string MemberSerNo { get; set; }
        /// <summary>
        /// 使用紅利點數
        /// </summary>
        public int UseBonus { get; set; }
        /// <summary>
        /// 獲得紅利點數
        /// </summary>
        public int TotalBonus { get; set; }
        /// <summary>
        /// 禮券編號
        /// </summary>
        public string CouponNo { get; set; }
        /// <summary>
        /// 禮券金額
        /// </summary>
        public int UseCouponTotal { get; set; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string InvoiceNo { get; set; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UniqueNo { get; set; }
        /// <summary>
        /// 統編買受人
        /// </summary>
        public string UniqueNoTitle { get; set; }
        /// <summary>
        /// 發票格式
        /// </summary>
        public int Format { get; set; }
        /// <summary>
        /// 稅額
        /// </summary>
        public int Tax { get; set; }
        /// <summary>
        /// 未稅銷售總額
        /// </summary>
        public int NonTaxedTotal { get; set; }
        /// <summary>
        /// 含稅銷售總額
        /// </summary>
        public int TaxedTotal { get; set; }
        /// <summary>
        /// 收貨人
        /// </summary>
        public string Receiver { get; set; }
        /// <summary>
        /// 收貨人地址
        /// </summary>
        public string ReceiverAddress { get; set; }
        /// <summary>
        /// 收貨人聯絡電話
        /// </summary>
        public string ReceiverPhone { get; set; }
        /// <summary>
        /// 客戶唯一碼
        /// </summary>
        public string CustomerSerNo { get; set; }
        /// <summary>
        /// 含稅運費
        /// </summary>
        public int TransFee { get; set; }
        /// <summary>
        /// 付款方式代號
        /// </summary>
        public string PayWayCode { get; set; }
        /// <summary>
        /// 付款方式卡號
        /// </summary>
        public string PayCardNo { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 發票
        /// </summary>
        public int InvoiceType { get; set; }
        /// <summary>
        /// 載具號碼
        /// </summary>
        public string InvoiceCarrierId { get; set; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string InvoiceNPOBAN { get; set; }
        /// <summary>
        /// 發票聯絡人
        /// </summary>
        public string InvoiceContactPerson { get; set; }
        /// <summary>
        /// 發票連絡電話
        /// </summary>
        public string InvoiceContactTel { get; set; }
        /// <summary>
        /// 發票寄送地址
        /// </summary>
        public string InvoiceDeliveryAddress { get; set; }
        /// <summary>
        /// 發票聯絡人 EMAIL
        /// </summary>
        public string InvoiceContactEMail { get; set; }
        /// <summary>
        /// 訂單商品資訊 [最大長度為 100 筆]
        /// </summary>
        public IEnumerable<ItemsWrapper> Datas { get; set; }
    }

    public enum InvoiceType
    {
        Personal = 0,
        Company,
        MemberCarrier,
        MobileBarcode,
        CompanyCarrier,
        Donate
    }

    public enum InvoiceFormat
    {
        Format31 = 31,
        Format35 = 35
    }
}