using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ClickVideoModel
    {
        public long ID { get; set; }

        public string Type { get; set; }

        public string Link { get; set; }

        public string PlayMode { get; set; }

        public bool IsAuto { get; set; }

        public bool IsRepeat { get; set; }
    }
}