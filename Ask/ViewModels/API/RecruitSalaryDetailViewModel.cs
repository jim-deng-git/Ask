using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitSalaryDetailRequest
    {
        /// <summary>
        /// 工作(報到)ID
        /// </summary>
        public long WorkID { get; set; }
    }

    public class RecruitSalaryDetailResult
    {
        /// <summary>
        /// 工作(報到)ID
        /// </summary>
        public long WorkID { get; set; }
        /// <summary>
        /// 職缺名稱
        /// </summary>
        public string RecruitTitle { get; set; }
        /// <summary>
        /// 工作類型
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 報到日
        /// </summary>
        public string CheckDate { get; set; }
        /// <summary>
        /// 工作時數
        /// </summary>
        public string Hours { get; set; }
        /// <summary>
        /// 所得薪資
        /// </summary>
        public string Salary { get; set; }
        /// <summary>
        /// 扣繳稅率
        /// </summary>
        public string TaxRate { get; set; }
        /// <summary>
        /// 估算薪資
        /// </summary>
        public string EstimatedSalary { get; set; }
        /// <summary>
        /// 實領薪資
        /// </summary>
        public string FinalSalary { get; set; }
        /// <summary>
        /// 領款人
        /// </summary>
        public string MemberName { get; set; }
        /// <summary>
        /// 匯款帳號
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 戶籍地址
        /// </summary>
        public string MemberAddress { get; set; }
        /// <summary>
        /// 注意事項
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 領款人是否確認
        /// </summary>
        public bool IsPayeeCheck { get; set; }
        public List<string> CheckInList { get; set; }
        public List<MoneyList> MoneyList { get; set; }
        /// <summary>
        /// 工作設定薪資
        /// </summary>
        public string JobSalary { get; set; }
    }

    public class MoneyList
    {
        /// <summary>
        /// 1:增加、-1:減少
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 金額
        /// </summary>
        public string Money { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }
    }

    public enum RecruitSalaryDetailResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("查無薪資")]
        WorkNull,
        Exception
    }
}