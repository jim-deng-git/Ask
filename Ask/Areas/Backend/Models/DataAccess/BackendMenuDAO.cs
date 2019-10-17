using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using WorkV3.Areas.Backend.Abstracts;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class BackendMenuDAO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="menuClass"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Menu> GetChildren(long ID, int menuClass = 3)
        {
            if (ID == 0)
                return new List<Menu>();

            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@ParentID", ID);

                string sql = "";
                IEnumerable<Menu> retValue = new List<Menu>();
                switch (menuClass)
                {
                    case 1:

                        long siteId = BackendMenuDAO.GetSiteId(ID);
                        IEnumerable<MenusModels> submenu = MenusDAO.GetRootDataBySite(siteId);

                        foreach (MenusModels item in submenu)
                        {
                            item.GetChildren();
                        }

                        List<Menu> childMenu = new List<Menu>();
                        MenusModels lv1Menu = new MenusModels();
                        MenusModels lv2Menu = new MenusModels();
                        MenusModels lv3Menu = new MenusModels();

                        lv1Menu.Type = 1;
                        lv1Menu.Title = "上導覽列";
                        lv1Menu.ID = long.MaxValue;
                        lv1Menu.Children = submenu.Where(m => m.AreaID == 1);
                        lv2Menu.Type = 1;
                        lv2Menu.Title = "主選單";
                        lv2Menu.ID = long.MaxValue - 1;
                        lv2Menu.Children = submenu.Where(m => m.AreaID == 2);
                        lv3Menu.Type = 1;
                        lv3Menu.Title = "下導覽列";
                        lv3Menu.ID = long.MaxValue - 2;
                        lv3Menu.Children = submenu.Where(m => m.AreaID == 3);

                        if (lv1Menu.Children.Count() > 0)
                            childMenu.Add(lv1Menu);
                        if (lv2Menu.Children.Count() > 0)
                            childMenu.Add(lv2Menu);
                        if (lv3Menu.Children.Count() > 0)
                            childMenu.Add(lv3Menu);

                        return childMenu;

                    case 2:
                    case 3:
                    default:
                        sql = @" SELECT * 
                                 FROM BackendMenu
                                 WHERE ParentID = @ParentID 
                                 ORDER BY Sort ";

                        retValue = conn.Query<BackendMenuModel>(sql, param);
                        foreach (BackendMenuModel menu in retValue)
                        {
                            menu.GetChildren();
                        }
                        return retValue;

                }
            }
        }

        public static long GetSiteId(long Id)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT SiteId 
                                FROM BackendMenu
                                WHERE ID = @Id ";

                return conn.Query<long>(sql, new { Id = Id }).FirstOrDefault();
            }
        }

        //預設 SN 是 Unique，所以只取一個
        public static BackendMenuModel GetItemBySn(string sn)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT * FROM BackendMenu WHERE SN = @SN ";

                return conn.Query<BackendMenuModel>(sql, new { SN = sn }).FirstOrDefault();
            }
        }

        public static long GetIdBySn(string sn)
        {
            return GetItemBySn(sn).ID;
        }

        public static IEnumerable<Menu> GetRoots(long? siteId = null)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string whereClause = "";

                if (siteId != null)
                {
                    whereClause = " AND [SiteID] = @SiteID ";
                    param.Add("@SiteID", siteId);
                }
                string sql = $@" SELECT * 
                                 FROM BackendMenu
                                 WHERE MenuClass<>9 AND ParentID IS NULL {whereClause}
                                 ORDER BY SiteID, Sort ";

                IEnumerable<Menu> retValue = conn.Query<BackendMenuModel>(sql, param);
                foreach (Menu menu in retValue)
                {
                    menu.GetChildren();
                }
                return retValue;
            }
        }
        public static IEnumerable<Menu> GetManagerRoots()
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string whereClause = " AND [MenuClass] = 9 ";
                string sql = $@" SELECT * 
                                 FROM BackendMenu
                                 WHERE ParentID IS NULL {whereClause}
                                 ORDER BY Sort ";

                IEnumerable<Menu> retValue = conn.Query<BackendMenuModel>(sql, param);
                foreach (Menu menu in retValue)
                {
                    menu.GetChildren();
                }
                return retValue;
            }
        }

        public static BackendMenuModel GetItem(long ID)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@ID", ID);
                string sql = @" SELECT * 
                                FROM BackendMenu
                                WHERE ID = @ID ";

                BackendMenuModel retValue = conn.Query<BackendMenuModel>(sql, param).SingleOrDefault();
                retValue.Children = BackendMenuDAO.GetChildren(ID);
                return retValue;
            }
        }
    }
}