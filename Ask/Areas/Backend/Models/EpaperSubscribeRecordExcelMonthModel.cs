using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 月報表
    /// </summary>
    public class EpaperSubscribeRecordExcelMonthModel
    {
        [Display(Name = "時間")]
        public string Time { get; set; }
        [Display(Name = "訂閱數量")]
        public string TotalOrder { get; set; }
        [Display(Name = "訂閱百分比")]
        public string TotalOrderPercent { get; set; }
        [Display(Name = "退訂數量")]
        public string TotalUnOrder { get; set; }
        [Display(Name = "退訂百分比")]
        public string TotalUnOrderPercent { get; set; }
    }
}