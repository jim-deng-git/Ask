using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.ViewModels.API
{
    public class RecruitDetailRequest : APISiteRequestBase
    {
        /// <summary>
        /// 工作類型ID
        /// </summary>
        public long RecruitJobID { get; set; }
        /// <summary>
        /// 應徵ID(取得應徵上傳照片用的), 可為null
        /// </summary>
        public long? ApplicationID { get; set; }
    }

    public class RecruitDetailResult
    {
        /// <summary>
        /// 段落
        /// </summary>
        public List<ParagraphItem> ParagraphList { get; set; }
        /// <summary>
        /// 工作
        /// </summary>
        public List<BlockItem> JobList { get; set; }
        /// <summary>
        /// 聯繫方式
        /// </summary>
        public List<BlockItem> ContactList { get; set; }
        /// <summary>
        /// 0:未登入 99:error 1:未收藏 2:已收藏
        /// </summary>
        public CollectionResult Collection { get; set; }
        /// <summary>
        /// 是否已應徵
        /// </summary>
        public bool IsSignUp { get; set; }
        /// <summary>
        /// 是否可以取消應徵
        /// </summary>
        public bool IsClientCancel { get; set; }
        /// <summary>
        /// 倒數幾天
        /// </summary>
        public int FinalCnt { get; set; }
        /// <summary>
        /// 分享網址
        /// </summary>
        public string ShareUrl { get; set; }
        public List<ClothesList> ClothesPhoto { get; set; }
    }

    public class BlockItem
    {
        public string Type { get; set; }
        public object Content { get; set; }
        public object ContentList { get; set; }
    }

    public class Contentlist
    {
        public string Name { get; set; }
        public string Photo { get; set; }
    }

    public class ClothesList
    {
        public int Width { get; set; }
        public int Height { get; set; }
        /// <summary>
        /// 檔案名稱(傳入RecruitSetClothesPhoto用)
        /// </summary>
        public string ImgName { get; set; }
        /// <summary>
        /// 圖片顯示Url
        /// </summary>
        public string ImgUrl { get; set; }
    }

    public enum RecruitDetailResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("查無職缺")]
        RecruitNull,
        [Message("查無工作類型")]
        RecruitJobNull,
        Exception
    }
}