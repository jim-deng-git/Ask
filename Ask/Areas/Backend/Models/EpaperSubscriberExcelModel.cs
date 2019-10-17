using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 訂閱者名單匯出Excel
    /// </summary>
    public class EpaperSubscriberExcelModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "訂閱者類型")]
        public string OrderType { get; set; }
        [Display(Name = "對應會員姓名/帳號")]
        public string MemberName { get; set; }
        //[Display(Name = "會員身分")]
        //public string MemberShip { get; set; }
        //[Display(Name = "喜好")]
        //public string Like { get; set; }
        [Display(Name = "訂閱報別")]
        public string EpaperTypeName { get; set; }
        //[Display(Name = "追蹤標記")]
        //public string Track { get; set; }
        [Display(Name = "訂閱時間")]
        public string OrderTime { get; set; }
    }
}