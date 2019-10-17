using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ClickImgModel
    {
        public long ID { get; set; }

        public string Img { get; set; }

        public string Spec { get; set; }

        public string SpecDetail { get; set; }

        public string Link { get; set; }

        public bool? IsOpenNew { get; set; }

        public bool IsShow { get; set; }
    }
}