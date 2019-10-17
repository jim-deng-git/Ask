using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 自訂廣告管理_帳務設定
    /// 刊登時間、種類、計價等
    /// </summary>
    public class AdsCustomizeAccountSet
    {
        public long ID { get; set; }
        /// <summary>
        /// 自訂廣告管理 ID
        /// </summary>
        public long AdsCustomize_ID { get; set; }
        /// <summary>
        /// 刊登時間起
        /// </summary>
        public DateTime? IssueStart { get; set; }
        /// <summary>
        /// 刊登時間迄
        /// </summary>
        public DateTime? IssueEnd { get; set; }
        /// <summary>
        /// 刊登種類
        /// </summary>
        public string IssueSetType { get; set; }
        /// <summary>
        /// 期間總額幣別
        /// </summary>
        public string CostOfPeriodCurrency { get; set; }
        /// <summary>
        /// 期間總額
        /// </summary>
        public long? CostOfPeriod { get; set; }
        /// <summary>
        /// 瀏覽計價幣別
        /// </summary>
        public string BillingByBrowseCurrency { get; set; }
        /// <summary>
        /// 瀏覽計價
        /// </summary>
        public int? BillingByBrowse { get; set; }
        /// <summary>
        /// 點擊計價幣別
        /// </summary>
        public string BillingByClickCurrency { get; set; }
        /// <summary>
        /// 點擊計價
        /// </summary>
        public int? BillingByClick { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 點擊數
        /// </summary>
        public long? Clicks { get; set; }
        /// <summary>
        /// 瀏覽人次
        /// </summary>
        public long? Visits { get; set; }
        /// <summary>
        /// 是否刊登
        /// </summary>
        public bool IsIssue { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public long? QueryClicks
        {
            get
            {
                return WorkV3.Areas.Backend.Models.DataAccess.AdvertisementDAO.QueryAdFlow(AdsCustomize_ID, UserEvent.Click, ID);
            }
        }
        public long? QueryVisits
        {
            get
            {
                return WorkV3.Areas.Backend.Models.DataAccess.AdvertisementDAO.QueryAdFlow(AdsCustomize_ID, UserEvent.Browsing, ID);
            }
        }
        /// <summary>
        /// 期間內所需費用
        /// </summary>
        public long GetFee
        {
            get
            {
                string type = IssueSetType.ToLower();
                long retValue = 0;
                switch(type)
                {
                    case "issuetime":
                        retValue = CostOfPeriod?? 0;
                        break;
                    case "free":
                    default:
                        retValue = 0;
                        break;
                    case "click":
                        retValue = GetClickFee + GetVisitFee;
                        break;
                }

                return retValue;
            }
        }
        /// <summary>
        /// 期間內點擊所產生的費用
        /// </summary>
        public long GetClickFee
        {
            get
            {
                return (long)QueryClicks * (BillingByClick ?? 0);
            }
        }
        /// <summary>
        /// 期間內瀏覽所產生的費用
        /// </summary>
        public long GetVisitFee
        {
            get
            {
                return (long)QueryVisits * (BillingByBrowse ?? 0);
            }
        }
        /// <summary>
        /// 刊登種類 vm
        /// </summary>
        public List<SelectListItem> IssueSetTypeVM
        {
            get
            {
                return new[]
                        {
                            new SelectListItem {  Text = GetClassDisplayAttrName<IssueSetType>(Models.IssueSetType.IssueTime),Value=Models.IssueSetType.IssueTime },
                            new SelectListItem {  Text = GetClassDisplayAttrName<IssueSetType>(Models.IssueSetType.Click),Value=Models.IssueSetType.Click },
                            new SelectListItem {  Text = GetClassDisplayAttrName<IssueSetType>(Models.IssueSetType.Free),Value=Models.IssueSetType.Free}
                        }.ToList();
            }
        }
        /// <summary>
        /// 幣別 vm
        /// </summary>
        public List<SelectListItem> CurrencyItemVM
        {
            get
            {
                return new[]
                        {
                            new SelectListItem {  Text = GetClassDisplayAttrName<CurrencyItem>(CurrencyItem.NTD),Value=CurrencyItem.NTD },
                            new SelectListItem {  Text = GetClassDisplayAttrName<CurrencyItem>(CurrencyItem.USD),Value=CurrencyItem.USD },
                        }.ToList();
            }
        }
        public int TimeDiff { get; set; }
        /// <summary>
        /// 取得刊登種類名稱
        /// </summary>
        public string IssueSetTypeName
        {
            get
            {
                return GetClassDisplayAttrName<IssueSetType>(this.IssueSetType);
            }
        }
        /// <summary>
        /// 取得刊登開始與結束時間差異天數或月數
        /// </summary>
        /// <param name="mode">1: 取得天數 2:取得月數</param>
        /// <returns></returns>
        private double GetIssueTimeDiff(DiffMode mode)
        {
            if(IssueStart.HasValue && IssueEnd.HasValue)
            {
                var ts = new TimeSpan(Convert.ToDateTime(IssueEnd).Ticks - Convert.ToDateTime(IssueStart).Ticks);
                if (mode == DiffMode.Days)
                    return Math.Round(ts.TotalDays, 2);
                else if (mode == DiffMode.Month)
                    return Math.Round(ts.TotalDays / 30, 1);
            }
            return 0;
        }

        public bool IsDurationBelowOneDay()
        {
            if(IssueStart.HasValue && IssueEnd.HasValue)
            {
                var ts = new TimeSpan(Convert.ToDateTime(IssueEnd).Ticks - Convert.ToDateTime(IssueStart).Ticks);

                    return Math.Round(ts.TotalDays, 2) < 1;
            }

            return false;
        }

        public bool IsDurationBelowOneMonth()
        {
            if(IssueStart.HasValue && IssueEnd.HasValue)
            {
                var ts = new TimeSpan(Convert.ToDateTime(IssueEnd).Ticks - Convert.ToDateTime(IssueStart).Ticks);

                    return Math.Round(ts.TotalDays, 2) < 30;
            }

            return false;
        }

        /// <summary>
        /// 每月平均費用
        /// </summary>
        /// <returns></returns>
        public double GetCostPerMonth
        {
            get
            {
                long cost = CostOfPeriod ?? 0;
                double month = GetIssueTimeDiff(DiffMode.Month);

                // 不足一月也視為一月
                if (month < 1)
                    month = 1;

                if (month != 0)
                {
                    return cost / month;
                }
                return 0;
            }
        }
        /// <summary>
        /// 每日平均費用
        /// </summary>
        /// <returns></returns>
        public double GetCostPerDays
        {
            get
            {
                long cost = CostOfPeriod ?? 0;
                double days = GetIssueTimeDiff(DiffMode.Days);

                // 不足一日也視為一日
                if (days < 1)
                    days = 1;

                if (days != 0)
                {
                    return Math.Round(cost / days);
                }
                return 0;
            }
        }
        /// <summary>
        /// 取得刊登時間差異之模式
        /// </summary>
        enum DiffMode
        {
            Days = 1,
            Month = 2
        }
        string GetClassDisplayAttrName<T>(string ItemStr) where T : class
        {
            if (!string.IsNullOrEmpty(ItemStr))
            {
                try
                {
                    MemberInfo[] memInfo = typeof(T).GetMember(ItemStr);
                    var da = memInfo[0].GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                    if (da != null)
                        return da.Name;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        //用於前端判斷本筆資料是不是在效期之內或者是否為最新一筆資料
        public SpecialRowType SpecialRow { get; set; } = SpecialRowType.Normal;

        public enum SpecialRowType
        {
            Normal = 1,
            InDuration = 2,
            Latest = 3
        }
    }
}