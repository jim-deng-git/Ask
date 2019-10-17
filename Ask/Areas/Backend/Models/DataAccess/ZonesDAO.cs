using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using WorkV3.Models.Interface;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ZonesDAO
    {
        public static void SetZoneInfo(ZonesModels zone) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Zones");
            tableObj.GetDataFromObject(zone);

            string sql = $"Select 1 From Zones Where No = { zone.No } AND Ver = { zone.Ver } AND SiteID = { zone.SiteID } AND PageNo = { zone.PageNo }";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj["Creator"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {
                string[] removeFields = { "No", "Ver", "SiteID", "PageNo", "Creator", "CreateTime" };
                foreach (string field in removeFields)
                    tableObj.Remove(field);

                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                SQLData.ParameterCollection keys = new SQLData.ParameterCollection();
                keys.Add("@No", zone.No);                
                keys.Add("@Ver", zone.Ver);
                keys.Add("@SiteID", zone.SiteID);
                keys.Add("@PageNo", zone.PageNo);

                tableObj.Update(keys);
            }
        }

        public static ZonesModels GetByNo(long No)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @"SELECT * FROM Zones WHERE No = @No;";
                return cn.Query<ZonesModels>(sql, new { No = No }).FirstOrDefault();
            }
        }
        public static void MoveZone(long soucePageNo, long targetSiteID)
        {
            long creator = MemberDAO.SysCurrent.Id;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"UPDATE Zones SET SiteID={targetSiteID}  " +
                                $" WHERE PageNo = { soucePageNo }";
                conn.Execute(sql);
            }
        }
    }
}