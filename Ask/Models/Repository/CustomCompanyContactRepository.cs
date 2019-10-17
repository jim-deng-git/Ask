using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class CustomCompanyContactRepository : RepositoryBase<CustomCompanyContactModel>, Interface.ICustomCompanyContactRepository<CustomCompanyContactModel>
    {
        public CustomCompanyContactRepository()
        {
            SetTableName("CustomCompanyContact");
        }
    }
}