using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ParagraphItem
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public object ContentList { get; set; }
    }

    public class ParagraphImage : ParagraphItem
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class ParagraphImageList
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class ParagraphVideo : ParagraphItem
    {
        public string VideoType { get; set; }
        public string Screenshot { get; set; }
    }

    public class ParagraphFile : ParagraphItem
    {
        public string Name { get; set; }
    }
}