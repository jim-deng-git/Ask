using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace WorkV3.Models.Repository
{
    public class ErrorLogRepository : RepositoryBase<ErrorLogModel>, Interface.IErrorLogRepository<ErrorLogModel>
    {
        public ErrorLogRepository()
        {
            SetTableName("ErrorLog");
        }
    }
}