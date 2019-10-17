using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class SearchModel
    {
        public string Key { get; set; }

        public DateTime? IssueStart { get; set; }

        public DateTime? IssueEnd { get; set; }
    }
}