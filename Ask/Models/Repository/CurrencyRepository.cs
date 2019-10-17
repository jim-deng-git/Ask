using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace WorkV3.Models.Repository
{
    public class CurrencyRepository : RepositoryBase<CurrencyModel>, Interface.ICurrencyRepository<CurrencyModel>
    {
        public CurrencyRepository()
        {
            SetTableName("Currency");
        }
    }
}