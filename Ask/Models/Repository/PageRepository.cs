using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class PageRepository : RepositoryBase<PagesModels>, Interface.IPageRepository<PagesModels>
    {
        public PageRepository()
        {
            SetTableName("Pages");
        }
    }
}