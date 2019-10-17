using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class ArticleRepository:RepositoryBase<ArticleModels>
    {
        public ArticleRepository() {
            SetTableName("Article");
        }
    }
}