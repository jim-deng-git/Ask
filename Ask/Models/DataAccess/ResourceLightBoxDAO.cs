using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.DataAccess
{
    public class ResourceLightBoxDAO
    {
        public static ResourceLightBoxModels GetItem(long sourceNo, string code)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select Top(1) ID,BtnName,BtnColor,Title,Spec from ResourceLightBox Where {0}";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceLightBoxModels>(string.Format(sql, string.Join(" And ", where))).SingleOrDefault();
            }
        }
    }
}