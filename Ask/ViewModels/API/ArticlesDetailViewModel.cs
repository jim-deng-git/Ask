using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ArticlesDetailRequest : APISiteRequestBase
    {
        public long CardNo { get; set; }
    }

    public class ArticlesDetailResult
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

    public class MainVision
    {
        /// <summary>
        /// youtube、vimeo、custom
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 影片Link
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 自行上傳圖片
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 自行上傳圖片寬度
        /// </summary>
        public int ImgWidth { get; set; }
        /// <summary>
        /// 自行上傳圖片高度
        /// </summary>
        public int ImgHeight { get; set; }
    }

    public enum ArticlesDetailResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("查無資料")]
        ArticlesNull,
        Exception
    }
}