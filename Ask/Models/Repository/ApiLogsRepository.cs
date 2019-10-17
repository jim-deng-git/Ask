using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using WorkV3.Models.Interface;

namespace WorkV3.Models.Repository
{
    public class ApiLogsRepository : RepositoryBase<ApiLogsModels>, Interface.IGenericRepository<ApiLogsModels>
    {
        public ApiLogsRepository()
        {
            SetTableName("ApiLogs");
        }
    }
}