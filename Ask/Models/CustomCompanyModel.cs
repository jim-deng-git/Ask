using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class CustomCompanyModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 公司統編
        /// </summary>
        public string TaxIdNumber { get; set; }
        /// <summary>
        /// 公司關帳開始時間
        /// </summary>
        public DateTime? AccountCloseDateStart { get; set; }
        /// <summary>
        /// 公司關帳結束時間
        /// </summary>
        public DateTime? AccountCloseDateEnd { get; set; }
        /// <summary>
        /// 付款週期
        /// </summary>
        public string PayingPeriod { get; set; }
        /// <summary>
        /// 付款舉例說明
        /// </summary>
        public string PayDescription { get; set; }
        public string Regions { get; set; }
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime ModifyTime { get; set; }
        public long Modifier { get; set; }
        public int Sort { get; set; } = 0;
    }
}