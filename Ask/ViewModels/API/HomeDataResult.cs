using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class HomeDataResult
    {
        /// <summary>
        /// 功能模組
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 樣式
        /// </summary>
        public int Style { get; set; }
        /// <summary>
        /// 功能模組內容
        /// </summary>
        public object Data { get; set; }
        public object List { get; set; }
    }

    #region Data Model

    /// <summary>
    /// 圖文卡組
    /// </summary>
    public class ImageTextList
    {
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 連結
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 內文
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 圖片URL
        /// </summary>
        public string ImgUrl { get; set; }
        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }
        /// <summary>
        /// 搭配類型(1:img、2:video、3:voice、4:file、5:link)
        /// </summary>
        public int MatchType { get; set; }
        /// <summary>
        /// 搭配資料
        /// </summary>
        public object MatchData { get; set; }
    }

    /// <summary>
    /// 主視覺
    /// </summary>
    public class HomeMainVision
    {
        /// <summary>
        /// Video: 影片、Image: 圖片
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 影片/圖片URL
        /// </summary>
        public string Url { get; set; }
        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }
        /// <summary>
        /// 連結
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 輪播秒數
        /// </summary>
        public int Second { get; set; }
    }

    /// <summary>
    /// 視差元件
    /// </summary>
    public class Parallax
    {
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 圖片Url
        /// </summary>
        public string ImgUrl { get; set; }
        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }
    }

    /// <summary>
    /// 文字卡
    /// </summary>
    public class PlainText
    {
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// 內容
        /// </summary>
        public string Description { get; set; } = "";
    }

    public class ArticleSet
    {
        public string SiteSN { get; set; }
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

    public class RecruitSet
    {
        public long ID { get; set; }
        public string SiteSN { get; set; }
        public string ImgUrl { get; set; }
        public string Title { get; set; }
        public List<RecruitTypes> Types { get; set; }
    }

    #endregion

    public class RecruitTypes
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}