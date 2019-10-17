using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using WorkV3.Areas.Backend.Models.DataAccess;
namespace WorkV3.Common
{
    public class CustomFunc
    {
        #region 確認新增者權限

        public static bool HaveEditPermission(long AddMemberID)
        {
            bool ret = false;
            long MemberID = MemberDAO.SysCurrent.Id;
            #region 新增者直接可以通過
            if (AddMemberID == MemberID)
            {
                ret = true;                
            }
            #endregion

            #region GroupID = 1 直接過
            if (ret == false)
            {
                if (MemberDAO.SysCurrent.GroupId == 1)
                    ret = true;
                //using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                //{
                //    string sql = $"SELECT COUNT(1) FROM Member where GroupID=1 And ID=@MemberID ";
                //    int cnt = conn.ExecuteScalar<int>(sql, new { MemberID = MemberID });
                //    if (cnt > 0) ret = true;
                //}
            }        
            #endregion

            return ret;
        }
        #endregion

        /// <summary>
        /// 取得日期區間List
        /// </summary>
        /// <param name="start">開始日期</param>
        /// <param name="end">結束日期</param>
        /// <returns></returns>
        public static List<DateTime> EachDay(DateTime? start, DateTime? end)
        {
            List<DateTime> result = new List<DateTime>();

            if (start != null && end != null)
            {
                for (DateTime dt = (DateTime)start; dt <= end; dt = dt.AddDays(1))
                {
                    result.Add(dt);
                }
            }
            else if (start != null)
                result.Add((DateTime)start);
            else if (end != null)
                result.Add((DateTime)end);

            return result;
        }
    }
}