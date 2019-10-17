using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSendRecordSearchModel
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
        /// 發送時間開始
        /// </summary>
        public DateTime? SendTimeStart { get; set; }
        /// <summary>
        /// 發送時間結束
        /// </summary>
        public DateTime? SendTimeEnd { get; set; }
        /// <summary>
        /// 調閱報別
        /// </summary>
        public string[] EpaperTypeSelect { get; set; }
        /// <summary>
        /// 發送狀況
        /// </summary>
        public string[] SendResult { get; set; }
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