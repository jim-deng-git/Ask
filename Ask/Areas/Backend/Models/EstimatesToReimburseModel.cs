using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EstimatesToReimburseModel
    {
        public long ID { get; set; }
        public long ReimburseID { get; set; }
        public long EstimateID { get; set; }
        /// <summary>
        /// 報支單已申請金額
        /// </summary>
        public int ReimburseTotal { get; set; }

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
        /// 原始預支金額
        /// </summary>
        public int Total { get; set; }

    }
}