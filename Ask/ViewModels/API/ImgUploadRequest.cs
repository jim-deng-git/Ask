using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ImgUploadRequest : APISiteRequestBase
    {
        /// <summary>
        /// 圖片檔案 Base64 編碼
        /// </summary>
        public string Img { get; set; }
        /// <summary>
        /// 徵才相關:1、會員:2、聊天室:3
        /// </summary>
        public int Type { get; set; }
    }

    public class ImgUploadResult
    {
        public string FileName { get; set; }
        public string ImageUrl { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
    }

    public enum ImgUploadType
    {
        Recruits = 1,
        Member,
        ChatImg
    }
}