using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class RecruitJobApplicationToMoneyModel
    {
        public long ID { get; set; }
        public long WorkID { get; set; }
        /// <summary>
        /// 1:增加、-1:減少
        /// </summary>
        public int Type { get; set; }
        public int Money { get; set; }
        public string Reason { get; set; }
    }

    public enum MoneyType
    {
        Plus = 1,
        Minus = -1
    }
}