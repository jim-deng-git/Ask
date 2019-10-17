using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Golbal;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ResourceFilesDAO
    {
        public static IEnumerable<ResourceFilesModels> GetItems(long sourceNo, string code = null) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select ID, FileType, FileInfo, FileSize, Descriptions, Detail, ShowName From ResourceFiles Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceFilesModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
            }
        }

        /// <summary>
        /// 傳入ID取得檔案名稱
        /// </summary>
        /// <param name="sourceNo"></param>
        /// <returns></returns>
        public static IEnumerable<ResourceFilesModels> GetFileInfo(long sourceNo) //Joe 20190924
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select r.FileInfo From ResourceFiles as r inner join Menus as m on r.SourceNo=m.ID Where r.SourceNo = @SourceNo ";
                return conn.Query<ResourceFilesModels>(sql, new {
                    SourceNo = sourceNo
                });
            }
        }
    }
}