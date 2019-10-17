using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ProjectTypesModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public int Type { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Color { get; set; }

        public string Description { get; set; }
        public bool IsIssue { get; set; } = true;
        public int Sort { get; set; } = 1;
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }

    public enum ProjectType
    {
        /// <summary>
        /// 報支單類別
        /// </summary>
        ReportType = 2,
        /// <summary>
        /// 預支單類別
        /// </summary>
        EstimatedType,
        /// <summary>
        /// 發票類別
        /// </summary>
        ReceiptType
    }
}