using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 電子報訂閱退訂紀錄
    /// </summary>
    public class EpaperSubscribeRecordModel
    {
        /// <summary>
        /// 報別ID
        /// </summary>
        public long EpaperType_ID { get; set; }
        /// <summary>
        /// 訂閱者ID
        /// </summary>
        public long EpaperSubscriber_ID { get; set; }
        /// <summary>
        /// 是否訂閱
        /// </summary>
        public bool IsSubscribe { get; set; }
        /// <summary>
        /// 紀錄時間
        /// </summary>
        public DateTime RecordTime { get; set; }

        //---
        public bool EpaperTypeStatus { get; set; }
        public bool IsMember { get; set; }
    }
}