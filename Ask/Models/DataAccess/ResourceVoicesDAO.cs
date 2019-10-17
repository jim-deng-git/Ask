using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ResourceVoicesDAO
    {
        public static IEnumerable<ResourceVoicesModels> GetItems(long sourceNo, string code = null) {
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select ID, Path, ShowName From ResourceVoices Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceVoicesModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
            }
        }        
    }
}