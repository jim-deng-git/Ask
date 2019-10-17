using System;
using WorkV3.Common;
namespace WorkV3.Areas.Backend.Models
{
    public class EpaperAreaModel
    {
        public long ID { get; set; }
        public long EpaperID { get; set; }
        public string Title { get; set; }
        public EpaperAreaType AreaType { get; set; }
        public string Img { get; set; }
        public bool IsIssue { get; set; }
        public string LinkUrl { get; set; }
        public string LinkTarget { get; set; }
        public string AreaContent { get; set; }
        public EpaperImgStyle ImgStyle { get; set; }
        public EpaperImgPosition ImgPosition { get; set; }
        public int Sort { get; set; } = 0;
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string SendStatus { get; set; }
    }
}