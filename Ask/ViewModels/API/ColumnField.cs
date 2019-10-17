using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 共用的欄位設定 view model
    /// </summary>
    public class ColumnField
    {
        /// <summary>
        /// 欄位辨視的 key 值
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 欄位標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 欄位內容
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 排序值
        /// </summary>
        public int SortIndex { get; set; }
    }

    public class PhotoColumn : ColumnField
    {
        public string FileName { get; set; }
    }
}