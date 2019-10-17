using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 日報表
    /// </summary>
    public class EpaperSubscribeRecordExcelDayModel
    {
        [Display(Name = "時間")]
        public string Time { get; set; }
        [Display(Name = "訂閱數量")]
        public string TotalOrder { get; set; }
        [Display(Name = "退訂數量")]
        public string TotalUnOrder { get; set; }
    }
}