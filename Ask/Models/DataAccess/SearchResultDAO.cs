using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public static class SearchResultDAO
    {
        public static WorkV3.Common.SitePage GetSearchPage(long siteId) {
            string sql = $"SELECT No FROM Cards WHERE CardsType = 'SearchResults' AND Status = 1 AND ZoneNo IN (SELECT No FROM Zones WHERE SiteID = { siteId })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            long? cardNo = db.GetFirstValue(sql) as long?;
            if (cardNo == null)
                return null;

            return CardsDAO.GetPage((long)cardNo);
        } 

        public static IEnumerable<SearchMenuModel> GetSearchMenus(long siteId) {
            string sql = 
                "SELECT ID, Title, DataType Module, ParentID, " +
                "   CASE DataType " +
                "       WHEN 'Article' THEN (SELECT DefaultImg  FROM ArticleSetting WHERE MenuID = M.ID) " +
               // "       WHEN 'Event' THEN (SELECT DefaultImg  FROM EventSetting WHERE MenuID = M.ID) " +  //Joe 20190916尚無此資料表故先註解
                "   ELSE NULL END Icon " +
                $"FROM Menus M WHERE SiteID = { siteId } AND ShowStatus = 1 AND DataType IN ('ArticleIntro', 'Article', 'Event', 'CustomShops') " +
                "ORDER BY (CASE AreaID WHEN 2 THEN 1 WHEN 1 THEN 2 ELSE AreaID END), Sort";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);

            List<SearchMenuModel> menus = new List<SearchMenuModel>();
            foreach(DataRow dr in datas.Rows) {
                string iconJson = dr["Icon"].ToString().Trim();
                string icon = (iconJson == string.Empty) ? string.Empty : Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(iconJson).Img;
                long id = (long)dr["ID"];
                
                menus.Add(new SearchMenuModel {
                    ID = id,
                    ParentID = (long)dr["ParentID"],
                    Module = dr["Module"].ToString().Trim(),
                    Title = dr["Title"].ToString().Trim(),
                    UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, id).TrimEnd('/') + "/",
                    Icon = icon
                });
            }
            
            return menus;
        }
        
        /// <summary>
        /// 返回指定 parentId 及上層選單的相關信息
        /// </summary>
        /// <param name="parentMenuIds">指定的 parentId</param>
        /// <returns>返回一個字符串數組列表，每個 Item 是一個數組，[0] 表示 parentMenuId，[1] 表示 parentMenuName，[2] 表示 parentMenuId 再上一級選單的名稱</returns>
        public static List<string[]> GetRootMenus(IEnumerable<long> parentMenuIds) {
            if (parentMenuIds?.Count() == 0)
                return null;
            
            string sql = $"Select ID, Title, ParentID From Menus Where ID In ({ string.Join(", ", parentMenuIds.Distinct()) })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);

            List<string[]> rootMenus = new List<string[]>();
            List<long> rootMenuIds = new List<long>();
            foreach (DataRow dr in datas.Rows) {
                long id = (long)dr["ID"];
                long parentId = (long)dr["ParentID"];
                rootMenus.Add(new string[] { id.ToString(), dr["Title"].ToString().Trim(), parentId == 0 ? null : parentId.ToString() });

                if (parentId != 0)
                    rootMenuIds.Add(parentId);
            }

            if(rootMenuIds.Count > 0) {
                sql = $"Select ID, Title From Menus Where ID In ({ string.Join(", ", rootMenuIds.Distinct()) })";
                foreach(DataRow dr in db.GetDataTable(sql).Rows) {
                    string id = dr["ID"].ToString().Trim();
                    string title = dr["Title"].ToString().Trim();
                    foreach(string[] item in rootMenus) {
                        if (item[2] == id)
                            item[2] = title;
                    }
                }
            }

            return rootMenus;
        }

        #region GetItems
        public static IEnumerable<SearchResultModel> GetItems(IEnumerable<SearchMenuModel> menus, string key)
        {


            if (menus?.Count() == 0 || string.IsNullOrWhiteSpace(key))
                return new List<SearchResultModel>();

            List<string> sql = new List<string>();

            IEnumerable<long> articleIntroMenuIds = menus.Where(m => m.Module.ToLower() == "articleintro").Select(m => m.ID);
            if (articleIntroMenuIds?.Count() > 0)
                sql.Add(BuildArticleIntroSql(articleIntroMenuIds));

            IEnumerable<long> articleMenuIds = menus.Where(m => m.Module.ToLower() == "article").Select(m => m.ID);
            if (articleMenuIds?.Count() > 0)
                sql.Add(BuildArticleSql(articleMenuIds));

            IEnumerable<long> eventMenuIds = menus.Where(m => m.Module.ToLower() == "event").Select(m => m.ID);
            if (eventMenuIds?.Count() > 0)
                sql.Add(BuildEventSql(eventMenuIds));

            IEnumerable<long> shopMenuIds = menus.Where(m => m.Module.ToLower() == "customshops").Select(m => m.ID);
            if (shopMenuIds?.Count() > 0)
                sql.Add(BuildShopSql(shopMenuIds));

            if (sql.Count == 0)
                return new List<SearchResultModel>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            key = $"N'%{ key.Replace("'", "''") }%'";
            string detailConds = string.Format(@"Select SourceNo From Paragraph Where (Title Like {0} OR Contents Like {0})
						            UNION
								   Select SourceNo From ResourceImages Where (Spec Like {0} OR SpecDetail Like {0})
						            UNION
						            Select SourceNo From SEO Where (Keywords Like {0})", key);
            string detailID = "";
            //WorkLib.WriteLog.Write(true, detailConds);
            DataTable detailTable = db.GetDataTable(detailConds);
            if (detailTable != null && detailTable.Rows.Count > 0)
            {
                foreach (DataRow detailRow in detailTable.Rows)
                {
                    detailID += detailRow[0].ToString() + ",";
                }
                detailID = detailID.Trim(',');
            }

            List<string> orSql = new List<string>();
            orSql.Add($"Title Like { key } OR TypeName Like { key }");
            //2019-01-22 charlie_shan 調整(效能)
            //orSql.Add($"ID IN (Select SourceNo From Paragraph Where (Title Like { key } OR Contents Like { key })) OR ID IN (Select SourceNo From ResourceImages Where (Spec Like { key } OR SpecDetail Like { key }))");
            //orSql.Add($"ID IN (Select SourceNo From Paragraph Where ID IN (Select SourceNo From ResourceImages Where (Spec Like { key } OR SpecDetail Like { key })))");
            //2019-01-22 charlie_shan 調整(效能)
            //orSql.Add($"ID IN (Select SourceNo From SEO Where (Keywords Like { key }))");
            if (!string.IsNullOrEmpty(detailID))
                orSql.Add($"ID IN ({detailID})");
            string query = $"Select * From ({ string.Join(" UNION ALL ", sql) }) N WHERE ({ string.Join(" OR ", orSql) }) Order By MenuID, Sort";

            //WorkLib.WriteLog.Write(true, query);
            return CreateResultItems(db.GetDataTable(query));
        }
        public static IEnumerable<SearchResultModel> GetItemsByPoster(IEnumerable<SearchMenuModel> menus, string key, long? posterID)
        {
            if (menus?.Count() == 0)
                return new List<SearchResultModel>();

            List<string> sql = new List<string>();


            IEnumerable<long> articleMenuIds = menus.Where(m => m.Module.ToLower() == "article").Select(m => m.ID);
            if (articleMenuIds?.Count() > 0)
                sql.Add(BuildArticleSql(articleMenuIds));

            if (sql.Count == 0)
                return new List<SearchResultModel>();

           
            List<string> orSql = new List<string>();
            if(posterID.HasValue)
                orSql.Add($" ID IN (SELECT ArticleID FROM ArticleToPoster WHERE PosterID={posterID.Value} OR PosterID IN (SELECT ID FROM ArticlePoster WHERE Name='{key.Replace("'", "''")}')) ");
            else
                orSql.Add($" ID IN (SELECT ArticleID FROM ArticleToPoster WHERE PosterID IN (SELECT ID FROM ArticlePoster WHERE Name='{key.Replace("'", "''")}')) ");

            string query = $"Select * From ({ string.Join(" UNION ALL ", sql) }) N WHERE ({ string.Join(" OR ", orSql) }) Order By MenuID, Sort";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return CreateResultItems(db.GetDataTable(query));
        }
        public static IEnumerable<SearchResultModel> GetItemsBySeries(IEnumerable<SearchMenuModel> menus, string key, long? seriesID)
        {
            if (menus?.Count() == 0)
                return new List<SearchResultModel>();

            List<string> sql = new List<string>();


            IEnumerable<long> articleMenuIds = menus.Where(m => m.Module.ToLower() == "article").Select(m => m.ID);
            if (articleMenuIds?.Count() > 0)
                sql.Add(BuildArticleSql(articleMenuIds));

            if (sql.Count == 0)
                return new List<SearchResultModel>();


            List<string> orSql = new List<string>();
            if(seriesID.HasValue)
                orSql.Add($" ID IN (SELECT ArticleID FROM ArticleToSeries WHERE SeriesID={seriesID.Value} OR SeriesID IN (SELECT ID FROM ArticleSeries WHERE Name='{key.Replace("'", "''")}')) ");
            else
                orSql.Add($" ID IN (SELECT ArticleID FROM ArticleToSeries WHERE SeriesID IN (SELECT ID FROM ArticleSeries WHERE Name='{key.Replace("'", "''")}')) ");

            string query = $"Select * From ({ string.Join(" UNION ALL ", sql) }) N WHERE ({ string.Join(" OR ", orSql) }) Order By MenuID, Sort";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return CreateResultItems(db.GetDataTable(query));
        }

        private static string BuildArticleIntroSql(IEnumerable<long> menuIds) {
            return
                "SELECT ID, MenuID, CardNo, Title, 1 Sort, Icon, NULL TypeName, " +
                "   (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = I.ID AND I.IsIssue = 1 AND Contents <> '' ORDER BY Sort) Summary," +
                "   (SELECT TOP(1) Img FROM ResourceImages R JOIN Paragraph P ON R.SourceNo = P.ID WHERE R.IsShow = 1 AND P.SourceNo = I.ID ORDER BY P.Sort, R.Sort) Image " +
                $"FROM ArticleIntro I WHERE IsIssue = 1 AND (IssueDate Is Null OR IssueDate <= GETDATE()) AND I.MenuID IN({ string.Join(", ", menuIds) })";
        }

        private static string BuildArticleSql(IEnumerable<long> menuIds) {            
            return
                "SELECT ID, MenuID, CardNo, Title, Sort, Icon, " +
                "   STUFF((Select '#' + Name From ArticleTypes Where IsIssue = 1 AND ID IN (Select TypeID From ArticleToType Where ArticleID = I.ID) FOR XML PATH('')), 1, 1, '') TypeName, " +
                "   (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = I.ID AND I.IsIssue = 1 AND Contents <> '' ORDER BY Sort) Summary," +
                "   (SELECT TOP(1) Img FROM ResourceImages R JOIN Paragraph P ON R.SourceNo = P.ID WHERE R.IsShow = 1 AND P.SourceNo = I.ID ORDER BY P.Sort, R.Sort) Image " +
                $"FROM Article I WHERE IsIssue = 1 AND (IssueDate Is Null OR IssueDate <= GETDATE()) AND MenuID IN({ string.Join(", ", menuIds) })";
        }

        private static string BuildEventSql(IEnumerable<long> menuIds) {
            return
                "SELECT ID, MenuID, CardNo, Title, Sort, Icon, " +
                "STUFF((Select '#' + Name From EventTypes Where IsIssue = 1 AND ID IN (Select TypeID From EventToType Where EventID = I.ID) FOR XML PATH('')), 1, 1, '') TypeName, " +
                "   (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = I.ID AND I.IsIssue = 1 AND Contents <> '' ORDER BY Sort) Summary," +
                "   (SELECT TOP(1) Img FROM ResourceImages R JOIN Paragraph P ON R.SourceNo = P.ID WHERE R.IsShow = 1 AND P.SourceNo = I.ID ORDER BY P.Sort, R.Sort) Image " +
                $"FROM Events I WHERE IsIssue = 1 AND (IssueDate Is Null OR IssueDate <= GETDATE()) AND MenuID IN({ string.Join(", ", menuIds) })";
        }

        private static string BuildShopSql(IEnumerable<long> menuIds) {
            return
                "SELECT ID, MenuID, CardNo, Title, Sort, Icon, NULL TypeName, " +
                "   (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = I.ID AND I.IsIssue = 1 AND Contents <> '' ORDER BY Sort) Summary," +
                "   (SELECT TOP(1) Img FROM ResourceImages R JOIN Paragraph P ON R.SourceNo = P.ID WHERE R.IsShow = 1 AND P.SourceNo = I.ID ORDER BY P.Sort, R.Sort) Image " +
                $"FROM CustomShops I WHERE IsIssue = 1 AND MenuID IN({ string.Join(", ", menuIds) })";
        }

        private static List<SearchResultModel> CreateResultItems(DataTable datas) {
            List<SearchResultModel> items = new List<SearchResultModel>();
            foreach(DataRow dr in datas.Rows) {
                string img = dr["Image"].ToString().Trim();
                string imageJSON = dr["Icon"].ToString().Trim();
                if (imageJSON != string.Empty) {
                    ResourceImagesModels imgItem = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(imageJSON);
                    if (!string.IsNullOrWhiteSpace(imgItem.Img))
                        img = imgItem.Img;
                }
                
                items.Add(new SearchResultModel {
                    MenuID = (long)dr["MenuID"],
                    CardNo = (long)dr["CardNo"],
                    Title = dr["Title"].ToString().Trim(),
                    Summary = dr["Summary"].ToString(),
                    Image = img
                });
            }

            return items;
        }
        #endregion

        public static IEnumerable<SearchMenuResultModel> GetMenuItems(long siteId, string key) {
            List<SearchMenuResultModel> items = new List<SearchMenuResultModel>();

            if (string.IsNullOrWhiteSpace(key))
                return items;

            string sql = 
                "Select M.ID, M.Title, CT.Code MenuCode, P.SN PageSN, RL.LinkInfo, " +
                "   CASE M.DataType " +
                "       WHEN 'Article' THEN (SELECT DefaultImg  FROM ArticleSetting WHERE MenuID = M.ID) " +
                // "       WHEN 'Event' THEN (SELECT DefaultImg  FROM EventSetting WHERE MenuID = M.ID) " +  //Joe 20190916尚無此資料表故先註解
                "   ELSE NULL END Image " +
                "From Menus M JOIN CardsType CT ON M.DataType = CT.Code " +
                "   LEFT JOIN Pages P ON P.SiteID = M.SiteID AND P.MenuID = M.ID AND P.SN = M.SN " +
                "   LEFT JOIN ResourceLinks RL ON RL.SourceType = 1 AND RL.SourceNo = M.ID AND RL.SiteID = M.SiteID " +
                $"WHERE M.SiteID = { siteId } AND M.ShowStatus = 1 AND M.Title Like N'%{ key.Replace("'", "''") }%'" +
                "ORDER BY (CASE M.AreaID WHEN 2 THEN 1 WHEN 1 THEN 2 ELSE M.AreaID END), M.Sort";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas == null || datas.Rows.Count == 0)
                return items;

            foreach(DataRow dr in datas.Rows) {
                string url = dr["MenuCode"].ToString().Trim() == "SingleLink" ? dr["LinkInfo"].ToString().Trim() : dr["PageSN"].ToString().Trim();
                string img = dr["Image"].ToString().Trim();
                if (img != string.Empty)
                    img = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, (long)dr["ID"]).TrimEnd('/') + "/" + img;
                items.Add(new SearchMenuResultModel { Title = dr["Title"].ToString().Trim(), Url = url, Image = img });
            }

            return items;
        }
    }
}