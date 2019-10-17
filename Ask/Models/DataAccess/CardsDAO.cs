using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using WorkV3.Common;
using WorkV3.Models.Interface;

namespace WorkV3.Models.DataAccess
{
    public class CardsDAO
    {
        public static List<CardsModels> GetPageData(long SiteID, long PageNO)
        {
            string Sql = "Select C.*,Z.SiteID,P.MenuID from Cards C inner join Zones Z on C.ZoneNO=Z.[No] inner join Pages P on  Z.PageNo = P.NO where Z.PageNo=@PageNO and Z.SiteID=@SiteID;";

            List<CardsModels> nLists = new List<CardsModels>();
            using (var cn = new SqlConnection(WebInfo.Conn))
            {

                var res = cn.Query<CardsModels>(
                    Sql,
                    new { SiteID = SiteID, PageNO = PageNO });
                nLists = res.ToList();
            }
            return nLists;

        }

        public static ZonesModels GetZoneInfo(long ZoneNo)
        {
            string Sql = $"Select * FROM Zones Z where [No]={ZoneNo};";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<ZonesModels>(Sql).SingleOrDefault();
            }
        }
        public static List<CardsModels> GetZoneData(long SiteID, long ZoneNo)
        {
            string Sql = "Select C.*, Z.SiteID from Cards C inner join Zones Z on C.ZoneNO=Z.[No] where Z.SiteID=@SiteID and Z.[No]=@ZoneNo;";

            List<CardsModels> nLists = new List<CardsModels>();
            using (var cn = new SqlConnection(WebInfo.Conn))
            {

                var res = cn.Query<CardsModels>(
                    Sql,
                    new { SiteID = SiteID, ZoneNo = ZoneNo });
                nLists = res.ToList();
            }
            return nLists;

        }       

        public static long GetMenuID(long cardNo) {
            string sql = $"SELECT MenuID FROM Pages WHERE [No] = (SELECT PageNo FROM Zones WHERE [No] = (SELECT ZoneNo FROM Cards WHERE [No] = { cardNo }))";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return (long)db.GetFirstValue(sql);
        }

        public static long GetListCardNo(long menuId) {
            string sql = $"SELECT No FROM Cards WHERE CardsType = 'Event' AND (ViewAction IS NULL OR ViewAction = 'List') AND ZoneNo IN (SELECT No FROM Zones WHERE PageNo IN (SELECT No FROM Pages WHERE MenuID = { menuId }))";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return (long)db.GetFirstValue(sql);
        }
        
        public static Dictionary<long, SitePage> GetPages(IEnumerable<long> cardNos) {
            if (cardNos == null || cardNos.Count() == 0)
                return null;

            string sql =
                "SELECT S.ID SiteID, S.SN SiteSN, S.Title SiteName, P.No PageNo, P.SN PageSN, P.MenuID, M.SN MenuSN, M.Title MenuName, C.No CardNo " +
                "FROM Pages P JOIN Sites S ON P.SiteID = S.ID JOIN Zones Z ON P.No = Z.PageNo JOIN Cards C ON C.ZoneNo = Z.No JOIN Menus M ON P.MenuID = M.ID " +
                $"WHERE C.No In ({ string.Join(", ", cardNos) })";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas == null || datas.Rows.Count == 0)
                return null;

            Dictionary<long, SitePage> pages = new Dictionary<long, SitePage>();
            foreach(DataRow dr in datas.Rows) {
                pages.Add((long)dr["CardNo"], new SitePage { SiteID = (long)dr["SiteID"], SiteSN = dr["SiteSN"].ToString().Trim(), SiteName = dr["SiteName"].ToString().Trim(),
                    MenuName = dr["MenuName"].ToString().Trim(), PageNo = (long)dr["PageNo"], PageSN = dr["PageSN"].ToString().Trim(), MenuID = (long)dr["MenuID"], MenuSN = dr["MenuSN"].ToString().Trim() });
            }

            return pages;
        }

        public static SitePage GetPage(long cardNo) {
            var existPages = GetPages(new long[] { cardNo });
            if (existPages!= null && existPages.Keys.Contains(cardNo))
                return existPages[cardNo];
            return null;
            //return GetPages(new long[] { cardNo })[cardNo];
        }        
    }
}