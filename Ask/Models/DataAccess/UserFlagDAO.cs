using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;

namespace WorkV3.Models.DataAccess {
    public class UserFlagDAO {
        public static IEnumerable<string> GetFlags(long siteId, params string[] userNo) {
            if (userNo == null || userNo.Length == 0)
                return new List<string>();

            IEnumerable<string> userNoList = userNo.Where(no => !string.IsNullOrWhiteSpace(no)).Select(no => $"N'{ no.Replace("'", "''") }'");
            if (userNoList.Count() == 0)
                return new List<string>();

            string sql = $"Select Distinct Flag From UserFlag Where SiteID = { siteId } AND UserNo In ({ string.Join(", ", userNoList) })";
            using(SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<string>(sql);
            }
        }

        public static IEnumerable<string> GetFlags(long siteId) {
            string sql = "Select Distinct Flag From UserFlag Where SiteID = " + siteId;
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<string>(sql);
            }
        }

        public static void SetItem(long siteId, string flag, params string[] userNo) {
            if (string.IsNullOrWhiteSpace(flag) || userNo == null || userNo.Length == 0)
                return;

            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            string fmt = 
                $"IF Not Exists(Select 1 From UserFlag Where SiteID = { siteId } AND UserNo = N'{{0}}' AND Flag = N'{ flag.Replace("'", "''") }')\n" +
                $"  Insert UserFlag(SiteID, UserNo, Flag) Values({ siteId }, N'{{0}}', N'{ flag.Replace("'", "''") }')\n";
            foreach(string no in userNo) {
                if(!string.IsNullOrWhiteSpace(no))
                    sql.AppendFormat(fmt, no.Replace("'", "''"));
            }

            if (sql.Length == 0)
                return;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql.ToString());
        }

        public static void Delete(long siteId, string flag, params string[] userNo) {
            if (userNo == null || userNo.Length == 0 || string.IsNullOrWhiteSpace(flag))
                return;

            IEnumerable<string> userNoList = userNo.Where(no => !string.IsNullOrWhiteSpace(no)).Select(no => $"N'{ no.Replace("'", "''") }'");
            if (userNoList.Count() == 0)
                return;

            string sql = $"Delete UserFlag Where SiteID = { siteId } AND Flag = N'{ flag.Replace("'", "''") }' AND UserNo In ({ string.Join(", ", userNoList) })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static void Delete(long siteId, params string[] userNo) {
            if (userNo == null || userNo.Length == 0)
                return;

            IEnumerable<string> userNoList = userNo.Where(no => !string.IsNullOrWhiteSpace(no)).Select(no => $"N'{ no.Replace("'", "''") }'");
            if (userNoList.Count() == 0)
                return;

            string sql = $"Delete UserFlag Where SiteID = { siteId } AND UserNo In ({ string.Join(", ", userNoList) })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }
    }
}