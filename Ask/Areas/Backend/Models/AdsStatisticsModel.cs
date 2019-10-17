using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 廣告統計
    /// </summary>
    public class AdsStatisticsModel
    {
        /// <summary>
        /// 自訂廣告ID
        /// </summary>
        public long AdsCustomizeID { get; set; }
        /// <summary>
        /// PageID
        /// </summary>
        public long PageID { get; set; }
        /// <summary>
        /// Session ID
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// Device ID (CPU ID)
        /// </summary>
        public string DeviceID { get; set; }
        /// <summary>
        /// 瀏覽器
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 事件類型
        /// </summary>
        public string Event { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public long IPNum { get; set; }
        /// <summary>
        /// 會員(Membership)ID
        /// </summary>
        public long MemberID { get; set; }
        /// <summary>
        /// 記錄日
        /// </summary>
        public DateTime RecordDay { get; set; }
        /// <summary>
        /// 紀錄時間
        /// </summary>
        public DateTime RecordTime { get; set; }
    }

    public class StatisticsModel
    {
        public string dt { get; set; }
        public int BrowseCount { get; set; }
        public int ClickCount { get; set; }
    }

    public class AdsStatisticsSearchModel
    {
        /// <summary>
        /// 分析開始時間
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 分析結束時間
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 廣告主 ID
        /// </summary>
        public List<long> Advertisers { get; set; }
        /// <summary>
        /// 排列方式
        /// 1: 點擊量 > 瀏覽人次
        /// 2: 瀏覽人次 > 點擊量
        /// 3: 費用估算 > 點擊量 > 瀏覽人次
        /// 4: CP值 > 點擊量 > 瀏覽人次
        /// </summary>
        public int OrderType { get; set; } = 1;
        public bool IsAsc { get; set; } = false;
        public long? SiteId { get; set; }
    }

    public class AdsDetailStatisticsSearchModel
    {
        /// <summary>
        /// 分析開始時間
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 分析結束時間
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 排序方向是否為 ascending
        /// </summary>
        public bool IsAsc { get; set; } = false;
        /// <summary>
        /// 是否顯示特定廣告詳細資料
        /// </summary>
        public long AdsCustomId { get; set; } = 0;
        public DetailType Type { get; set; } = DetailType.Click;
        public string PreviousPage { get; set; }
    }

    public enum DetailType
    {
        Click = 1,
        Browsing
    }
}