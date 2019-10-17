using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSettingModel
    {
        public long SiteID { get; set; }
        public string EpaperDisplayName { get; set; }
        public string EpaperSmtpServer { get; set; }
        public string EpaperEmailAcc { get; set; }
        public string EpaperEmailPwd { get; set; }
        public string EpaperEmailFrom { get; set; }
        public int EpaperEmailPort { get; set; }
        public bool EpaperEnabledSSL { get; set; }
        /// <summary>
        /// 連線逾時
        /// </summary>
        public int EpaperTimeOut { get; set; }
        /// <summary>
        /// 黑名單判定次數
        /// </summary>
        public int EpaperSendFailRounds { get; set; }
        /// <summary>
        /// 間隔寄送最小秒數
        /// </summary>
        public int EpaperSendIntervalMin { get; set; }
        /// <summary>
        /// 間隔寄送最大秒數
        /// </summary>
        public int EpaperSendIntervalMax { get; set; }
        /// <summary>
        /// 是否開放民眾訂閱
        /// </summary>
        public bool EpaperOpenToPeople { get; set; }
        /// <summary>
        /// 訂閱喜好 
        /// value:1使用會員喜好2只填入email
        /// </summary>
        public int EpaperSubscribeLike { get; set; }
        /// <summary>
        /// 是否開放會員訂閱
        /// </summary>
        public bool EpaperOpenToMember { get; set; }
    }
}