using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using WorkV3.Common;

namespace WorkV3.Models.DataAccess
{
    public class MenusDAO
    {
        #region  選單
        public static List<MenusModels>GetFrontMenus(long SiteID) {

            string Sql = "Select " +
             " M.ID,M.SiteID,M.ParentID,M.AreaID,M.Title,M.SN MenuSN, M.DataType,M.DataTypeValue,M.ShowStatus, M.Sort, " +
             " CT.Code MenuCode, " +
             " P.SN PageSN, " +
             " RL.LinkInfo, RL.ClickType AS RLClickType ,RL.IsOpenNew " +
             " ,RF.ClickType AS RFClickType,RF.FileInfo " +
             " from Menus M " +
             " inner join CardsType CT on DataType = CT.Code " +
             " left join Pages P on P.SiteID = P.SiteID and P.MenuID = M.ID and P.SN = M.SN " +
             " left join[ResourceLinks] RL on RL.SourceType = 1 and RL.SourceNo = M.ID and RL.SiteID = M.SiteID " +
             " left join[ResourceFiles] RF on RF.SourceType = 1 and RF.SourceNo = M.ID and RL.SiteID = M.SiteID " +
             " where M.SiteID = @SiteID and M.ShowStatus=1 order by Sort ";


            List<MenusModels> nLists = new List<MenusModels>();
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<MenusModels>(
                    Sql,
                    new { SiteID = SiteID });
                nLists = res.ToList();
            }
            return nLists;


        }

        #endregion

        public static MenusModels GetInfo(long SiteID, long Id)
        {

            string Sql = "Select *, S.SN SiteSN From Menus M Inner join Sites S on S.ID =M.SiteID where SiteID={0} and M.ID={1}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable dt = db.GetDataTable(string.Format(Sql, SiteID, Id));
            MenusModels _TempRow = new MenusModels();
            if (dt.Rows.Count > 0)
            {
                _TempRow = CreateData(dt.Rows[0]);
            }

            return _TempRow;
        }

        public static MenusModels GetInfo(long Id)
        {

            string Sql = "Select *, S.SN SiteSN From Menus M Inner join Sites S on S.ID =M.SiteID where M.ID={0}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable dt = db.GetDataTable(string.Format(Sql, Id));
            MenusModels _TempRow = new MenusModels();
            if (dt.Rows.Count > 0)
            {
                _TempRow = CreateData(dt.Rows[0]);
            }

            return _TempRow;
        }
        public static MenusModels GetInfo(long siteId, string menuSn) {
            if (string.IsNullOrWhiteSpace(menuSn))
                return null;

            string Sql = "Select *, S.SN SiteSN From Menus M Inner join Sites S on S.ID =M.SiteID where SiteID = {0} AND M.SN = N'{1}'";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable dt = db.GetDataTable(string.Format(Sql, siteId, menuSn.Replace("'", "''")));
            MenusModels _TempRow = new MenusModels();
            if (dt.Rows.Count > 0) {
                _TempRow = CreateData(dt.Rows[0]);
            }

            return _TempRow;
        }


        #region CreateDataRow


        private static MenusModels CreateData(DataRow dr)
        {

            MenusModels nData = new MenusModels
            {
                Id = (long)dr["id"],
                SiteID = (long)dr["SiteID"],
                SiteSN = dr["SiteSN"].ToString().Trim(),
                ParentID = (long)dr["ParentID"],
                AreaID = (byte)dr["AreaID"],
                Title = dr["Title"].ToString().Trim(),
                SN = dr["SN"].ToString().Trim(),
                DataType = dr["DataType"].ToString().Trim(),
                DataTypeValue = dr["DataTypeValue"].ToString().Trim(),
                ShowStatus = (byte)dr["ShowStatus"],
                Sort = (int)dr["Sort"],
                Creator = (long)dr["Creator"],
                CreateTime = dr["CreateTime"] as DateTime?,
                Modifier = dr["Modifier"] as long?,
                ModifyTime = dr["ModifyTime"] as DateTime?,
            };

            return nData;
        }
        #endregion

