using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ChatQuickCommentModel
    {
        public long ID { get; set; }
        public string QuickComment { get; set; }
        public DateTime CreatTime { get; set; }
    }
}