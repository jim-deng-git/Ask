using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ParagraphModels
    {
        public long ID { get; set; }
        public long SourceNo { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string MatchType { get; set; }        
        public string ImgPos { get; set; }
        public string ImgAlign { get; set; }
        public byte Sort { get; set; }
        public bool IsOriginalSize { get; set; } = false;
        public string SizeType { get; set; } = "0";
    }
}