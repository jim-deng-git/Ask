using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using WorkV3.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace WorkV3.Models.DataAccess
{
    public class SiteLangMenuDAO
    {
        public static List<SiteLangMenuModel> GetDatas(long SiteID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "Select * From SiteLangMenu where SiteID=@SiteID ORDER BY Sort ";

            List<SiteLangMenuModel> nLists = new List<SiteLangMenuModel>();
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<SiteLangMenuModel>(
                   Sql,
                   new { SiteID = SiteID });
                nLists = res.ToList();
            }
            return nLists;

        }
        public static SiteLangMenuModel GetItem(long id)
        {
            string sql = "Select * From SiteLangMenu Where ID = " + id;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<SiteLangMenuModel>(sql).SingleOrDefault();
            }
        }

        public static void SetItem(SiteLangMenuModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("SiteLangMenu");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From SiteLangMenu Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;
                tableObj["Sort"] = 1;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }

        public static void Sort(long SiteID, IEnumerable<WorkV3.Common.SortItem> items)
        {
            CommonDA.Sort("SiteLangMenu", items, "SiteID = " + SiteID);
        }

        public static int Delete(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "Delete SiteLangMenu Where ID In ({0})\n" ;

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }
            return num;
        }

    }
}