        public static SitePage GetListPage(long menuId) {
            string sql =
                "SELECT TOP(1) S.ID SiteID, S.SN SiteSN, P.No PageNo, P.SN PageSN " +
                "FROM Pages P JOIN Sites S ON P.SiteID = S.ID JOIN Zones Z ON P.No = Z.PageNo JOIN Cards C ON Z.No = C.ZoneNo " +
                $"WHERE P.MenuID = { menuId } AND (C.ViewAction IS NULL OR C.ViewAction = 'List') " +
                "ORDER BY ISNULL(C.ViewAction, 'ZZZZZ')";
            
            using(SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<SitePage>(sql).SingleOrDefault();
            }
        }

        public static long? GetFirstCardNo(long menuId, string CardsType)
        {
            string sql =
                "SELECT TOP(1) C.No FROM Pages P JOIN Zones Z ON P.No = Z.PageNo JOIN Cards C ON Z.No = C.ZoneNo " +
                "WHERE P.MenuID = " + menuId +
                " and C.CardsType='" + CardsType + "'" +
                "ORDER BY CASE WHEN C.ViewAction = 'List' THEN 1 WHEN C.ViewAction IS NULL THEN 2 WHEN C.ViewAction = 'Content' THEN 3 ELSE 4 END";

            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.ExecuteScalar<long?>(sql);
            }
        }

        public static IEnumerable<IDValue> GetListCards(string cardType) {
            string sql =
                "SELECT DISTINCT M.ID, M.Title Value, M.SiteID FROM Menus M JOIN Pages P ON M.ID = P.MenuID JOIN Zones Z ON Z.PageNo = P.No JOIN Cards C ON C.ZoneNo = Z.No " +
                $"WHERE C.CardsType IN('{ cardType.Replace("'", "''") }') AND(C.ViewAction IS NULL OR C.ViewAction = 'List')";

            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<IDValue>(sql);
            }
        }

        public static List<BreadCrumbsModels> GetBreadCrumbs(long siteId, long menuId, long pageNo)
        {
            string Sql = "WITH Dep_Menu (ParentID,ID,SiteID,Title,DEP_PATH,DEP_SN,DEP_LEVEL) AS" +
                     "(" +
                     "    select ParentID,ID,Menus.SiteID,Menus.Title,CAST(Menus.Title as nvarchar(MAX)),ISNULL(CAST(Menus.SN as nvarchar(MAX)),''),0 AS DT_LEVEL" +
                     "    from Menus " +
                     "    where ParentID = 0" +
                     "    union all" +
                     "    select D.ParentID,D.ID,D.SiteID,D.Title,CAST(DC.DEP_PATH + ',' + D.Title as nvarchar(MAX)),CAST(DEP_SN + ',' + ISNULL(D.SN,'') as nvarchar(MAX)), DC.DEP_LEVEL + 1" +
                     "    from Menus D INNER JOIN Dep_Menu DC on D.ParentID=DC.ID" +
                     ")" +
                     " select Pages.[No] as PagesNo, Pages.[Title] as PagesTitle, Dep_Menu.* from Pages left join Dep_Menu on Pages.MenuID = Dep_Menu.ID " +
                     " where Pages.SiteID = '{0}' and Pages.MenuID = '{1}' and Pages.[No] = '{2}'  ";

            List<BreadCrumbsModels> nLists = new List<BreadCrumbsModels>();
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<BreadCrumbsModels>(string.Format(Sql, siteId, menuId, pageNo));
                nLists = res.ToList();
            }
            return nLists;
        }

        public static IEnumerable<MenusModels> GetFormAndQuestionnaireMenus(long siteId) {
            string sql = "SELECT * FROM Menus WHERE DataType IN('Form', 'Questionnaire') AND SiteID = " + siteId;
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<MenusModels>(sql);
            }
        }
    }
}