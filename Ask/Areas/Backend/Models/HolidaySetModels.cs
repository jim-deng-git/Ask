using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 休假日設定
    /// </summary>
    public class HolidaySetModels
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 假日說明
        /// </summary>
        public string Description { get; set; }
        public int Type { get; set; }
    }

    public enum HolidayType
    {
        /// <summary>
        /// 平日
        /// </summary>
        WeekDay = 0,
        /// <summary>
        /// 例假日
        /// </summary>
        Holiday = 1,
        /// <summary>
        /// 國定假日
        /// </summary>
        NationalHoliday
    }
}