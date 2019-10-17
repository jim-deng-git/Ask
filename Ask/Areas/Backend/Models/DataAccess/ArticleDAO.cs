using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ArticleDAO
    {
        public static ArticleModels GetItem(long id)
        {
            return CommonDA.GetItem<ArticleModels>("Article", id);
        }

        public static void SetItem(ArticleModels item,int cardStyleID = 1) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Article");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From Article Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                MenusModels menu = MenusDAO.GetInfo(item.SiteID, item.MenuID);
                long cardNo = WorkV3.Golbal.PubFunc.AddPage(item.SiteID, item.MenuID, menu.SN, "Article", "Content", true, item.Title ,CardStyleId: cardStyleID);
                tableObj["CardNo"] = cardNo;
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;
                tableObj["Sort"] = 1;

                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("MenuID");
                tableObj.Remove("CardNo");
                tableObj.Remove("Clicks");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }

        public static int Delete(long id) {
            long[] ids = { id };
            return Delete(ids);
        }

        public static int Delete(IEnumerable<long> ids) {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "Delete Pages Where NO IN (SELECT PageNo From Zones Where No IN (SELECT ZoneNo From Cards WHERE No IN (Select CardNo From Article WHERE  ID IN ({0}))))\n" +
                "Delete Zones Where No IN (SELECT ZoneNo From Cards WHERE No IN (Select CardNo From Article WHERE  ID IN ({0})))\n" +
                "Delete Cards WHERE No IN (Select CardNo From Article WHERE  ID IN ({0}))\n" +
                "Delete Article Where ID In ({0})\n" +
                "Delete ArticleToPoster Where ArticleID In ({0})\n" +
                "Delete ArticleToType Where ArticleID In ({0})\n";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }
            ParagraphDAO.DeleteBySourceNo(ids);

            return num;
        }

        public static void Copy(long sourceId, long targetId, long targetCardNo, long? targetMenuId = null, long? targetSiteId = null, bool forceCopyRelative = false) {
            string menuId = targetMenuId == null ? "MenuID" : targetMenuId.ToString();
            string siteId = targetSiteId == null ? "SiteID" : targetSiteId.ToString();
            long creator = MemberDAO.SysCurrent.Id;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "INSERT Article(ID, SiteID, MenuID, Type, Title, IssueDate, RemarkText, Icon, IsIssue, IssueStart, IssueEnd, Link, IsOpenNew, Archive, CardNo, Clicks, CustomIcon, TmplName, TmplThumb, Sort, Creator, CreateTime, VideoImgIsCustom, IsShowVideo) " +
                    $"SELECT { targetId }, { siteId }, { menuId }, Type, Title, IssueDate, RemarkText, Icon, IsIssue, IssueStart, IssueEnd, Link, IsOpenNew, Archive, { targetCardNo }, 0, CustomIcon, TmplName, TmplThumb, Sort, { creator }, GETDATE(), VideoImgIsCustom, IsShowVideo "  +
                    $"FROM Article WHERE ID = { sourceId }";
                conn.Execute(sql);

                if(forceCopyRelative || (targetMenuId == null && targetSiteId == null)) { // 強迫複製關聯或為同一單元，則需要複製 Type 和 Poster
                    sql =
                        $"INSERT ArticleToType(ArticleID, TypeID) SELECT { targetId }, TypeID FROM ArticleToType WHERE ArticleID = { sourceId }\n" +
                        $"INSERT ArticleToSeries(ArticleID, SeriesID) SELECT { targetId }, SeriesID FROM ArticleToSeries WHERE ArticleID = { sourceId }\n" +
                        $"INSERT ArticleToPoster(ArticleID, PosterID, Sort) SELECT { targetId }, PosterID, Sort FROM ArticleToPoster WHERE ArticleID = { sourceId }\n";
                    conn.Execute(sql);
                }
            }

            ParagraphDAO.CopyAllInSourceNo(sourceId, targetId, targetSiteId);
        }

        public static void CopyArticle(long sourceId, long targetId, long targetCardNo, long targetMenuId, long targetSiteId, bool forceCopyRelative = false)
        {
            long creator = MemberDAO.SysCurrent.Id;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "INSERT Article(ID, SiteID, MenuID, Type, Title, IssueDate, RemarkText, Icon, IsIssue, IssueStart, IssueEnd, Link, IsOpenNew, Archive, CardNo, Clicks, CustomIcon, TmplName, TmplThumb, Sort, Creator, CreateTime, IsShowVideo, VideoImgIsCustom, VideoImg, VideoID, VideoType, SubTitle) " +
                    $"SELECT { targetId }, { targetSiteId }, { targetMenuId }, Type, Title+'- (複製)', IssueDate, RemarkText, Icon, 0, IssueStart, IssueEnd, Link, IsOpenNew, Archive, { targetCardNo }, 0, CustomIcon, TmplName, TmplThumb, Sort, { creator }, GETDATE(), IsShowVideo, VideoImgIsCustom, VideoImg, VideoID, VideoType,SubTitle " +
                    $"FROM Article WHERE ID = { sourceId }";
                conn.Execute(sql);

                if (forceCopyRelative)
                { // 強迫複製關聯或為同一單元，則需要複製 Type 和 Poster
                    sql =
                        $"INSERT ArticleToType(ArticleID, TypeID) SELECT { targetId }, TypeID FROM ArticleToType WHERE ArticleID = { sourceId }\n" +
                        $"INSERT ArticleToPoster(ArticleID, PosterID, Sort) SELECT { targetId }, PosterID, Sort FROM ArticleToPoster WHERE ArticleID = { sourceId }\n";
                    conn.Execute(sql);
                }
            }

            ParagraphDAO.CopyAllInSourceNo(sourceId, targetId, targetSiteId);
        }

        public static void Move(long sourceId, long targetMenuId, long targetSiteId)
        {
            string menuId = targetMenuId.ToString();
            string siteId = targetSiteId.ToString();
            long creator = MemberDAO.SysCurrent.Id;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"UPDATE Article SET MenuID={menuId} , SiteID={siteId} , IsIssue=0 " +
                                $" WHERE ID = { sourceId }";
                conn.Execute(sql);

                // 強迫刪除關聯(搬移只能是不同單元)
                sql =
                    $"DELETE ArticleToType WHERE ArticleID = { sourceId }\n" +
                    $"DELETE ArticleToPoster WHERE ArticleID = { sourceId }\n";
                conn.Execute(sql);
            }
        }
        public static void Sort(long menuId, IEnumerable<SortItem> items) {
            CommonDA.Sort("Article", items, "MenuID = " + menuId);
        }

        public static void ToggleIssue(long id) {
            CommonDA.ToggleIssue("Article", id);
        }

        public static IEnumerable<ArticleModels> GetItems(ArticleSearchModels search, int pageSize, int pageIndex, out int recordCount) {
            List<ArticleModels> items = new List<ArticleModels>();
            if (search == null) {
                recordCount = 0;
                return items;
            }

            string sql = "Select ID, CardNo, Type, Title, Link, Archive, IsIssue, IssueStart, IssueEnd, Creator From Article A Where {0} Order By Sort, CreateTime Desc";

            List<string> where = new List<string>();
            where.Add("MenuID = " + search.MenuID);

            if (!string.IsNullOrWhiteSpace(search.Key)) {
                string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                where.Add(string.Format("(Title {0} OR Exists(Select 1 From Paragraph Where SourceNo = A.ID And (Title {0} OR Contents {0})))", key));
            }

            if(search.Types != null && search.Types.Count() > 0) 
                where.Add(string.Format("ID In (Select ArticleID From ArticleToType Where TypeID In ({0}))", string.Join(", ", search.Types)));            

            if(search.IssueStart != null) 
                where.Add(string.Format("IssueEnd >= '{0:yyyy/MM/dd HH:mm}'", search.IssueStart));

            if (search.IssueEnd != null)
                where.Add(string.Format("IssueStart <= '{0:yyyy/MM/dd HH:mm}'", search.IssueEnd));
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(string.Format(sql, string.Join(" And ", where)), pageSize, pageIndex, out recordCount);
            foreach(DataRow dr in datas.Rows) {
                items.Add(new ArticleModels {
                    ID = (long)dr["ID"],
                    CardNo = (long)dr["CardNo"],
                    Type = dr["Type"].ToString().Trim(),
                    Title = dr["Title"].ToString().Trim(),
                    Link = dr["Link"].ToString().Trim(),
                    Archive = dr["Archive"].ToString().Trim(),
                    IsIssue = (bool)dr["IsIssue"], 
                    IssueStart = dr["IssueStart"] as DateTime?,
                    IssueEnd = dr["IssueEnd"] as DateTime?,
                    Creator = (long)dr["Creator"]
                });
            }

            return items;
        }

        public static IEnumerable<long> GetItemTypes(long itemId) {
            string sql = "SELECT TypeID FROM ArticleToType WHERE ArticleID = " + itemId;
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<long>(sql);
            }
        }
        public static IEnumerable<long> GetItemSeries(long itemId)
        {
            string sql = "SELECT SeriesID FROM ArticleToSeries WHERE ArticleID = " + itemId;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<long>(sql);
            }
        }
        public static IEnumerable<long> GetItemCategories(long itemId)
        {
            string sql = "SELECT CategoryID FROM ArticleToCategory WHERE ArticleID = " + itemId;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<long>(sql);
            }
        }

        public static void SetItemTypes(long articleId, IEnumerable<long> types) {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendLine("DELETE ArticleToType WHERE ArticleID = " + articleId);

            if (types != null && types.Count() > 0) {
                string fmt = "Insert ArticleToType(ArticleID, TypeID) Values(" + articleId + ", {0})\n";
                foreach (long typeId in types)
                    sql.AppendFormat(fmt, typeId);
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                conn.Execute(sql.ToString());
            }
        }
        public static void SetItemSeries(long articleId, IEnumerable<long> series) {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendLine("DELETE ArticleToSeries WHERE ArticleID = " + articleId);

            if (series != null && series.Count() > 0) {
                string fmt = "Insert ArticleToSeries(ArticleID, SeriesID) Values(" + articleId + ", {0})\n";
                foreach (long seriesId in series)
                    sql.AppendFormat(fmt, seriesId);
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                conn.Execute(sql.ToString());
            }
        }

        public static void SetItemCategories(long articleId, string categoryType, IEnumerable<long> categories) {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendLine("DELETE ArticleToCategory WHERE ArticleID = " + articleId + " and CategoryType = '" + categoryType.Replace("'", "") + "' ");

            if (categories != null && categories.Count() > 0)
            {
                string fmt = "Insert ArticleToCategory (ArticleID, CategoryID, CategoryType) Values(" + articleId + ", {0}, '" + categoryType.Replace("'", "") + "')\n";
                foreach (long categoryID in categories)
                    sql.AppendFormat(fmt, categoryID);
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                conn.Execute(sql.ToString());
            }
        }

        public static Dictionary<long, List<ArticleTypesModels>> GetItemTypes(IEnumerable<long> itemIds) {
            Dictionary<long, List<ArticleTypesModels>> types = new Dictionary<long, List<ArticleTypesModels>>();
            if (itemIds == null || itemIds.Count() == 0)
                return types;

            string sql = "SELECT I.ArticleID, I.TypeID, T.Name FROM ArticleToType I JOIN ArticleTypes T ON I.TypeID = T.ID WHERE I.ArticleID IN ({0}) ORDER BY T.Sort";
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
                    Name = dr["Name"].ToString().Trim()
                });
            }

            return types;
        }

        public static IEnumerable<ArticlePosterModels> GetItemPosters(long itemId) {
            string sql = "SELECT P.ID, P.Name, P.Photo FROM ArticleToPoster T JOIN ArticlePoster P On T.PosterID = P.ID WHERE T.ArticleID = {0} And P.IsIssue = 1 Order By T.Sort";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticlePosterModels>(string.Format(sql, itemId));
            }
        }

        public static void SetItemPosters(long articleId, IEnumerable<long> posters) {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendLine("DELETE ArticleToPoster WHERE ArticleID = " + articleId);

            if (posters != null && posters.Count() > 0) {
                string fmt = "Insert ArticleToPoster(ArticleID, PosterID, Sort) Values(" + articleId + ", {0}, {1})\n";
                int i = 0;
                foreach (long posterId in posters)
                    sql.AppendFormat(fmt, posterId, ++i);
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                conn.Execute(sql.ToString());
            }
        }
        
        public static IEnumerable<WorkV3.Models.SitesModels> GetItemSites(long itemId) {
            string sql = "SELECT S.ID, S.Title, S.SN FROM ArticleToSite T JOIN Sites S ON T.SiteID = S.ID WHERE T.ArticleID = " + itemId;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<WorkV3.Models.SitesModels>(sql);
            }
        }
        
        public static void SetItemSites(long articleId, IEnumerable<long> sites) {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendLine("DELETE ArticleToSite WHERE ArticleID = " + articleId);

            if (sites?.Count() > 0) {
                string fmt = "Insert ArticleToSite(ArticleID, SiteID) Values(" + articleId + ", {0})\n";
                foreach (long siteId in sites)
                    sql.AppendFormat(fmt, siteId);
            }

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                conn.Execute(sql.ToString());
            }
        }

        public static void SetTemplateName(long id, string name, string thumb = null) {
            string sql = $"Update Article Set TmplName = N'{ (name ?? string.Empty).Replace("'", "''") }'" + 
                (thumb == null ? string.Empty : $", TmplThumb = N'{ thumb.Replace("'", "''") }'") + " Where ID = " + id;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                conn.Execute(sql);
            }
        }
        
        public static IEnumerable<WorkV3.Models.TemplateModels> GetTemplates(long menuId) {
            string sql = $"SELECT ID, TmplName, TmplThumb FROM Article WHERE MenuID = { -menuId } AND TmplName <> ''";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);

            List<WorkV3.Models.TemplateModels> templates = new List<WorkV3.Models.TemplateModels>();
            foreach(DataRow dr in datas.Rows) {
                templates.Add(new WorkV3.Models.TemplateModels {
                    ID = (long)dr["ID"],
                    Name = dr["TmplName"].ToString().Trim(),
                    Thumb = dr["TmplThumb"].ToString().Trim()
                });
            }

            return templates;
        }

        public static WorkV3.Models.TemplateModels GetTemplate(long id) {
            string sql = "SELECT ID, TmplName, TmplThumb FROM Article WHERE ID = " + id;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas == null || datas.Rows.Count == 0)
                return null;

            DataRow dr = datas.Rows[0];
            return new WorkV3.Models.TemplateModels {
                ID = (long)dr["ID"],
                Name = dr["TmplName"].ToString().Trim(),
                Thumb = dr["TmplThumb"].ToString().Trim()
            };
        }

        public static IEnumerable<WorkV3.Models.SitesModels> GetMobuleSites()
        {
            string sql = "Select * FROM Sites Where ID IN (SELECT DISTINCT SiteID FROM Menus WHERE DataType='Article') ORDER BY ID ";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<WorkV3.Models.SitesModels>(sql);
            }
        }
        public static IEnumerable<WorkV3.Models.MenusModels> GetMobuleMenus(long SiteID)
        {
            string sql = $"SELECT * FROM Menus WHERE DataType='Article' AND SiteID={SiteID} ORDER BY Sort ";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<WorkV3.Models.MenusModels>(sql);
            }
        }
    }
}