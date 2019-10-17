using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberFieldStatisticViewModel
    {
        /// <summary>
        /// 欄位值
        /// </summary>
        public string FieldValue { get; set; }
        /// <summary>
        /// 計數
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 計數百分比
        /// </summary>
        public decimal Percentage { get; set; }
    }
}