using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AdsStatisticsDetailViewModel
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
        /// <summary>
        /// 關聯到的 page 標題
        /// </summary>
        public string PageTitle { get; set; }

        public string Title { get; set; }
        /// <summary>
        /// 會員資料
        /// </summary>
        public string MemberName { get; set; }
        public string MemberEmail { get; set; }
    }
}