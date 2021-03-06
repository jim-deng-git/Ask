﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ReimbursesModel
    {
        public long ID { get; set; }
        public long ProjectID { get; set; }
        public long SiteID { get; set; }
        /// <summary>
        /// 申請月份
        /// </summary>
        public DateTime ApplyMonth { get; set; }
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
        /// 銀行代碼
        /// </summary>
        public string BankID { get; set; }
        /// <summary>
        /// 銀行帳號
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 銀行帳戶名稱
        /// </summary>
        public string BankAccountName { get; set; }
        /// <summary>
        /// 撥款方式 1:轉帳 2:現金 3:匯款 4:支票
        /// </summary>
        public int Transfer { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 連絡電話
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// 連絡信箱
        /// </summary>
        public string ContactEmail { get; set; }
        /// <summary>
        /// 已申請預支總計
        /// </summary>
        public int EstimateTotal { get; set; }
        /// <summary>
        /// 現金支付總額
        /// </summary>
        public int CashPayTotal { get; set; }
        /// <summary>
        /// 應繳(請)款金額
        /// </summary>
        public int AccountsPayable { get; set; }
        /// <summary>
        /// 公司支付總額
        /// </summary>
        public int CompanyPayTotal { get; set; }
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
    }

    public class ReimburseSearchModel
    {
        public long? ProjectID { get; set; }
        public long SiteID { get; set; }
        public string Key { get; set; }
        public DateTime? ApplyDateStart { get; set; }
        public DateTime? ApplyDateEnd { get; set; }
        public DateTime? ApplyMonthStart { get; set; }
        public DateTime? ApplyMonthEnd { get; set; }
        //public int? TotalStart { get; set; }
        //public int? TotalEnd { get; set; }
        public int[] Status { get; set; }
    }

    public enum ReimburseStatus
    {
        [Description("草稿中")]
        Draft,
        [Description("確認送出")]
        Checked,
        [Description("棄單")]
        Discard
    }
}