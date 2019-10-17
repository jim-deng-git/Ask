using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisPageViewSessionViewModel
    {
        public string No { get; set; }
        public string SiteID { get; set; }
        public string SiteSN { get; set; }
        public string SN { get; set; }
        public string PageCreator { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string RefererTitle { get; set; }
        public string RefererPageNo { get; set; }
        public string RefererUrl { get; set; }
        /// <summary>
        /// Session ID
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// 進入時間
        /// </summary>
        public string LogDate { get; set; }
        /// <summary>
        /// 停留秒數
        /// </summary>
        public double StaySeconds { get; set; }
        /// <summary>
        /// 裝置 ID
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 裝置 ID
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 會員姓名/帳號
        /// </summary>
        public string MemberInfo { get; set; }
        /// <summary>
        /// 會員ID
        /// </summary>
        public string MemberID { get; set; }
        /// <summary>
        /// 會員照片
        /// </summary>
        public string MemberPhoto { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
    }
}