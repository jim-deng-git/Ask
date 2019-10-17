using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 發送紀錄匯出Excel
    /// </summary>
    public class EpaperSendRecordExcelModel
    {
        [Display(Name ="Email")]
        public string Email { get; set; }
        [Display(Name = "訂閱者類型")]
        public string OrderType { get; set; }
        [Display(Name = "對應會員姓名/帳號")]
        public string MemberName { get; set; }
        [Display(Name = "訂閱報別")]
        public string EpaperTypeName { get; set; }
        [Display(Name = "發送狀況(發送時間)")]
        public string SendStatus { get; set; }
        [Display(Name = "已讀(讀取時間)")]
        public string ReadStatus { get; set; }
    }
}