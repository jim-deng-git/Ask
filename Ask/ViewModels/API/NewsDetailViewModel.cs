using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class NewsDetailRequest : APISiteRequestBase
    {
        public long CardNo { get; set; }
    }

    public class NewsDetailResult
    {
        /// <summary>
        /// 文章標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章發文日期
        /// </summary>
        public string IssueDate { get; set; }
        /// <summary>
        /// 文章類別
        /// </summary>
        public List<ArticleTypes> ArticleTypes { get; set; }
        /// <summary>
        /// 主影片
        /// </summary>
        public MainVision MainVision { get; set; }
        /// <summary>
        /// 段落
        /// </summary>
        public List<ParagraphItem> ParagraphList { get; set; }
    }

    public enum NewsDetailResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("查無資料")]
        NewsNull,
        Exception
    }
}