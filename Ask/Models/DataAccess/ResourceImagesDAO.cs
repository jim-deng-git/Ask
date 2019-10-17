using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ResourceImagesDAO
    {
        public static IEnumerable<ResourceImagesModels> GetItems(long sourceNo, string code = null) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select ID, SourceNo, Img, SizeType, IsOriginalSize, MultiImgStyle, Spec, SpecDetail, Link, IsOpenNew, IsShow From ResourceImages Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceImagesModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
            }
        }
    }
}