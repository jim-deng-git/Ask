using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class KeywordSaveViewModel
    {
        public long? ID { get; set; }
        public string Text { get; set; }
        public bool IsIssue { get; set; }
    }
}