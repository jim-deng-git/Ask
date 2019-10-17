using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class NewsListRequest : APISiteRequestBase
    {
        /// <summary>
        /// 第幾頁
        /// </summary>
        public int? Index { get; set; }
        public long[] Types { get; set; }
    }

    public class NewsListResult
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 文章CardNo
        /// </summary>
        public long CardNo { get; set; }
        /// <summary>
        /// page、link:連結外部頁面、 archive:連結檔案
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 文章標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章發文日期
        /// </summary>
        public string IssueDate { get; set; }
        /// <summary>
        /// Type = (link、archive)的連結
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 文章類別
        /// </summary>
        public List<ArticleTypes> ArticleTypes { get; set; }
    }


    public enum NewsListResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("Menu尚未建立")]
        ModuleSNNull,
        [Message("Module SN 尚未建立")]
        MenuNull,
        Exception
    }
}