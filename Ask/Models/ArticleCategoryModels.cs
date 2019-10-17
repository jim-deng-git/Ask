using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ArticleCategoryModels
    {
        public long ArticleID { get; set; }
        public long CategoryID { get; set; }
        public string CategoryType { get; set; }
        public string Title { get; set; }
    }
}