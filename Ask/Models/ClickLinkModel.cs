using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ClickLinkModel
    {
        public string Link { get; set; }

        public string LinkType { get; set; }

        public bool IsOpenNew { get; set; }
    }
}