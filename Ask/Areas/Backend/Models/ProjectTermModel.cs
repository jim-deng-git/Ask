using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ProjectTermModel
    {
        public long SiteID { get; set; }
        public int Type { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Contents { get; set; }

        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }

    public enum ProjectTermType
    {
        /// <summary>
        /// 報價單
        /// </summary>
        Quote = 1,
        /// <summary>
        /// 報支單
        /// </summary>
        Report,
        /// <summary>
        /// 預支單
        /// </summary>
        Estimated,
        /// <summary>
        /// 發票
        /// </summary>
        Receipt
    }
}