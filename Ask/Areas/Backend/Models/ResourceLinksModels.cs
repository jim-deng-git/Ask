using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ResourceLinksModels
    {

        public long Id { get; set; }
        public long SiteID { get; set; }
        public long SourceNo { get; set; }
        public byte SourceType { get; set; }
        public int Ver { get; set; }
        public byte AreaID { get; set; }
        public string Code { get; set; }
        public string LinkType { get; set; }
        public string LinkInfo { get; set; }
        public string Descriptions { get; set; }
        public string Detail { get; set; }
        public bool? IsOpenNew { get; set; }
        public int ClickType { get; set; }
        public int? Sort { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string InLinkPath { get; set; }
        public string InlinkTitle { get; set; }
        public string GetLink() {
            if (LinkType == ResourceLinkType.outLink)
                return LinkInfo;

            // TODO: 內部連結處理
            return "#";
        }
    }


    /// <summary>
    /// 連結類型
    /// </summary>
    public static class ResourceLinkType
    {

        /// <summary>
        /// 內部連結
        /// </summary>
        public static string inLink = "inLink";
        /// <summary>
        /// 外部連結
        /// </summary>
        public static string outLink = "outLink";

        //其他來源outFile_xxx

    }

}