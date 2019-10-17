using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class CustomCompanyRepository : RepositoryBase<CustomCompanyModel>, Interface.ICustomCompanyRepository<CustomCompanyModel>
    {
        public CustomCompanyRepository()
        {
            SetTableName("CustomCompany");
        }
    }
}