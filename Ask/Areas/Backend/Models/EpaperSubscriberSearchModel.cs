using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSubscriberSearchModel
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 訂閱者類型
        /// </summary>
        public string SubscriberType { get; set; }
        /// <summary>
        /// 調閱報別
        /// </summary>
        public string[] EpaperTypeSelect { get; set; }
        /// <summary>
        /// 訂閱時間 開始
        /// </summary>
        public DateTime? CreateTime_Start { get; set; }
        /// <summary>
        /// 訂閱時間 結束
        /// </summary>
        public DateTime? CreateTime_End { get; set; }
        /// <summary>
        /// 會員身份
        /// </summary>
        public string[] identitySelect { get; set; }
        /// <summary>
        /// 喜好
        /// </summary>
        public string[] favoritySelect { get; set; }
    }
}