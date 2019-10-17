using Dapper;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class MemberShipPageSetDAO
    {
        public static IEnumerable<MemberShipPageSetModel> GetItems(long SiteID, string Type = "")
        {
            string TypeCond = "";
            if(Type != "")
            {
                switch(Type)
                {
                    case "memberMenu":
                        TypeCond = "AND IsSubMenu = 0 AND IsDesktopCard = 0";
                        break;
                    case "memberSubMenu":
                        TypeCond = "AND IsSubMenu = 1";
                        break;
                    case "DesktopCard":
                        TypeCond = "AND IsDesktopCard = 1";
                        break;
                }
            }

            string sql = $"Select * From MemberShipPageSet Where SiteID = {SiteID} {TypeCond} Order By Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipPageSetModel>(sql);
            }
        }

        public static bool UpdatePageSet(long SiteID, string PageSN, bool isOpen)
        {
            string isOpenSet = "0", isNeedValueSet = "0";
            isOpenSet = isOpen ? "1" : "0";
            string sql = $"UPDATE MemberShipPageSet SET IsOpen={isOpenSet} WHERE  SiteID={SiteID} AND PageSN='{PageSN.Replace("'", "")}'";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            return true;
        }

        public static bool UpdatePageSetSort(long SiteID, string PageSN, int sort)
        {
            string sql = $"UPDATE MemberShipPageSet SET Sort={sort} WHERE  SiteID={SiteID} AND PageSN='{PageSN.Replace("'", "")}';";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            return true;
        }
    }
}