using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WorkV3.Models
{
    public class StoreModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string SquareLogoComputer { get; set; }
        public string RectangleLogoComputer { get; set; }
        public string LogoMobile { get; set; }
        public string MainVisionComputer { get; set; }
        public string MainVisionMobile { get; set;  }
        public long CurrencyID { get; set; }
        public string SN { get; set; }

        /// <summary>
        /// 是否刊登
        /// </summary>
        public bool IsIssue { get; set; }
        public DateTime? IssueStart { get; set; }
        public DateTime? IssueEnd { get; set; }
        /// <summary>
        /// 是否有效(強迫停權 為 false)
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>
        /// 是否推薦
        /// </summary>
        public bool IsRecommand { get; set; }
        public string RecommandNote { get; set;  }
        public bool AddManager(long memberId)
        {
            return true;
        }

        public bool AddManagers(List<long> memberId)
        {
            return true;
        }

        public string CompanyName { get; set; }
        public string CompanyNameEn { get; set; }
        public string CompanyNum { get; set; }
        public string CompanyPerson { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress { get; set; }
        public string BankID { get; set; }
        public string BankBranchID { get; set; }
        public string BankAccount { get; set; }
        public string BankAccountName { get; set; }
        public string Remark { get; set; }
        public int StyleMode { get; set; }
        public string Slogan { get; set; }
        public string Intro { get; set; }
        public string Country { get; set; }
        public int Reciept { get; set; }
        public int MessageMode { get; set; }
        public string Message { get; set; }
    }
    public class StoreSearch
    {
        public long SiteID { get; set; } = 0;
    }
}