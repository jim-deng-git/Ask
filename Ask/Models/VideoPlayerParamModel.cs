using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class VideoPlayerParamModel
    {
        public string Url { get; set; }
        public string Image { get; set; } = "";
        public bool AutoPlay { get; set; } = true;
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;
    }
}