using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Interface
{
    public interface ICompanyRepository : IDisposable, IGenericRepository<Areas.Backend.Models.CompanyModel>
    {
    }
}