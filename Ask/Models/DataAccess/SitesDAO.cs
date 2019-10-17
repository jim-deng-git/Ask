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
    public class SitesDAO
    {

        #region GetInfo
        public static SitesModels GetSiteInfo(string SiteSN)
        {
            SitesModels nData = new SitesModels();
            string Sql = "Select * from Sites where SN=@SN ";
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<SitesModels>(
                    Sql,
                    new { SN = SiteSN });
                nData = res.FirstOrDefault();
            }
            return nData;
        }
        public static SitesModels GetInfo(long Id)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "Select * From Sites Where Id=@Id ";
            SitesModels nData = new SitesModels();
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<SitesModels>(
                    Sql,
                    new { Id = Id });
                nData = res.FirstOrDefault();
            }
            return nData;


        }

        public static List<SitesModels> GetDatas()
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "Select * From Sites WHERE IsDelete=0 ";

            List<SitesModels> nLists = new List<SitesModels>();
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<SitesModels>(
                    Sql);
                nLists = res.ToList();
            }
            if (nLists != null && nLists.Count > 0)
            {
                foreach (SitesModels item in nLists)
                {
                    string uploadImagePath = WorkV3.Golbal.UpdFileInfo.GetUPathBySiteID(item.Id, "SiteCover").TrimEnd('\\') + "\\Default.jpg";
                    if (System.IO.File.Exists(uploadImagePath))
                    {
                      item.DefaultImage = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(item.Id, "SiteCover").TrimEnd('/') + "/Default.jpg";
                    }
                    
                }
            }
            return nLists;

        }
        #endregion

        public static void SetItem(SitesModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Sites");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From Sites Where ID = " + item.Id;
            bool isNew = db.GetFirstValue(sql) == null;
            DateTime now = DateTime.Now;
            if (isNew)
            {
                if (string.IsNullOrEmpty(item.Lang))
                    tableObj["Lang"] = "zh-hant";
                if (string.IsNullOrEmpty(item.Logo))
                    tableObj["Logo"] = "";
                if (string.IsNullOrEmpty(item.Favicon))
                    tableObj["Favicon"] = "";
                if (string.IsNullOrEmpty(item.Logoshrink))
                    tableObj["Logoshrink"] = "";
                if (string.IsNullOrEmpty(item.socialSetting))
                    tableObj["socialSetting"] = "";
                if (string.IsNullOrEmpty(item.ICO))
                    tableObj["ICO"] = "";
                if (string.IsNullOrEmpty(item.GAInfo))
                    tableObj["GAInfo"] = "";
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = now;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.Id);
            }
        }
        public static void CreateSiteManagers()
        {
            List<SitesModels> sites = GetDatas();
            foreach (SitesModels site in sites)
            {
                site.CreateSiteManager();
            }
        }

        /// <summary>
        /// 網站root網址
        /// </summary>
        public static string SiteUrl { get; set; }


        public static bool IsSNExist(string ID, string SN)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"Select * From Sites Where ID <> {ID.Replace("'", "")} AND SN='{ SN.Replace("'", "")}'";
            bool isExist = db.GetFirstValue(sql) != null;
            return isExist;
        }

        public static bool CopyStructure(long sourceId, long targetId)
        {
            long creator = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;

            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                //複製選單
                long parentID = 0;
                string InsertSQL = "";
                string SelectEpaperSQL = "";
                conn.Open();
                string menuSQL = $" SELECT ID FROM Menus WHERE SiteID={sourceId} and ParentID={parentID} ";
                bool IsFail = false;
                try
                {
                    SqlCommand scmd = new SqlCommand(menuSQL, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(scmd);
                    DataTable menuTable = new DataTable();
                    sda.Fill(menuTable);

                    if (menuTable == null)
                        throw new Exception("站台不存在");

                    int exeCount;
                    //是否需要Epaper
                    /*
                    SelectEpaperSQL = $"Select SiteID from EpaperSetting Where SiteID = {sourceId}";
                    scmd.CommandText = SelectEpaperSQL;
                    int exeCount = scmd.ExecuteNonQuery();
                    if (exeCount > 0)
                    {
                        //複製網站別的東西
                        InsertSQL = @"Insert Into EpaperSetting " +
                                      $" SELECT {targetId}, EpaperDisplayName, EpaperSmtpServer, EpaperEmailAcc, EpaperEmailPwd, EpaperEmailFrom, EpaperEmailPort, EpaperEnabledSSL, EpaperTimeOut, EpaperSendFailRounds, EpaperSendIntervalMin, EpaperSendIntervalMax, EpaperOpenToPeople, EpaperOpenToMember, EpaperSubscribeLike FROM EpaperSetting WHERE SiteID={sourceId} ";
                        scmd.CommandText = InsertSQL;
                        exeCount = scmd.ExecuteNonQuery();
                        if (exeCount <= 0)
                        {
                            IsFail = true;
                        }
                    }
                    */
                    foreach (DataRow menuRow in menuTable.Rows)
                    {
                        long newMenuID = GetItem.NewSN();
                        long oldMenuID = (long)menuRow["ID"];
                        //InsertSQL = @"Insert Into ArticleSetting " +
                        //            $" SELECT {newMenuID}, '', PagingMode, PageSize, IssueSetting, SortMode, SortField, DefaultImg, ReplyStatus, ReplyTitle, ReplyPageSize, ReplyFBID, ReplyFBAccounts, ExtendReadOpen, ExtendReadTitle, ExtendReadMenus, ExtendReadMode, ExtendReadRowCount, ExtendReadOpen2, ExtendReadTitle2, ExtendReadMenus2, ExtendReadMode2, ExtendReadRowCount2, ADOpen, ADTitle, { creator }, GETDATE(), { creator }, GETDATE(), ReadMode, ReadModeSet FROM ArticleSetting WHERE MenuID={oldMenuID} " +
                        //            @"Insert Into EventSetting " +
                        //            $" SELECT {newMenuID}, '', PagingMode, PageSize, IssueSetting, SortMode, SortField, DefaultImg, ReplyStatus, ReplyTitle, ReplyPageSize, ReplyFBID, ReplyFBAccounts, ExtendReadOpen, ExtendReadTitle, ExtendReadMenus, ExtendReadMode, ADOpen, ADTitle, { creator }, GETDATE(), { creator }, GETDATE() FROM EventSetting WHERE MenuID= {oldMenuID} " +
                        //            @"Insert Into QuestionnaireSetting " +
                        //            $" SELECT {newMenuID}, Term, { creator }, GETDATE(), { creator }, GETDATE() FROM QuestionnaireSetting WHERE MenuID= {oldMenuID} " +
                        //            @"Insert Into Menus " +
                        //            $" SELECT {newMenuID}, {targetId}, {parentID}, AreaID, Title, SN, DataType, DataTypeValue, ShowStatus, Sort, { creator }, GETDATE(), { creator }, GETDATE(), DisableDelete FROM Menus WHERE ID= {oldMenuID} " +
                        //            @"Insert Into Pages " +
                        //            $" SELECT {newMenuID}, Lang, Ver, {targetId}, {newMenuID}, SN, Title, MetaDescriptions, MetaImage, MetaKeywords, ShowStatus, PublishStatus, PublishTime, StartTime, EndTime, ShowTime, { creator }, GETDATE(),  { creator }, GETDATE() FROM Pages WHERE No= {oldMenuID} ";

                        InsertSQL = $@"Insert Into ArticleSetting 
                                       SELECT {newMenuID}, '', PagingMode, PageSize, IssueSetting, SortMode, SortField, DefaultImg, ReplyStatus, ReplyTitle, ReplyPageSize, ReplyFBID, ReplyFBAccounts, ExtendReadOpen, ExtendReadTitle, ExtendReadMenus, ExtendReadMode, ExtendReadRowCount, ExtendReadOpen2, ExtendReadTitle2, ExtendReadMenus2, ExtendReadMode2, ExtendReadRowCount2, ADOpen, ADTitle, { creator }, GETDATE(), { creator }, GETDATE(), ReadMode, ReadModeSet FROM ArticleSetting WHERE MenuID={oldMenuID} 
                                       Insert Into Menus 
                                       SELECT {newMenuID}, {targetId}, {parentID}, AreaID, Title, SN, DataType, DataTypeValue, ShowStatus, Sort, { creator }, GETDATE(), { creator }, GETDATE(), DisableDelete FROM Menus WHERE ID= {oldMenuID}
                                       Insert Into Pages
                                       SELECT {newMenuID}, Lang, Ver, {targetId}, {newMenuID}, SN, Title, MetaDescriptions, MetaImage, MetaKeywords, ShowStatus, PublishStatus, PublishTime, StartTime, EndTime, ShowTime, { creator }, GETDATE(),  { creator }, GETDATE() FROM Pages WHERE No= {oldMenuID} ";


                        scmd.CommandText = InsertSQL;
                        exeCount = scmd.ExecuteNonQuery();
                        if (exeCount == 0)
                        {
                            IsFail = true;
                            break;
                        }

                        if (!IsFail)
                        {
                            if (!AddCopyChildZones(conn, sourceId, oldMenuID, targetId, newMenuID, creator))
                            {
                                IsFail = true;
                                break;
                            }
                        }
                        if (!AddCopyChildMenu(conn,  sourceId, oldMenuID, targetId, newMenuID, creator))
                        {
                            IsFail = true;
                            break;
                        }
                    }
                }

                catch (Exception exp)
                {
                    WorkLib.WriteLog.Write(true, exp.Message);
                }
                finally
                {
                    conn.Close();
                }
                return !IsFail;
            }
        }

        public static bool CopyBackendStructure(long sourceId, long targetId)
        {
            long creator = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                //複製選單
                long parentID = 0;
                string InsertSQL = "";
                conn.Open();

                string GroupPermissionInsertSQL = @"Insert Into GroupSitePermissionType " +
                                  $" SELECT {targetId}, GroupID, PermissionType  FROM GroupSitePermissionType WHERE SiteID={sourceId}; ";
                SqlCommand GroupPermissionInsertCcmd = new SqlCommand(GroupPermissionInsertSQL, conn);
                int GroupPermissionInsertCount = GroupPermissionInsertCcmd.ExecuteNonQuery();

                string menuSQL = $" SELECT ID FROM BackendMenu WHERE SiteID={sourceId} and ParentID IS NULL AND MenuClass<>9 ";
                bool IsFail = false;
                try
                {
                    SqlCommand scmd = new SqlCommand(menuSQL, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(scmd);
                    DataTable menuTable = new DataTable();
                    sda.Fill(menuTable);

                    if (menuTable != null)
                    {
                        foreach (DataRow menuRow in menuTable.Rows)
                        {
                            long oldMenuID = (long)menuRow["ID"];
                            InsertSQL = @"Insert Into BackendMenu (SiteID, SN, MenuSN, MenuClass, Icon, Title, IsLink, UrlType, UrlActionName, UrlControllerName, UrlRouteName, Url, QueryString, Sort) OUTPUT Inserted.ID " +
                                          $" SELECT {targetId}, SN, MenuSN, MenuClass, Icon, Title, IsLink, UrlType, UrlActionName, UrlControllerName, UrlRouteName, Url, QueryString, Sort FROM BackendMenu WHERE ID={oldMenuID}; ";
                            scmd.CommandText = InsertSQL;
                            var exeResult = scmd.ExecuteScalar();
                            if (exeResult == null || (long)exeResult < 0)
                                throw new Exception("無後台選單");

                            long newMenuID = (long)exeResult;
                            if (!AddCopyChildBackendMenu(conn, sourceId, newMenuID, oldMenuID, targetId, creator))
                                throw new Exception("複製後台子選單錯誤");
                        }
                    }
                }
                catch (Exception exp)
                {
                    //WorkLib.WriteLog.Write(true, exp.Message);
                    IsFail = true;
                }
                finally
                {
                    conn.Close();
                }

                return !IsFail;
            }
        }
        private static bool AddCopyChildMenu(SqlConnection conn, long sourceSiteID, long parentMenuID, long targetSiteID, long targetMenuID, long creator)
        {
            //複製選單
            string menuSQL = $" SELECT ID FROM Menus WHERE SiteID={sourceSiteID} and ParentID={parentMenuID}";
            SqlCommand scmd = new SqlCommand(menuSQL, conn);
            SqlDataAdapter sda = new SqlDataAdapter(scmd);
            DataTable menuTable = new DataTable();
            sda.Fill(menuTable);
            string InsertSQL = "";
            bool IsFail = false;
            if (menuTable != null)
            {
                foreach (DataRow menuRow in menuTable.Rows)
                {
                    long newMenuID = WorkLib.GetItem.NewSN();
                    long oldMenuID = (long)menuRow["ID"];
                    InsertSQL = @"Insert Into Menus " +
                                  $" SELECT {newMenuID}, {targetSiteID}, {targetMenuID}, AreaID, Title, SN, DataType, DataTypeValue, ShowStatus, Sort, { creator }, GETDATE(),  { creator }, GETDATE(), DisableDelete FROM Menus WHERE ID={oldMenuID}; " +
                                @"Insert Into GroupPermission (MenuType, MenuID, GroupID, SiteID, PermissionType) " +
                                  $" SELECT MenuType, {newMenuID}, GroupID, {targetSiteID}, PermissionType FROM GroupPermission WHERE ID={oldMenuID}; " +
                                @"Insert Into Pages " +
                                  $" SELECT {newMenuID}, Lang, Ver, {targetSiteID}, {newMenuID}, SN, Title, MetaDescriptions, MetaImage, MetaKeywords, ShowStatus, PublishStatus, PublishTime, StartTime, EndTime, ShowTime, { creator }, GETDATE(),  { creator }, GETDATE() FROM Pages WHERE No={oldMenuID}; ";
                    scmd.CommandText = InsertSQL;
                    int exeCount = scmd.ExecuteNonQuery();
                    if (exeCount <= 0)
                    {
                        IsFail = true;
                        break;
                    }

                    if (!IsFail)
                    {
                        if (!AddCopyChildZones(conn, sourceSiteID, oldMenuID, targetSiteID, newMenuID, creator))
                        {
                            IsFail = true;
                            break;
                        }
                    }

                    if (!IsFail)
                    {
                        if (!AddCopyChildMenu(conn, sourceSiteID, oldMenuID, targetSiteID, newMenuID, creator))
                        {
                            IsFail = true;
                            break;
                        }
                    }
                }

                if (IsFail)
                    return false;
            }
            return true;
        }

        private static bool AddCopyChildBackendMenu(SqlConnection conn, long sourceSiteID, long newParentMenuID, long oldParentMenuID, long targetSiteID, long creator)
        {
            //複製選單
            string menuSQL = $" SELECT ID FROM BackendMenu WHERE SiteID={sourceSiteID} and ParentID={oldParentMenuID} AND MenuClass<>9";
            SqlCommand scmd = new SqlCommand(menuSQL, conn);
            SqlDataAdapter sda = new SqlDataAdapter(scmd);
            DataTable menuTable = new DataTable();
            sda.Fill(menuTable);
            string InsertSQL = "";
            bool IsFail = false;
            if (menuTable != null)
            {
                foreach (DataRow menuRow in menuTable.Rows)
                {
                    long oldMenuID = (long)menuRow["ID"];
                    InsertSQL = @"Insert Into BackendMenu (ParentID, SiteID, SN, MenuSN, MenuClass, Icon, Title, IsLink, UrlType, UrlActionName, UrlControllerName, UrlRouteName, Url, QueryString, Sort) OUTPUT Inserted.ID " +
                                          $" SELECT {newParentMenuID}, {targetSiteID}, SN, MenuSN, MenuClass, Icon, Title, IsLink, UrlType, UrlActionName, UrlControllerName, UrlRouteName, Url, QueryString, Sort FROM BackendMenu WHERE ID={oldMenuID}; ";
                    scmd.CommandText = InsertSQL;
                    var exeResult = scmd.ExecuteScalar();
                    if (exeResult == null || (long)exeResult < 0)
                    {
                        IsFail = true;
                        break;
                    }
                    if (!IsFail)
                    {
                        long newMenuID = (long)exeResult;
                        if (!AddCopyChildBackendMenu(conn, sourceSiteID, newMenuID, oldMenuID, targetSiteID, creator))
                        {
                            IsFail = true;
                            break;
                        }
                    }
                }

                if (IsFail)
                    return false;
            }
            return true;
        }
        private static bool AddCopyChildZones(SqlConnection conn, long sourceSiteID, long sourcePageNo, long targetSiteID, long targetPageNo, long creator)
        {
            //複製選單
            string menuSQL = $" SELECT No FROM Zones WHERE SiteID={sourceSiteID} and PageNo={sourcePageNo}";
            SqlCommand scmd = new SqlCommand(menuSQL, conn);
            SqlDataAdapter sda = new SqlDataAdapter(scmd);
            DataTable menuTable = new DataTable();
            sda.Fill(menuTable);
            string InsertSQL = "";
            bool IsFail = false;
            if (menuTable != null)
            {
                foreach (DataRow menuRow in menuTable.Rows)
                {
                    long newZoneNo = WorkLib.GetItem.NewSN();
                    long oldZoneNo = (long)menuRow["No"];
                    InsertSQL = @"Insert Into Zones " +
                                  $" SELECT {newZoneNo}, Ver, {targetSiteID}, {targetPageNo}, SN, Sort, Background, TotalWidth, MainSpacing, SubSpacing, Boundary, ShowComputer, ShowMobile, ShowStatus, StyleID, { creator }, GETDATE(),  { creator }, GETDATE() FROM Zones WHERE No={oldZoneNo}; ";
                    scmd.CommandText = InsertSQL;
                    int exeCount = scmd.ExecuteNonQuery();
                    if (exeCount <= 0)
                    {
                        IsFail = true;
                        break;
                    }

                    if (!IsFail)
                    {
                        if (!AddCopyChildCards(conn, oldZoneNo, newZoneNo, targetPageNo, creator))
                        {
                            IsFail = true;
                            break;
                        }
                    }
                }

                if (IsFail)
                    return false;
            }
            return true;
        }
        private static bool AddCopyChildCards(SqlConnection conn, long sourceZoneNo, long targetZoneNo, long targetMenuID, long creator)
        {
            //複製選單
            string menuSQL = $" SELECT No FROM Cards WHERE ZoneNo={sourceZoneNo}";
            SqlCommand scmd = new SqlCommand(menuSQL, conn);
            SqlDataAdapter sda = new SqlDataAdapter(scmd);
            DataTable menuTable = new DataTable();
            sda.Fill(menuTable);
            string InsertSQL = "";
            bool IsFail = false;
            if (menuTable != null)
            {
                foreach (DataRow menuRow in menuTable.Rows)
                {
                    long newCardNo = WorkLib.GetItem.NewSN();
                    long newIntroNo = WorkLib.GetItem.NewSN();
                    long oldCardNo = (long)menuRow["No"];
                    InsertSQL =
                         //@"Insert Into ArticleIntro " +
                         //                 $" SELECT {newIntroNo}, {targetMenuID}, Title, IssueDate, RemarkText, Icon, IsIssue, { creator }, GETDATE(), { creator }, GETDATE(), {newCardNo} FROM ArticleIntro WHERE CardNo={oldCardNo}; " +
                         @"Insert Into ArticleSet " +
                                          $" SELECT {newCardNo}, '', '', PagingMode, PageSize, IssueSetting, SortMode, SortField, DefaultImg, { creator }, GETDATE(), { creator }, GETDATE() FROM ArticleSet WHERE CardNo={oldCardNo}; " +
                         // @"Insert Into EventSet " +
                         //                 $" SELECT {newCardNo}, '', '', PagingMode, PageSize, IssueSetting, SortMode, SortField, DefaultImg, { creator }, GETDATE(), { creator }, GETDATE() FROM EventSet WHERE CardNo={oldCardNo}; " +
                         //@"Insert Into QuestionnaireSet " +
                         //                 $" SELECT {newCardNo}, '', '', PagingMode, PageSize, IssueSetting, SortMode, SortField, DefaultImg, { creator }, GETDATE(), { creator }, GETDATE() FROM QuestionnaireSet WHERE CardNo={oldCardNo}; " +
                        @"Insert Into Cards " +
                                  $" SELECT {newCardNo}, Lang, Ver, {targetZoneNo}, SN, Title, Descriptions, CardsType, ViewAction, StartTime, EndTime, PublishTime, StylesID, Color, Status, { creator }, GETDATE(),  { creator }, GETDATE() FROM Cards WHERE No={oldCardNo}; ";
                    scmd.CommandText = InsertSQL;
                    int exeCount = scmd.ExecuteNonQuery();
                    if (exeCount <= 0)
                    {
                        IsFail = true;
                        break;
                    }
                }

                if (IsFail)
                    return false;
            }
            return true;
        }
        public static void CopyWebsiteFiles(string sourcePath, string targetPath)
        {
            if (System.IO.Directory.Exists(sourcePath))
            {
                if (!System.IO.Directory.Exists(targetPath))
                {
                    System.IO.Directory.CreateDirectory(targetPath);
                }
                CopyFiles(sourcePath, targetPath);
                string[] sourceDirs = System.IO.Directory.GetDirectories(sourcePath);
                foreach (string sourceDir in sourceDirs)
                {
                    CopyFiles(sourceDir, targetPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(sourceDir));
                }
            }
        }
        private static void CopyFiles(string sourceDirPath, string targetDirPath)
        {
            string[] sourceFiles = System.IO.Directory.GetFiles(sourceDirPath);
            if (!System.IO.Directory.Exists(targetDirPath))
                System.IO.Directory.CreateDirectory(targetDirPath);
            if (sourceFiles != null && sourceFiles.Length > 0)
            {
                foreach (string sourceFilePath in sourceFiles)
                {
                    string targetFilePath = targetDirPath + "\\" + System.IO.Path.GetFileName(sourceFilePath);
                    System.IO.File.Copy(sourceFilePath, targetFilePath);
                }
            }
        }
        private class InitBackendMenu
        {
            public int MenuClass { get; set; }
            public string Icon { get; set; }
            public string Title { get; set; }
            public bool IsLink { get; set; }
            public int UrlType { get; set; }
            public string UrlActionName { get; set; }
            public string UrlControllerName { get; set; }
            public string Url { get; set; }
            public string QueryString { get; set; }
            public int Sort { get; set; }
            public List<InitBackendMenu> ChildBackendMenu { get; set; } = new List<InitBackendMenu>();
        }
        private static List<InitBackendMenu> InitBackendMenus(bool IsCopyMember)
        {
            List<InitBackendMenu> rootList = new List<InitBackendMenu>();

            #region //網頁
            rootList.Add(new InitBackendMenu
            {
                MenuClass = 1,
                Icon = "",
                Title = "網頁",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 1
            });
            #endregion

            #region  //管理
            rootList.Add(new InitBackendMenu
            {
                MenuClass = 2,
                Icon = "",
                Title = "管理",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 2
            });
            #endregion

            #region //設定>站台> 網站設定,社群關連設定,全站SEO設定
            InitBackendMenu menu_sites = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "",
                Title = "設定",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 3
            };
            InitBackendMenu menu_sites_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-list",
                Title = "站台",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 1
            };
            InitBackendMenu menu_sites_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-laptop",
                Title = "網站設定",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Setting",
                UrlControllerName = "Sites",
                Sort = 1
            };
            menu_sites_child.ChildBackendMenu.Add(menu_sites_child_child);
            menu_sites_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-share-o",
                Title = "社群關連設定",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Index",
                UrlControllerName = "SocialSetting",
                Sort = 2
            };
            menu_sites_child.ChildBackendMenu.Add(menu_sites_child_child);
            menu_sites_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-search-text",
                Title = "全站SEO設定",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Index",
                UrlControllerName = "SiteSeoSetting",
                Sort = 3
            };
            menu_sites_child.ChildBackendMenu.Add(menu_sites_child_child);
            menu_sites.ChildBackendMenu.Add(menu_sites_child);
            rootList.Add(menu_sites);
            #endregion

            #region //行銷>數據> 流量分析> 網頁流量, 比較分析 / 站內搜尋> 熱門關鍵字設定, 搜尋關鍵字分析 / 行銷合作> LINE News RSS / 廣告> 廣告區管理, 廣告區設定
            InitBackendMenu menu_sales = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "",
                Title = "行銷",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 4
            };
            InitBackendMenu menu_sales_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-bar-chart",
                Title = "數據",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 1
            };
            #region 流量分析> 網頁流量, 比較分析
            InitBackendMenu menu_sales_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-line-chart",
                Title = "流量分析",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 1
            };
            InitBackendMenu menu_sales_child_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-line-chart",
                Title = "網頁流量",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "ListWeb",
                UrlControllerName = "Analysis",
                Sort = 1
            };
            menu_sales_child_child.ChildBackendMenu.Add(menu_sales_child_child_child);
            menu_sales_child_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-line-chart",
                Title = "比較分析",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "ListWeb",
                UrlControllerName = "Statistic",
                Sort = 2
            };
            menu_sales_child_child.ChildBackendMenu.Add(menu_sales_child_child_child);
            menu_sales_child.ChildBackendMenu.Add(menu_sales_child_child);
            #endregion
            menu_sales.ChildBackendMenu.Add(menu_sales_child);

            menu_sales_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-search-text",
                Title = "站內搜尋",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 4
            };
            #region 站內搜尋> 熱門關鍵字設定, 搜尋關鍵字分析
            menu_sales_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-search-text",
                Title = "熱門關鍵字設定",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Index",
                UrlControllerName = "Keyword",
                Sort = 1
            };
            menu_sales_child.ChildBackendMenu.Add(menu_sales_child_child);
            menu_sales_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-bar-chart",
                Title = "搜尋關鍵字分析",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Index",
                UrlControllerName = "KeywordQueried",
                Sort = 2
            };
            menu_sales_child.ChildBackendMenu.Add(menu_sales_child_child);
            #endregion
            menu_sales.ChildBackendMenu.Add(menu_sales_child);

            menu_sales_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-cursor-o",
                Title = "行銷合作",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 5
            };
            #region 行銷合作> LINE News RSS
            menu_sales_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-search-text",
                Title = "LINE News RSS",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "List",
                UrlControllerName = "CustomLineRss",
                Sort = 1
            };
            menu_sales_child.ChildBackendMenu.Add(menu_sales_child_child);
            #endregion

            menu_sales_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-store-sign",
                Title = "廣告",
                IsLink = false,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 6
            };
            #region 廣告> 廣告區管理, 廣告區設定
            menu_sales_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-store-sign",
                Title = "廣告區管理",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Index",
                UrlControllerName = "Advertisement",
                Sort = 1
            };
            menu_sales_child.ChildBackendMenu.Add(menu_sales_child_child);
            menu_sales_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-trapezoid",
                Title = "廣告區設定",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Index",
                UrlControllerName = "AdvertisementSet",
                Sort = 2
            };
            menu_sales_child.ChildBackendMenu.Add(menu_sales_child_child);
            menu_sales_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-bar-chart",
                Title = "成效分析",
                IsLink = true,
                UrlType = 0,
                UrlActionName = "",
                UrlControllerName = "",
                Sort = 3
            };
            menu_sales_child_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-lnr-bookmark",
                Title = "廣告成效",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Index",
                UrlControllerName = "AdvertisementStatistics",
                Sort = 1
            };
            menu_sales_child_child.ChildBackendMenu.Add(menu_sales_child_child_child);
            menu_sales_child_child_child = new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "cc-email-open-outline",
                Title = "廣告主統計",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "AdvertiserStatistics",
                UrlControllerName = "AdvertisementStatistics",
                Sort = 2
            };
            menu_sales_child_child.ChildBackendMenu.Add(menu_sales_child_child_child);
            menu_sales_child.ChildBackendMenu.Add(menu_sales_child_child);
            #endregion
            menu_sales.ChildBackendMenu.Add(menu_sales_child);
            rootList.Add(menu_sales);
            #endregion

            if (IsCopyMember)
            {
                #region //會員 > 會員名單, 註冊設定, 會員分析> [註冊/登入], 會員資料
                InitBackendMenu menu_member = new InitBackendMenu
                {
                    MenuClass = 3,
                    Icon = "",
                    Title = "會員",
                    IsLink = false,
                    UrlType = 0,
                    UrlActionName = "",
                    UrlControllerName = "",
                    Sort = 5
                };
                InitBackendMenu menu_member_child = new InitBackendMenu
                {
                    MenuClass = 3,
                    Icon = "cc-user-o",
                    Title = "會員名單",
                    IsLink = true,
                    UrlType = 1,
                    UrlActionName = "List",
                    UrlControllerName = "Member",
                    QueryString = "MenuID=1000",
                    Sort = 1
                };
                menu_member.ChildBackendMenu.Add(menu_member_child);
                menu_member_child = new InitBackendMenu
                {
                    MenuClass = 3,
                    Icon = "cc-trapezoid",
                    Title = "註冊設定",
                    IsLink = true,
                    UrlType = 1,
                    UrlActionName = "List",
                    UrlControllerName = "MemberShipSetting",
                    QueryString = "MenuID=1000",
                    Sort = 2
                };
                menu_member.ChildBackendMenu.Add(menu_member_child);
                menu_member_child = new InitBackendMenu
                {
                    MenuClass = 3,
                    Icon = "cc-bar-chart",
                    Title = "會員分析",
                    IsLink = false,
                    UrlType = 0,
                    UrlActionName = "",
                    UrlControllerName = "",
                    Sort = 3
                };
                InitBackendMenu menu_member_child_child = new InitBackendMenu
                {
                    MenuClass = 3,
                    Icon = "cc-user-o",
                    Title = "註冊/登入",
                    IsLink = true,
                    UrlType = 1,
                    UrlActionName = "Logs",
                    UrlControllerName = "MemberAnalysis",
                    Sort = 1
                };
                menu_member_child.ChildBackendMenu.Add(menu_member_child_child);
                menu_member_child_child = new InitBackendMenu
                {
                    MenuClass = 3,
                    Icon = "cc-user-o",
                    Title = "會員資料",
                    IsLink = true,
                    UrlType = 1,
                    UrlActionName = "FieldAnalysis",
                    UrlControllerName = "MemberAnalysis",
                    Sort = 2
                };
                menu_member_child.ChildBackendMenu.Add(menu_member_child_child);
                menu_member.ChildBackendMenu.Add(menu_member_child);
                rootList.Add(menu_member);
                #endregion
            }
            #region  //站台
            rootList.Add(new InitBackendMenu
            {
                MenuClass = 3,
                Icon = "",
                Title = "站台",
                IsLink = true,
                UrlType = 1,
                UrlActionName = "Sites",
                UrlControllerName = "Home",
                Sort = 6
            });
            #endregion
            return rootList;
        }
        private static long InsertMenu(SqlConnection conn, long? parentID, long targetSiteID, InitBackendMenu menu)
        {
            string InsertSQL = string.Format(@"INSERT [BackendMenu] ([ParentID], [SiteID], [SN], [MenuSN], [MenuClass], [Icon], [Title], [IsLink], 
                                                        [UrlType], [UrlActionName], [UrlControllerName], [UrlRouteName], [Url], [QueryString], [Sort]) OUTPUT Inserted.ID 
                                               VALUES ( {0}, {1}, NULL, NULL, {2}, '{3}', N'{4}', {5}, 
                                                        {6}, '{7}', '{8}', NULL, '{9}', '{10}', {11});",
                                  (parentID.HasValue ? parentID.Value.ToString() : "NULL"), targetSiteID.ToString(), menu.MenuClass.ToString(),
                                  menu.Icon, menu.Title, (menu.IsLink ? "1" : "0"),
                                  menu.UrlType.ToString(), menu.UrlActionName, menu.UrlControllerName, menu.Url, menu.QueryString, menu.Sort.ToString());
            SqlCommand scmd = new SqlCommand(InsertSQL, conn);
            long addID = 0;
            try
            {
                var exeResult = scmd.ExecuteScalar();
                if (exeResult == null || (long)exeResult < 0)
                {
                    return 0;
                }
                addID = (long)exeResult;
                if (menu.ChildBackendMenu.Count > 0)
                {
                    foreach (InitBackendMenu childMenu in menu.ChildBackendMenu)
                    {
                        InsertMenu(conn, addID, targetSiteID, childMenu);
                    }
                }
            }
            catch (Exception exp)
            {
                ////WorkLib.WriteLog.Write(true, exp.Message);
                //WorkLib.WriteLog.Write(true, InsertSQL);
            }
            return addID;
        }
        public static bool AddInitBackendStructure(long targetId, bool IsCopyMember)
        {
            List<InitBackendMenu> menuList = InitBackendMenus(IsCopyMember);

            long creator = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                conn.Open();
                bool IsFail = false;
                try
                { 
                    foreach (InitBackendMenu menu in menuList)
                    {
                        long menuID = InsertMenu(conn, null, targetId, menu);
                    }
                }
                catch (Exception exp)
                {
                    //WorkLib.WriteLog.Write(true, exp.Message);
                    IsFail = true;
                }
                finally
                {
                    conn.Close();
                }
                return !IsFail;
            }
        }
        public static void AddInitHomePage(long targetSiteId)
        {

            long creator = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
            WorkV3.Areas.Backend.Models.MenusModels newMenu = new WorkV3.Areas.Backend.Models.MenusModels();
            newMenu.ID = targetSiteId;
            newMenu.SiteID = targetSiteId;
            newMenu.ParentID = 0;
            newMenu.AreaID = 2;
            newMenu.Title = "首頁";
            newMenu.SN = "Index";
            newMenu.DataType = "Intro";
            newMenu.ShowStatus = 2;
            newMenu.Sort = 1;
            newMenu.Creator = creator;
            newMenu.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.MenusDAO.Insert_Single(newMenu);

            WorkV3.Areas.Backend.Models.PagesModels newPage = new WorkV3.Areas.Backend.Models.PagesModels();
            newPage.No = targetSiteId;
            newPage.Lang = "zh-Hant";
            newPage.Ver = 1;
            newPage.SiteID = targetSiteId;
            newPage.MenuID = newMenu.ID;
            newPage.SN = "Index";
            newPage.Title = "Home";
            newPage.ShowStatus = 1;
            newPage.PublishStatus = 1;
            newPage.PublishTime = DateTime.Now;
            newPage.StartTime = DateTime.Now;
            newPage.Creator = creator;
            newPage.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.SetPageInfo(newPage);

            WorkV3.Areas.Backend.Models.ZonesModels newHeaderZone = new WorkV3.Areas.Backend.Models.ZonesModels();
            newHeaderZone.No = WorkLib.GetItem.NewSN();
            newHeaderZone.Ver = 1;
            newHeaderZone.SiteID = targetSiteId;
            newHeaderZone.PageNo = newPage.No;
            newHeaderZone.SN = "";
            newHeaderZone.Sort = 1;
            newHeaderZone.Background = "";
            newHeaderZone.TotalWidth = 100;
            newHeaderZone.MainSpacing = 100;
            newHeaderZone.SubSpacing = 100;
            newHeaderZone.Boundary = 100;
            newHeaderZone.ShowComputer  = 1;
            newHeaderZone.ShowMobile = 1;
            newHeaderZone.ShowStatus = 1;
            newHeaderZone.StyleID = 1000;
            newHeaderZone.Creator = creator;
            newHeaderZone.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.SetZoneInfo(newHeaderZone);

            WorkV3.Areas.Backend.Models.CardsModels newHeaderCard = new WorkV3.Areas.Backend.Models.CardsModels();
            newHeaderCard.No = WorkLib.GetItem.NewSN();
            newHeaderCard.Lang = newPage.Lang;
            newHeaderCard.Ver = 1;
            newHeaderCard.ZoneNo = newHeaderZone.No;
            newHeaderCard.SN = "";
            newHeaderCard.Title = "Header";
            newHeaderCard.Descriptions = "";
            newHeaderCard.CardsType = "Header";
            newHeaderCard.StylesID = 2;
            newHeaderCard.Color = 0;
            newHeaderCard.Status = 1;
            newHeaderCard.Creator = creator;
            newHeaderCard.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.SetCardInfo(newHeaderCard);

            WorkV3.Areas.Backend.Models.ZonesModels newFooterZone = new WorkV3.Areas.Backend.Models.ZonesModels();
            newFooterZone.No = WorkLib.GetItem.NewSN();
            newFooterZone.Ver = 1;
            newFooterZone.SiteID = targetSiteId;
            newFooterZone.PageNo = newPage.No;
            newFooterZone.SN = "";
            newFooterZone.Sort = 99;
            newFooterZone.Background = "";
            newFooterZone.TotalWidth = 100;
            newFooterZone.MainSpacing = 100;
            newFooterZone.SubSpacing = 100;
            newFooterZone.Boundary = 100;
            newFooterZone.ShowComputer = 1;
            newFooterZone.ShowMobile = 1;
            newFooterZone.ShowStatus = 1;
            newFooterZone.StyleID = 1001;
            newFooterZone.Creator = creator;
            newFooterZone.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.SetZoneInfo(newFooterZone);

            WorkV3.Areas.Backend.Models.CardsModels newFooterCard = new WorkV3.Areas.Backend.Models.CardsModels();
            newFooterCard.No = WorkLib.GetItem.NewSN();
            newFooterCard.Lang = newPage.Lang;
            newFooterCard.Ver = 1;
            newFooterCard.ZoneNo = newFooterZone.No;
            newFooterCard.SN = "";
            newFooterCard.Title = "Footer";
            newFooterCard.Descriptions = "";
            newFooterCard.CardsType = "Footer";
            newFooterCard.StylesID = 4;
            newFooterCard.Color = 0;
            newFooterCard.Status = 1;
            newFooterCard.Creator = creator;
            newFooterCard.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.SetCardInfo(newFooterCard);
        }
        private class DetaultPageInfos
        {
            public string Title { get; set; }
            public string CardsType { get; set; }
            public string ViewAction { get; set; }
        }
        public static void AddInitSystemPages(long targetSiteId)
        {
            string defaultLang = "zh-Hant";
            long creator = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;

            //會員的選單
            long newMenuberMenuNo = WorkLib.GetItem.NewSN();
            AddMenu(targetSiteId, newMenuberMenuNo, 1, "Member", "會員", "Member", 1, creator);
            Dictionary<string, DetaultPageInfos> MemberPages = new Dictionary<string, DetaultPageInfos>();
            MemberPages.Add("Login", new DetaultPageInfos(){Title="會員登入",CardsType ="Member", ViewAction= "Login"});
            MemberPages.Add("MyInfo", new DetaultPageInfos() { Title = "個人設定", CardsType = "Member", ViewAction = "MyInfo" });
            MemberPages.Add("register", new DetaultPageInfos() { Title = "會員註冊", CardsType = "Member", ViewAction = "Register" });
            MemberPages.Add("verify", new DetaultPageInfos() { Title = "會員註冊驗證", CardsType = "Member", ViewAction = "Verify" });
            MemberPages.Add("forget", new DetaultPageInfos() { Title = "忘記密碼", CardsType = "Member", ViewAction = "Forget" });
            MemberPages.Add("fb", new DetaultPageInfos() { Title = "FB 會員登入", CardsType = "Member", ViewAction = "FB" });
            MemberPages.Add("resetpwd", new DetaultPageInfos() { Title = "重設密碼", CardsType = "Member", ViewAction = "ResetPwd" });
            MemberPages.Add("MySocialInfo", new DetaultPageInfos() { Title = "會員基本資料", CardsType = "Member", ViewAction = "MySocialInfo" });
            MemberPages.Add("SecurityDesc", new DetaultPageInfos() { Title = "會員同意條款說明", CardsType = "Member", ViewAction = "SecurityDesc" });
            MemberPages.Add("ResentMail", new DetaultPageInfos() { Title = "重發驗證信", CardsType = "Member", ViewAction = "ResentMail" });
            MemberPages.Add("ChangePWD", new DetaultPageInfos() { Title = "會員變更密碼", CardsType = "Member", ViewAction = "ChangePWD" });
            MemberPages.Add("Collection", new DetaultPageInfos() { Title = "會員收藏", CardsType = "Member", ViewAction = "Collection" });
            MemberPages.Add("Desktop", new DetaultPageInfos() { Title = "會員主頁", CardsType = "Member", ViewAction = "Desktop" });
            MemberPages.Add("CollectionList", new DetaultPageInfos() { Title = "你的收藏", CardsType = "Member", ViewAction = "CollectionList" });
            MemberPages.Add("LikeEventCalendar", new DetaultPageInfos() { Title = "你的收藏", CardsType = "Member", ViewAction = "LikeEventCalendar" });
            MemberPages.Add("Point", new DetaultPageInfos() { Title = "我的點數", CardsType = "Member", ViewAction = "Point" });
            //會員的相關頁面
            foreach (string memberPageKey in MemberPages.Keys)
            {
                long newPageNo = WorkLib.GetItem.NewSN();
                AddPage(targetSiteId, newMenuberMenuNo, newPageNo, MemberPages[memberPageKey].ViewAction, MemberPages[memberPageKey].Title, defaultLang, creator);
                AddHeaderZoneCards(targetSiteId, newPageNo, defaultLang, creator);
                AddBreadZoneCards(targetSiteId, newPageNo, defaultLang, creator);
                AddContentZoneCards(targetSiteId, newPageNo, defaultLang, MemberPages[memberPageKey].Title, MemberPages[memberPageKey].CardsType, MemberPages[memberPageKey].ViewAction, creator);
                AddFooterZoneCards(targetSiteId, newPageNo, defaultLang, creator);
            }

            long newSearchMenuNo = WorkLib.GetItem.NewSN();
            AddMenu(targetSiteId, newSearchMenuNo, 1, "SearchResults", "搜尋", "SearchResults", 4, creator);
            Dictionary<string, DetaultPageInfos> SearchPages = new Dictionary<string, DetaultPageInfos>();
            SearchPages.Add("SearchResults", new DetaultPageInfos() { Title = "搜尋結果", CardsType = "SearchResults", ViewAction = "" });
            //搜尋的相關頁面
            foreach (string searchPageKey in SearchPages.Keys)
            {
                long newPageNo = WorkLib.GetItem.NewSN();
                AddPage(targetSiteId, newSearchMenuNo, newPageNo, SearchPages[searchPageKey].ViewAction, SearchPages[searchPageKey].Title, defaultLang, creator);
                AddHeaderZoneCards(targetSiteId, newPageNo, defaultLang, creator);
                AddBreadZoneCards(targetSiteId, newPageNo, defaultLang, creator);
                AddContentZoneCards(targetSiteId, newPageNo, defaultLang, SearchPages[searchPageKey].Title, SearchPages[searchPageKey].CardsType, SearchPages[searchPageKey].ViewAction, creator);
                AddFooterZoneCards(targetSiteId, newPageNo, defaultLang, creator);
            }
        }
        #region 新增預設頁的共用方法 AddMenu, AddPage, AddHeaderZoneCards, AddBreadZoneCards, AddContentZoneCards, AddFooterZoneCards
        private static void AddMenu(long targetSiteId, long menuID, int AreaID, string SN, string Title, string DataType, int Sort, long creator)
        {
            WorkV3.Areas.Backend.Models.MenusModels newMenu = new WorkV3.Areas.Backend.Models.MenusModels();
            newMenu.ID = menuID;
            newMenu.SiteID = targetSiteId;
            newMenu.ParentID = 0;
            newMenu.AreaID = (byte)AreaID;
            newMenu.Title = Title;
            newMenu.SN = SN;
            newMenu.DataType = DataType;
            newMenu.ShowStatus = 1;
            newMenu.Sort = Sort;
            newMenu.Creator = creator;
            newMenu.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.MenusDAO.Insert_Single(newMenu);
        }
        private static void AddPage(long targetSiteId, long menuID, long pageNo, string SN, string Title, string Lang, long creator)
        {
            WorkV3.Areas.Backend.Models.PagesModels newPage = new WorkV3.Areas.Backend.Models.PagesModels();
            newPage.No = pageNo;
            newPage.Lang = Lang;
            newPage.Ver = 1;
            newPage.SiteID = targetSiteId;
            newPage.MenuID = menuID;
            newPage.SN = SN;
            newPage.Title = Title;
            newPage.ShowStatus = 1;
            newPage.PublishStatus = 1;
            newPage.PublishTime = DateTime.Now;
            newPage.StartTime = DateTime.Now;
            newPage.Creator = creator;
            newPage.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.SetPageInfo(newPage);
        }
        private static void AddHeaderZoneCards(long targetSiteId, long pageNo, string pageLang, long creator)
        {
            #region header zones, cards
            WorkV3.Areas.Backend.Models.ZonesModels newHeaderZone = new WorkV3.Areas.Backend.Models.ZonesModels();
            newHeaderZone.No = WorkLib.GetItem.NewSN();
            newHeaderZone.Ver = 1;
            newHeaderZone.SiteID = targetSiteId;
            newHeaderZone.PageNo = pageNo;
            newHeaderZone.Sort = 1;
            newHeaderZone.Background = "";
            newHeaderZone.TotalWidth = 100;
            newHeaderZone.MainSpacing = 100;
            newHeaderZone.SubSpacing = 100;
            newHeaderZone.Boundary = 100;
            newHeaderZone.ShowComputer = 1;
            newHeaderZone.ShowMobile = 1;
            newHeaderZone.ShowStatus = 1;
            newHeaderZone.StyleID = 1000;
            newHeaderZone.Creator = creator;
            newHeaderZone.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.SetZoneInfo(newHeaderZone);

            WorkV3.Areas.Backend.Models.CardsModels newHeaderCard = new WorkV3.Areas.Backend.Models.CardsModels();
            newHeaderCard.No = WorkLib.GetItem.NewSN();
            newHeaderCard.Lang = pageLang;
            newHeaderCard.Ver = 1;
            newHeaderCard.ZoneNo = newHeaderZone.No;
            newHeaderCard.SN = "";
            newHeaderCard.Title = "Header";
            newHeaderCard.Descriptions = "";
            newHeaderCard.CardsType = "Header";
            newHeaderCard.StylesID = 2;
            newHeaderCard.Color = 0;
            newHeaderCard.Status = 1;
            newHeaderCard.Creator = creator;
            newHeaderCard.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.SetCardInfo(newHeaderCard);
            #endregion
        }
        private static void AddBreadZoneCards(long targetSiteId, long pageNo, string pageLang, long creator)
        {
            #region bread zones, cards
            WorkV3.Areas.Backend.Models.ZonesModels newBreadZone = new WorkV3.Areas.Backend.Models.ZonesModels();
            newBreadZone.No = WorkLib.GetItem.NewSN();
            newBreadZone.Ver = 1;
            newBreadZone.SiteID = targetSiteId;
            newBreadZone.PageNo = pageNo;
            newBreadZone.Sort = 2;
            newBreadZone.Background = "";
            newBreadZone.TotalWidth = 100;
            newBreadZone.MainSpacing = 100;
            newBreadZone.SubSpacing = 100;
            newBreadZone.Boundary = 100;
            newBreadZone.ShowComputer = 1;
            newBreadZone.ShowMobile = 1;
            newBreadZone.ShowStatus = 1;
            newBreadZone.StyleID = 1002;
            newBreadZone.Creator = creator;
            newBreadZone.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.SetZoneInfo(newBreadZone);

            WorkV3.Areas.Backend.Models.CardsModels newBreadCard = new WorkV3.Areas.Backend.Models.CardsModels();
            newBreadCard.No = WorkLib.GetItem.NewSN();
            newBreadCard.Lang = pageLang;
            newBreadCard.Ver = 1;
            newBreadCard.ZoneNo = newBreadZone.No;
            newBreadCard.SN = "";
            newBreadCard.Title = "Bread";
            newBreadCard.Descriptions = "";
            newBreadCard.CardsType = "BreadCrumbs";
            newBreadCard.StylesID = 1;
            newBreadCard.Color = 0;
            newBreadCard.Status = 1;
            newBreadCard.Creator = creator;
            newBreadCard.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.SetCardInfo(newBreadCard);
            #endregion
        }
        private static void AddContentZoneCards(long targetSiteId, long pageNo, string pageLang, string Title, string CardsType, string ViewAction, long creator)
        {
            #region content zones, cards
            WorkV3.Areas.Backend.Models.ZonesModels newContentZone = new WorkV3.Areas.Backend.Models.ZonesModels();
            newContentZone.No = WorkLib.GetItem.NewSN();
            newContentZone.Ver = 1;
            newContentZone.SiteID = targetSiteId;
            newContentZone.PageNo = pageNo;
            newContentZone.Sort = 5;
            newContentZone.Background = "";
            newContentZone.TotalWidth = 100;
            newContentZone.MainSpacing = 100;
            newContentZone.SubSpacing = 100;
            newContentZone.Boundary = 100;
            newContentZone.ShowComputer = 1;
            newContentZone.ShowMobile = 1;
            newContentZone.ShowStatus = 1;
            newContentZone.StyleID = 1;
            newContentZone.Creator = creator;
            newContentZone.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.SetZoneInfo(newContentZone);

            WorkV3.Areas.Backend.Models.CardsModels newContentCard = new WorkV3.Areas.Backend.Models.CardsModels();
            newContentCard.No = WorkLib.GetItem.NewSN();
            newContentCard.Lang = pageLang;
            newContentCard.Ver = 1;
            newContentCard.ZoneNo = newContentZone.No;
            newContentCard.SN = "";
            newContentCard.Title = Title;
            newContentCard.Descriptions = "";
            newContentCard.CardsType = CardsType;
            newContentCard.ViewAction = ViewAction;
            newContentCard.StylesID = 1;
            newContentCard.Color = 0;
            newContentCard.Status = 1;
            newContentCard.Creator = creator;
            newContentCard.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.SetCardInfo(newContentCard);
            #endregion
        }
        private static void AddFooterZoneCards(long targetSiteId, long pageNo, string pageLang, long creator)
        {
            #region footer zones, cards
            WorkV3.Areas.Backend.Models.ZonesModels newFooterZone = new WorkV3.Areas.Backend.Models.ZonesModels();
            newFooterZone.No = WorkLib.GetItem.NewSN();
            newFooterZone.Ver = 1;
            newFooterZone.SiteID = targetSiteId;
            newFooterZone.PageNo = pageNo;
            newFooterZone.SN = "";
            newFooterZone.Sort = 99;
            newFooterZone.Background = "";
            newFooterZone.TotalWidth = 100;
            newFooterZone.MainSpacing = 100;
            newFooterZone.SubSpacing = 100;
            newFooterZone.Boundary = 100;
            newFooterZone.ShowComputer = 1;
            newFooterZone.ShowMobile = 1;
            newFooterZone.ShowStatus = 1;
            newFooterZone.StyleID = 1001;
            newFooterZone.Creator = creator;
            newFooterZone.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.SetZoneInfo(newFooterZone);

            WorkV3.Areas.Backend.Models.CardsModels newFooterCard = new WorkV3.Areas.Backend.Models.CardsModels();
            newFooterCard.No = WorkLib.GetItem.NewSN();
            newFooterCard.Lang = pageLang;
            newFooterCard.Ver = 1;
            newFooterCard.ZoneNo = newFooterZone.No;
            newFooterCard.Title = "Footer";
            newFooterCard.Descriptions = "";
            newFooterCard.CardsType = "Footer";
            newFooterCard.StylesID = 4;
            newFooterCard.Color = 0;
            newFooterCard.Status = 1;
            newFooterCard.Creator = creator;
            newFooterCard.CreateTime = DateTime.Now;
            WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.SetCardInfo(newFooterCard);
            #endregion
        }
        #endregion
        public static bool Delete(long siteID, string sourcePath)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                conn.Open();
                string sql = @" Delete Sites Where ID=@ID; 
                                Delete Menus WHERE SiteID=@ID; 
                                Delete Cards Where ZoneNo IN ( SELECT No FROM Zones Where PageNo IN (SELECT No FROM  Pages WHERE SiteID=@ID)); 
                                Delete Zones Where PageNo IN (SELECT No FROM  Pages WHERE SiteID=@ID); 
                                Delete Pages WHERE SiteID=@ID";
                bool IsFail = false;
                try
                {
                    conn.Execute(sql, new { ID = siteID });
                    if (System.IO.Directory.Exists(sourcePath))
                    {
                        System.IO.Directory.Delete(sourcePath,true);
                    }
                }
                catch (Exception exp)
                {
                    WorkLib.WriteLog.Write(true, exp.Message);
                    IsFail = true;
                }
                finally
                {
                    conn.Close();
                }
                return !IsFail;
            }
        }

    }
}