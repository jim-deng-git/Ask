using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ResourceImagesModels {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long SourceNo { get; set; }
        public byte SourceType { get; set; }
        public int Ver { get; set; }
        public byte AreaID { get; set; }
        public string Code { get; set; }
        public string Img { get; set; }
        /// <summary>
        /// 0:小 1:中 2:大
        /// </summary>
        public byte? SizeType { get; set; }
        public bool IsOriginalSize { get; set; }
        public string MultiImgStyle { get; set; }
        public string Spec { get; set; }
        public string SpecDetail { get; set; }
        public string Link { get; set; }
        public bool? IsOpenNew { get; set; }
        public bool IsShow { get; set; }
        public int? Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}