using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class StorePlanModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string PlanName { get; set; }
        public string CoverImg { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 簽約金 0:免簽約金、1:須簽約金
        /// </summary>
        public int SigningBonusMode { get; set; }
        public string SigningCurrency { get; set; }
        public double SigningMoney { get; set; }

        /// <summary>
        /// 交易抽成 0:免抽成、1:需抽成
        /// </summary>
        public int CommissionMode { get; set; }
        public double CommissionPercent { get; set; }

        /// <summary>
        /// 業績獎金 0:無、1:有
        /// </summary>
        public int BonusMode { get; set; }
        public string BonusCurrency { get; set; }
        public double BonusMoney { get; set; }
        public int Period { get; set; }
        public bool IsIssue { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int Sort { get; set; }
    }
}