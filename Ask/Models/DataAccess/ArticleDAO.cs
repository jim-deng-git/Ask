using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ArticleDAO
    {
        public static ArticleModels GetItem(long id)
        {
            string sql = "Select ID, CardNo, Type, Title, IssueDate, RemarkText, CustomIcon, Icon, Link, IsOpenNew, Archive, VideoID, VideoImgIsCustom, VideoImg, isShowVideo From Article Where ID = " + id;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {                
                return conn.Query<ArticleModels>(sql).SingleOrDefault();
            }
        }

        public static ArticleModels GetItemByCard(long cardNo) {
            string sql = "Select Top(1) ID, CardNo, Type, Title, SubTitle, IssueDate, RemarkText, CustomIcon, Icon, Link, IsOpenNew, Archive, VideoID, VideoImgIsCustom, VideoImg, isShowVideo, VideoType, ReadMode From Article Where CardNo = " + cardNo;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticleModels>(sql).SingleOrDefault();
            }
        }
        
        public static List<ArticleModels> GetItems(long menuId, long? typeId, int pageSize, int pageIndex, out int recordCount) {
            List<ArticleModels> items = new List<ArticleModels>();

            string typeWhere = string.Empty;
            if (typeId != null)
                typeWhere = "AND ID IN (Select ArticleID From ArticleToType Where TypeID = " + typeId;

            string sql =
                "Select ID, CardNo, Type, Title, IssueDate, CustomIcon, Icon, (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = A.ID AND Contents <> '' ORDER BY Sort) Summary From Article A " +
                $"Where IsIssue = 1 AND MenuID = { menuId } AND (IssueStart IS NULL OR IssueStart <= GETDATE()) AND (IssueEnd IS NULL OR IssueEnd >= GETDATE()) " +
                typeWhere + " Order By Sort, CreateTime Desc";
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            if (datas != null && datas.Rows.Count > 0) {
                foreach (DataRow dr in datas.Rows) {
                    items.Add(new ArticleModels {
                        ID = (long)dr["ID"],
                        CardNo = (long)dr["CardNo"],
                        Type = dr["Type"].ToString().Trim(),
                        Title = dr["Title"].ToString().Trim(),
                        IssueDate = dr["IssueDate"] as DateTime?,
                        Icon = dr["Icon"].ToString().Trim(),
                        Summary = dr["Summary"].ToString().Trim()
                    });
                }
            }

            return items;
        }

        public static IEnumerable<ArticleModels> GetItems(IEnumerable<long> typeIds, int? topNum = null) {
            string sql =
                $"Select { (topNum == null ? string.Empty : "Top(" + topNum + ")") } ID, CardNo, Type, Title, IssueDate, CustomIcon, Icon, " +
                "   (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = A.ID AND Contents <> '' ORDER BY Sort) Summary From Article A " +
                "Where IsIssue = 1 AND (A.IssueStart IS NULL OR A.IssueStart <= GETDATE()) AND (A.IssueEnd IS NULL OR A.IssueEnd >= GETDATE()) " +
                (typeIds?.Count() > 0 ? $" AND ID IN (Select ArticleID From ArticleToType Where TypeID In ({ string.Join(", ", typeIds) }))" : string.Empty) +
                "Order By Sort, CreateTime Desc";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticleModels>(sql);
            }
        }

        public static List<ArticleModels> GetItems(ArticleSetModels articleSet, int pageIndex = 1) {
            string sql =
                "Select ID, SiteID, CardNo, Type, Title, Link, IsOpenNew, Archive, IssueDate, CustomIcon, Icon, IsShowVideo, VideoID, " +
                "(SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = A.ID AND Contents <> '' ORDER BY Sort) Summary " +
                "From Article A Where {0} Order By {1}";

            List<string> where = new List<string>();
            where.Add("IsIssue = 1 And MenuID > 0"); // 去除掉模板

            List<string> orSql = new List<string>();
            IEnumerable<long> menus = articleSet.GetMenus();
            if (menus?.Count() > 0)
                orSql.Add($"MenuID In ({ string.Join(", ", menus) })");

            IEnumerable<long> types = articleSet.GetTypes();
            if (types?.Count() > 0)
                orSql.Add($"ID IN (Select ArticleID From ArticleToType Where TypeID In ({ string.Join(", ", types) }))");

            if (orSql.Count > 0)
                where.Add($"({ string.Join(" OR ", orSql) })");

            IEnumerable<int> issueSetting = articleSet.GetIssueSetting();
            if(issueSetting.Count() == 1) {
                if(issueSetting.Contains(0)) {
                    where.Add("(IssueStart IS NULL OR IssueStart <= GETDATE()) AND (IssueEnd IS NULL OR IssueEnd >= GETDATE())");
                } else {
                    where.Add("IssueEnd <= GETDATE()");
                }
            }

            string whereSql = string.Join(" AND ", where);
            sql = string.Format(sql, whereSql, articleSet.SortField);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            int totalRecord;
            DataTable datas = db.GetPageData(sql, articleSet.PageSize, pageIndex, out totalRecord);
            return GetListItems(datas);
        }
        
        public static List<ArticleModels> GetItems(ArticleSettingModels setting, string key, string type, long? typeId, int pageIndex, out int totalRecord) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            //string sessionKey = setting.MenuID.ToString();
            //string token = key + type + typeId;
            //System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;

            //dynamic sessionVal = session[sessionKey];
            List<long> ids = null;
            //if(sessionVal == null || sessionVal.Token != token) {
            //    session.Remove(sessionKey);

                string query = $"Select ID From Article A Where { GetWhereSql(setting, key, type, typeId) } Order By { setting.SortField }";
                ids = db.GetDataTable(query).AsEnumerable().Select(dr => (long)dr["ID"]).ToList();

            //    session.Add(sessionKey, new { Token = token, IDList = ids });
            //} else {
            //    ids = sessionVal.IDList;
            //}

            if (ids == null)
                ids = new List<long>();

            totalRecord = ids.Count;
                        
            IEnumerable<long> currentIds = ids.Skip((pageIndex - 1) * setting.PageSize).Take(setting.PageSize);
            if (currentIds.Count() == 0)
                return new List<ArticleModels>();
            
            string sql =
                "Select ID,SiteID, CardNo, Type, Title, Link, IsOpenNew, Archive, IssueDate, CustomIcon, Icon, IsShowVideo, VideoID, " +
                "(SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = A.ID AND Contents <> '' ORDER BY Sort) Summary " +
                $"From Article A Where ID IN ({ string.Join(", ", currentIds) })";
            DataTable datas = db.GetDataTable(sql);

            List<ArticleModels> items = GetListItems(datas);
            List<ArticleModels> itemList = new List<ArticleModels>();
            foreach(long id in currentIds) {
                ArticleModels item = items.FirstOrDefault(a => a.ID == id);
                if (item != null)
                    itemList.Add(item);
            }

            return itemList;
        }
        
        public static IEnumerable<ArticleModels> GetRecommendItems(ArticleSettingModels setting, long currentItemId, IEnumerable<long> currentItemTypeIds) {
            string sql =
                "Select Top({0}) ID, CardNo, Type, Title, IssueDate, CustomIcon, Icon, (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = A.ID AND Contents <> '' ORDER BY Sort) Summary " +
                "From Article A Where {1} Order By {2}";

            List<string> where = new List<string>();            
            where.Add("IsIssue = 1 AND MenuID > 0");
            where.Add("ID <> " + currentItemId);

            IEnumerable<int> issueSetting = setting.GetIssueSetting();
            if (issueSetting.Count() == 1) {
                if (issueSetting.Contains(0)) {
                    where.Add("(IssueStart IS NULL OR IssueStart <= GETDATE()) AND (IssueEnd IS NULL OR IssueEnd >= GETDATE())");
                } else {
                    where.Add("IssueEnd <= GETDATE()");
                }
            }

            string orderBy = "NewID()";
            List<long> menuIds = new List<long>();
            if (setting.ExtendReadMode == 1) { // 與本篇「相同類別」的其他文章優先推薦。如果筆數不足，以勾選單元的隨機文章補足。
                if (currentItemTypeIds?.Count() > 0) {
                    orderBy = $"ISNULL((SELECT TOP(1) 1 FROM ArticleToType WHERE ArticleID = A.ID AND TypeID IN ({ string.Join(", ", currentItemTypeIds) })), 2), Case When MenuID = { setting.MenuID } Then 1 Else 2 End, NewID()";
                    menuIds.Add(setting.MenuID);
                }
            }
            else if (setting.ExtendReadMode == 2)
            { // 與本篇 相同關鍵字 的其他文章優先推薦 如果筆數不足 以勾選單元的隨機文章補足。
                SEOModels seo = SEODAO.GetItem(currentItemId);
                if (seo != null && !string.IsNullOrWhiteSpace(seo.Keywords))
                {
                    string[] keywords = seo.Keywords.Split(';');
                    string fullLikeStr = "";
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(keywords[i]))
                        {
                            if (!string.IsNullOrWhiteSpace(fullLikeStr))
                                fullLikeStr += " OR ";
                                fullLikeStr += string.Format(" (Keywords LIKE '%{0}%')", keywords[i].Replace("'", "").Replace("--", ""));

                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fullLikeStr))
                    {
                        orderBy = string.Format(@"CASE WHEN ID in (
                                                SELECT SourceNo FROM SEO WHERE ({0})
                                ) THEN 1 ELSE 2 END, NewID()", fullLikeStr);
                    }

                }
            }
            else { // 在勾選單元中，隨機選擇其他文章顯示。

            }

            IEnumerable<long> settingExtendReadMenus = setting.GetExtendReadMenus();
            if (settingExtendReadMenus?.Count() > 0)
                menuIds.AddRange(settingExtendReadMenus);

            if (menuIds.Count > 0)
                where.Add($"MenuID In ({ string.Join(", ", menuIds.Distinct()) })");

            sql = string.Format(sql, setting.ExtendReadRowCount, string.Join(" AND ", where), orderBy);

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticleModels>(sql);
            }
        }

        public static IEnumerable<ArticleModels> GetRecommendItems2(ArticleSettingModels setting, long currentItemId, IEnumerable<long> currentItemTypeIds)
        {
            string sql =
                "Select Top({0}) ID, CardNo, Type, Title, IssueDate, CustomIcon, Icon, (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = A.ID AND Contents <> '' ORDER BY Sort) Summary " +
                "From Article A Where {1} Order By {2}";

            List<string> where = new List<string>();
            where.Add("IsIssue = 1 AND MenuID > 0");
            where.Add("ID <> " + currentItemId);

            IEnumerable<int> issueSetting = setting.GetIssueSetting();
            if (issueSetting.Count() == 1)
            {
                if (issueSetting.Contains(0))
                {
                    where.Add("(IssueStart IS NULL OR IssueStart <= GETDATE()) AND (IssueEnd IS NULL OR IssueEnd >= GETDATE())");
                }
                else
                {
                    where.Add("IssueEnd <= GETDATE()");
                }
            }

            string orderBy = "NewID()";
            List<long> menuIds = new List<long>();
            if (setting.ExtendReadMode2 == 1)
            { // 與本篇「相同類別」的其他文章優先推薦。如果筆數不足，以勾選單元的隨機文章補足。
                if (currentItemTypeIds?.Count() > 0)
                {
                    orderBy = $"ISNULL((SELECT TOP(1) 1 FROM ArticleToType WHERE ArticleID = A.ID AND TypeID IN ({ string.Join(", ", currentItemTypeIds) })), 2), Case When MenuID = { setting.MenuID } Then 1 Else 2 End, NewID()";
                    menuIds.Add(setting.MenuID);
                }
            }
            else if (setting.ExtendReadMode2 == 2)
            { // 與本篇 相同關鍵字 的其他文章優先推薦 如果筆數不足 以勾選單元的隨機文章補足。
                SEOModels seo = SEODAO.GetItem(currentItemId);
                if (seo != null && !string.IsNullOrWhiteSpace(seo.Keywords))
                {
                   // WriteLogs("Keywords:" + seo.Keywords);
                    string[] keywords = seo.Keywords.Split(';');
                    string fullLikeStr = "";
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(keywords[i]))
                        {
                            if (!string.IsNullOrWhiteSpace(fullLikeStr))
                                fullLikeStr += " OR ";
                            fullLikeStr += string.Format(" (Keywords LIKE '%{0}%')", keywords[i].Replace("'", "").Replace("--", ""));

                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fullLikeStr))
                    {
                        orderBy = string.Format(@"CASE WHEN ID in (
                                                SELECT SourceNo FROM SEO WHERE ({0})
                                ) THEN 1 ELSE 2 END, NewID()", fullLikeStr);
                    }

                }
            }
            else
            { // 在勾選單元中，隨機選擇其他文章顯示。

            }

            IEnumerable<long> settingExtendReadMenus = setting.GetExtendReadMenus2();
            if (settingExtendReadMenus?.Count() > 0)
                menuIds.AddRange(settingExtendReadMenus);

            if (menuIds.Count > 0)
                where.Add($"MenuID In ({ string.Join(", ", menuIds.Distinct()) })");

            sql = string.Format(sql, setting.ExtendReadRowCount2, string.Join(" AND ", where), orderBy);
            //WriteLogs(setting.ExtendReadMode2.ToString());
            //WriteLogs(sql);
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<ArticleModels>(sql);
            }
        }
        //private static void WriteLogs(string msg)
        //{
        //    string folderPath = HttpContext.Current.Server.MapPath("~/App_Data");
        //    string logFileName = string.Format("log_{0}.txt", DateTime.Now.ToString("yyyyMM"));
        //    if (!System.IO.Directory.Exists(folderPath))
        //    {
        //        System.IO.Directory.CreateDirectory(folderPath);
        //    }
        //    System.IO.File.AppendAllText(folderPath + "\\" + logFileName, DateTime.Now.ToString()+" "+ msg+System.Environment.NewLine);
        //}
        public static IEnumerable<long> GetAllIDs(ArticleSettingModels setting) {
            string sql = @"Select ID From Article A Where  CardNo IN (SELECT No FROM Cards WHERE ZoneNo IN (SELECT No FROM Zones Where PageNo IN 
                                (SELECT No From Pages))) {0} Order By {1}";
            string where_cond = GetWhereSql(setting, null, null);
            sql = string.Format(sql, string.IsNullOrEmpty(where_cond)?"":(" AND "+ where_cond), setting.SortField);
            //WorkLib.WriteLog.Write(true, sql);
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<long>(sql);
            }
        }

        private static string GetWhereSql(ArticleSettingModels setting, string key, string type, long? typeId = null) {
            List<string> where = new List<string>();
            where.Add("IsIssue = 1");
            where.Add("MenuID = " + setting.MenuID);
            
            if(!string.IsNullOrWhiteSpace(key)) {
                key = $"N'%{ key.Replace("'", "''") }%'";
                List<string> orSql = new List<string>();
                orSql.Add($"Title Like { key }");
                orSql.Add($"ID IN (Select SourceNo From Paragraph Where (Title Like { key } OR Contents Like { key }))");
                orSql.Add($"ID IN (Select SourceNo From Paragraph Where ID IN (Select SourceNo From ResourceImages Where (Spec Like { key } OR SpecDetail Like { key })))");
                orSql.Add($"ID IN (Select SourceNo From SEO Where (Keywords Like { key }) AND MenuID = { setting.MenuID })");
                where.Add($"({ string.Join(" OR ", orSql) })");
            }

            if (!string.IsNullOrWhiteSpace(type) && type != "全部") {
                where.Add($"ID IN (Select ArticleID From ArticleToType Where TypeID IN (Select ID From ArticleTypes Where IsIssue = 1 And ID IN ({ type.Replace("'", "''") }) And MenuID = { setting.MenuID }))");
            }

            if (typeId != null) {
                where.Add($"ID IN (Select ArticleID From ArticleToType Where TypeID = { typeId })");
            } else {
                if (setting.Types != "all") {
                    IEnumerable<long> types = setting.GetTypes();
                    if (types?.Count() > 0)
                        where.Add($"ID IN (Select ArticleID From ArticleToType Where TypeID In ({ string.Join(", ", types) }))");
                    else
                        where.Add("1 <> 1"); // 不選取任何資料
                }
            }

            IEnumerable<int> issueSetting = setting.GetIssueSetting();
            if (issueSetting.Count() == 1) {
                if (issueSetting.Contains(0)) {
                    where.Add("(IssueStart IS NULL OR IssueStart <= GETDATE()) AND (IssueEnd IS NULL OR IssueEnd >= GETDATE())");
                } else {
                    where.Add("IssueEnd <= GETDATE()");
                }
            }

            return string.Join(" AND ", where);
        }

        private static List<ArticleModels> GetListItems(DataTable datas) {
            List<ArticleModels> articles = new List<ArticleModels>();
            if (datas != null) {
                foreach (DataRow dr in datas.Rows) {
                    articles.Add(new ArticleModels {
                        ID = (long)dr["ID"],
                        CardNo = (long)dr["CardNo"],
                        Type = dr["Type"].ToString().Trim(),
                        Title = dr["Title"].ToString().Trim(),
                        Link = dr["Link"].ToString().Trim(),
                        IsOpenNew = (bool)dr["IsOpenNew"],
                        Archive = dr["Archive"].ToString().Trim(),
                        IssueDate = dr["IssueDate"] as DateTime?,
                        CustomIcon = (bool)dr["CustomIcon"],
                        Icon = dr["Icon"].ToString().Trim(),
                        Summary = dr["Summary"].ToString(),
                        isShowVideo = (bool)dr["IsShowVideo"],
                        VideoID = dr["VideoID"].ToString().Trim(),
                        SiteID = long.Parse(dr["SiteID"].ToString())
                    });
                }
            }

            return articles;
        }
        
        public static IEnumerable<ArticleTypesModels> GetItemTypes(long itemId) {            
            string sql = $"SELECT T.ID, T.Name, T.Color, T.Icon FROM ArticleToType M JOIN ArticleTypes T ON M.TypeID = T.ID WHERE T.IsIssue = 1 AND M.ArticleID = { itemId } ORDER BY T.Sort";
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticleTypesModels>(sql);
            }
        }
        
        public static IEnumerable<ArticleSeriesModels> GetItemSeries(long itemId) {            
            string sql = $"SELECT T.ID, T.Name, T.Color, T.Icon FROM ArticleToSeries M JOIN ArticleSeries T ON M.SeriesID = T.ID WHERE T.IsIssue = 1 AND M.ArticleID = { itemId } ORDER BY T.Sort";
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticleSeriesModels>(sql);
            }
        }

        public static IEnumerable<ArticleCategoryModels> GetItemCategories(long itemId, string type)
        {
            string sql = $"select CategoryID,c.Title from ArticleToCategory a left join Categories c on a.CategoryID = c.ID where ArticleID = {itemId} and CategoryType = '{type.Replace("'", "")}'";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<ArticleCategoryModels>(sql);
            }
        }

        public static Dictionary<long, List<ArticleTypesModels>> GetItemTypes(IEnumerable<long> itemIds) {
            Dictionary<long, List<ArticleTypesModels>> types = new Dictionary<long, List<ArticleTypesModels>>();
            if (itemIds == null || itemIds.Count() == 0)
                return types;

            string sql = "SELECT I.ArticleID, I.TypeID, T.Name, T.MenuID, T.Color FROM ArticleToType I JOIN ArticleTypes T ON I.TypeID = T.ID WHERE T.IsIssue = 1 AND I.ArticleID IN ({0}) ORDER BY T.Sort";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(string.Format(sql, string.Join(", ", itemIds)));
            foreach(DataRow dr in datas.Rows) {
                long articleId = (long)dr["ArticleID"];
                List<ArticleTypesModels> articleTypes;
                if(!types.TryGetValue(articleId, out articleTypes)) {
                    articleTypes = new List<ArticleTypesModels>();
                    types.Add(articleId, articleTypes);
                }

                articleTypes.Add(new ArticleTypesModels {
                    ID = (long)dr["TypeID"],
                    MenuID = (long)dr["MenuID"],
                    Name = dr["Name"].ToString().Trim(),
                    Color = dr["Color"].ToString().Trim()
                });
            }

            return types;
        }

        public static IEnumerable<ArticlePosterModels> GetItemPosters(long itemId) {
            string sql = "SELECT P.ID, P.Name, P.Photo, P.Intro, P.Resume FROM ArticleToPoster T JOIN ArticlePoster P On T.PosterID = P.ID WHERE T.ArticleID = {0} And P.IsIssue = 1 Order By T.Sort";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticlePosterModels>(string.Format(sql, itemId));
            }
        }

        public static void AddItemClicks(long id) {
            string sql = "Update Article Set Clicks = Clicks + 1 Where ID = " + id;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                conn.Execute(sql);
            }
        }

        public static bool CheckReadMode(long memberID, string type, IEnumerable<ArticleCategoryModels> readModeCategories)
        {
            bool CheckOk = false;
            Areas.Backend.Models.MemberShipSettingModel memberSet = Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMemberSetItem(memberID, type);
            if (memberSet != null && readModeCategories != null)
            {
                foreach (var item in readModeCategories)
                {
                    if (item.CategoryID == memberSet.CategoryID)
                    {
                        CheckOk = true;
                        break;
                    }
                }
            }
            return CheckOk;
        }

        public static bool ListCheckReadMode(long memberID, string type, IEnumerable<Areas.Backend.Models.CategoryModels> readModeCategories)
        {
            bool CheckOk = false;
            Areas.Backend.Models.MemberShipSettingModel memberSet = Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMemberSetItem(memberID, type);
            if (memberSet != null && readModeCategories != null)
            {
                foreach (var item in readModeCategories)
                {
                    if (item.ID == memberSet.CategoryID)
                    {
                        CheckOk = true;
                        break;
                    }
                }
            }
            return CheckOk;
        }
    }
}