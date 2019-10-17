using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class SiteLangMenuModel
    {

        public long ID { get; set; }
        public string IDStr { get; set; }
        public long SiteID { get; set; }
        public string Title { get; set; } = "";
        public string LangCode { get; set; }
        public string LinkType { get; set; }
        public bool? IsOpenNew { get; set; } = false;
        public long? SelectSiteID { get; set; }
        public string Url { get; set; }
        public int Sort { get; set; } = 0;
        public bool? Translate { get; set; } = false;
        public bool IsShow { get; set; } = false;

    }
    /// <summary>
    /// 連結類型
    /// </summary>
    public static class LangLinkType
    {

        /// <summary>
        /// 本站主語系
        /// </summary>
        public static string Main = "Main";
        /// <summary>
        /// 本平台其他網站
        /// </summary>
        public static string WebSites = "WebSites";
        /// <summary>
        /// 外部連結
        /// </summary>
        public static string OutLink = "OutLink";
        /// <summary>
        /// 與主語系繁簡中文切換
        /// </summary>
        public static string CnTw = "CnTw";
    }
}