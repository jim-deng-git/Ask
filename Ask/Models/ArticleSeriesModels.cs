using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class ArticleSeriesModels
    {
        public long ID { get; set; }        
        public string Name { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }

        public int GetArticleCount() {
            return ArticleSeriesDAO.GetArticleCount(ID);
        }
    }
}