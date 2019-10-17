using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Golbal
{
    public class PermissionCheck
    {
        public static bool havePermission(long MemberID, string PermissionName, long? SiteID)
        {
            bool ret = false;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"SELECT COUNT(1) FROM Member m LEFT JOIN PermissionGroup p ON m.GroupId=p.GroupId WHERE m.ID=@MemberID AND p.PermissionName=@PermissionName ";
                var paras = new { MemberID = MemberID, PermissionName = PermissionName };
                int cnt = 0;
                if (SiteID.HasValue)
                {
                    sql += " and SiteID=@SiteID ";
                    cnt = conn.ExecuteScalar<int>(sql, new { MemberID = MemberID, PermissionName = PermissionName, SiteID = SiteID });
                }
                else
                {
                    cnt = conn.ExecuteScalar<int>(sql, new { MemberID = MemberID, PermissionName = PermissionName });
                }
                
                if (cnt > 0) ret = true;
            }

            return ret;
        }
    }
}