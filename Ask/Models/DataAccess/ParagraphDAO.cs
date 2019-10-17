using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ParagraphDAO
    {
        public static IEnumerable<ParagraphModels> GetItems(long sourceNo) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select ID, Title, Contents, MatchType, ImgPos, ImgAlign From Paragraph Where SourceNo = {0} Order By Sort";
                
                return conn.Query<ParagraphModels>(string.Format(sql, sourceNo));
            }
        }

        public static string GetFirstImage(long sourceNo, string code = null) {
            string sql = "SELECT TOP(1) I.Img FROM Paragraph P JOIN ResourceImages I ON P.ID = I.SourceNo WHERE {0} ORDER BY P.Sort, ISNULL(I.Sort, {1})";

            List<string> where = new List<string>();
            where.Add("P.SourceNo = " + sourceNo);
            where.Add("I.Img <> ''");

            if (!string.IsNullOrWhiteSpace(code))
                where.Add($"I.Code = N'{ code.Replace("'", "''") }'");

            string whereSql = string.Join(" AND ", where);

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.ExecuteScalar<string>(string.Format(sql, whereSql, int.MaxValue));
            }
        }

        public static ResourceVideosModels GetFirstVideo(long sourceNo, string code = null) {
            string sql = "SELECT TOP(1) V.* FROM Paragraph P JOIN ResourceVideos V ON P.ID = V.SourceNo WHERE {0} ORDER BY P.Sort";

            List<string> where = new List<string>();
            where.Add("P.SourceNo = " + sourceNo);
            
            if (!string.IsNullOrWhiteSpace(code))
                where.Add($"V.Code = N'{ code.Replace("'", "''") }'");

            string whereSql = string.Join(" AND ", where);

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ResourceVideosModels>(string.Format(sql, whereSql)).SingleOrDefault();
            }
        }

        public static string GetFirstContent(long sourceNo)
        {
            string sql = "SELECT TOP(1) Contents FROM Paragraph WHERE {0} ORDER BY Sort, ISNULL(Sort, {1})";

            List<string> where = new List<string>();
            where.Add("SourceNo = " + sourceNo);
            where.Add("Contents <> ''");

            string whereSql = string.Join(" AND ", where);

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.ExecuteScalar<string>(string.Format(sql, whereSql, int.MaxValue));
            }
        }
    }
}