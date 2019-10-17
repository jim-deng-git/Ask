using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class MatchViewModel
    {
        #region 段落搭配Model
        public class ImagesModel
        {
            public string ImgUrl { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class VideoModel
        {
            /// <summary>
            /// 影片種類(custom: 自行上傳、youtube、tudou: 土豆、vimeo)
            /// </summary>
            public string VideoType { get; set; }
            /// <summary>
            /// 影片連結
            /// </summary>
            public string VideoUrl { get; set; }
            /// <summary>
            /// 影片封面圖
            /// </summary>
            public string CoverImg { get; set; }
        }

        public class LinkModel
        {
            /// <summary>
            /// 連結名稱
            /// </summary>
            public string LinkName { get; set; }
            /// <summary>
            /// 連結網址
            /// </summary>
            public string LinkUrl { get; set; }
        }

        public class VoiceModel
        {
            /// <summary>
            /// 聲音連結
            /// </summary>
            public string VoiceUrl { get; set; }
        }

        public class FileModel
        {
            public string FileUrl { get; set; }
            public string FileName { get; set; }
        }
        #endregion
    }
}