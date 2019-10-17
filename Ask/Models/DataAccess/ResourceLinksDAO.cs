using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ResourceLinksDAO
    {
        public static IEnumerable<ResourceLinksModels> GetItems(long sourceNo, string code = null) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select ID, LinkType, LinkInfo, Descriptions, Detail, IsOpenNew From ResourceLinks Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceLinksModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
            }
        }
    }
}