using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ArticleSearchModels {
        public long MenuID { get; set; }
        public string Key { get; set; }
        public IEnumerable<long> Types { get; set; }
        public DateTime? IssueStart { get; set; }
        public DateTime? IssueEnd { get; set; }        
    }
}