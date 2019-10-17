using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class KeywordQueriedViewModel
    {
        public long ID { get; set; }
        public string Text { get; set; }
        public bool IsIssue { get; set; }
        public int Count { get; set; }
        public int? KeywordQueriedID { get; set; }
    }
}