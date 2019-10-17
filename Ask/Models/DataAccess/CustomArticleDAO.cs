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
    public class CustomArticleDAO
    {
        public static IEnumerable<CustomArticleListItemModel> GetItems(long menuId) {
            string sql = 
                $"Select C.MenuID, C.ArticleID, A.Title, M.Name Creator, S.SN SiteSN, P.SN PageSN " +
                $"From CustomArticle C " +
                $"  JOIN Article A ON C.ArticleID = A.ID " +
                $"  JOIN Cards ON A.CardNo = Cards.No " +
                $"  JOIN Zones Z ON Cards.ZoneNo = Z.NO " +
                $"  JOIN Pages P ON Z.PageNo = P.No " +
                $"  JOIN Member M ON A.Creator = M.ID " +
                $"  JOIN Sites S ON A.SiteID = S.ID " +
                $"Where C.MenuID = { menuId } AND A.IsIssue = 1 Order By C.Sort";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<CustomArticleListItemModel>(sql);
            }
        }

        /// <summary>
        /// 後端編輯頁文章列表
        /// </summary>        
        public static IEnumerable<CustomArticleListItemModel> GetItems(long menuId, string key, int pageSize, int pageIndex, out int recordCount) {
            List<CustomArticleListItemModel> items = new List<CustomArticleListItemModel>();

            string sql =
                "Select A.ID ArticleID, A.Title, M.Name Creator, S.SN SiteSN, P.SN PageSN " +
                "From Article A " +
                "  JOIN Cards ON A.CardNo = Cards.No " +
                "  JOIN Zones Z ON Cards.ZoneNo = Z.NO " +
                "  JOIN Pages P ON Z.PageNo = P.No " +
                "  JOIN Member M ON A.Creator = M.ID " +
                "  JOIN Sites S ON A.SiteID = S.ID " +
                "Where {0} Order By A.CreateTime Desc";

            List<string> where = new List<string>();
            where.Add("A.IsIssue = 1 AND A.MenuID > 0");
            where.Add($"A.ID Not In (Select ArticleID From CustomArticle Where MenuID = { menuId })");

            if(!string.IsNullOrWhiteSpace(key)) {
                key = string.Format("Like N'%{0}%'", key.Replace("'", "''"));
                where.Add(string.Format("(A.Title {0} OR Exists(Select 1 From Paragraph Where SourceNo = A.ID And (Title {0} OR Contents {0})))", key));
            }

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(string.Format(sql, string.Join(" And ", where)), pageSize, pageIndex, out recordCount);
            foreach (DataRow dr in datas.Rows) {
                items.Add(new CustomArticleListItemModel {
                    MenuID = 0,
                    ArticleID = (long)dr["ArticleID"],
                    Title = dr["Title"].ToString().Trim(),
                    Creator = dr["Creator"].ToString().Trim(),
                    SiteSN = dr["SiteSN"].ToString().Trim(),
                    PageSN = dr["PageSN"].ToString().Trim()
                });
            }

            return items;
        }

        /// <summary>
        /// 前端列表頁文章列表
        /// </summary>        
        public static IEnumerable<ArticleModels> GetItems(long menuId, int pageSize, int pageIndex, out int recordCount) {
            List<CustomArticleListItemModel> items = new List<CustomArticleListItemModel>();

            string sql =
                $"Select ID, CardNo, Type, Title, Link, IsOpenNew, Archive, IssueDate, CustomIcon, Icon, (SELECT TOP(1) Contents FROM Paragraph WHERE SourceNo = A.ID AND Contents <> '' ORDER BY Sort) Summary " +
                $"From Article A Left Join CustomArticle C ON A.ID = C.ArticleID AND C.MenuID = { menuId } " +
                $"Where {{0}} Order By IsNull(C.Sort, { int.MaxValue }), A.CreateTime Desc";

            List<string> where = new List<string>();
            where.Add("A.IsIssue = 1");
            where.Add("(A.IssueStart IS NULL OR A.IssueStart <= GETDATE()) AND (A.IssueEnd IS NULL OR A.IssueEnd >= GETDATE())");
            where.Add("A.MenuID > 0");

            sql = string.Format(sql, string.Join(" AND ", where));

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            return GetListItems(datas);
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
                        Summary = dr["Summary"].ToString()
                    });
                }
            }

            return articles;
        }

        public static int Add(long menuId, IEnumerable<long> articleIds) {
            if (articleIds == null || articleIds.Count() == 0)
                return 0;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            string fmt =                
                $"IF Not Exists(Select 1 From CustomArticle Where MenuID = { menuId } AND ArticleID = {{0}})\n" +
                $"Begin\n" +
                $"  Select @maxSort = IsNull(Max(Sort), 0) From CustomArticle Where MenuID = { menuId }\n" +
                $"  Insert CustomArticle(MenuID, ArticleID, Sort) Values({ menuId }, {{0}}, @maxSort)\n" +
                $"End\n";

            System.Text.StringBuilder sql = new System.Text.StringBuilder("Declare @maxSort int\n");
            foreach(long articleId in articleIds) {
                sql.AppendFormat(fmt, articleId);
            }

            return db.ExecuteNonQuery(sql.ToString());
        }

        public static int Remove(long menuId, IEnumerable<long> articleIds) {
            if (articleIds == null || articleIds.Count() == 0)
                return 0;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            string sql = $"Delete CustomArticle Where MenuID = { menuId } AND ArticleID IN ({ string.Join(", ", articleIds) })";
            return db.ExecuteNonQuery(sql);
        }

        public static void Sort(long menuId, IEnumerable<SortItem> items) {
            IEnumerable<long> sortIds = items.Select(item => item.ID);
            List<SortItem> itemList = items.ToList();
            
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = $"Select ArticleID From CustomArticle Where MenuID = { menuId } Order By Sort";
                IEnumerable<long> ids = conn.Query<long>(sql);

                int index = 1;
                System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
                string fmt = $"Update CustomArticle Set Sort = {{0}} Where MenuID = { menuId } And ArticleID = {{1}}\n";
                foreach (long id in ids) {
                    IEnumerable<SortItem> updateItems = itemList.Where(item => item.Index <= index).OrderBy(item => item.Index);
                    foreach (SortItem item in updateItems) {
                        sortSql.AppendFormat(fmt, index++, item.ID);
                        itemList.Remove(item);
                    }

                    if (!sortIds.Contains(id)) {
                        sortSql.AppendFormat(fmt, index++, id);
                    }
                }

                conn.Execute(sortSql.ToString());
            }
        }

        public static Dictionary<long, string[]> GetArticleMenus(IEnumerable<long> articles) {
            Dictionary<long, string[]> menus = new Dictionary<long, string[]>();
            if (articles == null || articles.Count() == 0)
                return menus;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"SELECT A.ID, M.Title Menu, (SELECT Title FROM Menus WHERE ID = M.ParentID) ParentMenu FROM Article A JOIN Menus M ON A.MenuID = M.ID WHERE A.ID IN ({ string.Join(", ", articles) })";
            DataTable datas = db.GetDataTable(sql);
            foreach(DataRow dr in datas.Rows) {
                menus.Add((long)dr["ID"], new string[] { dr["ParentMenu"].ToString().Trim(), dr["Menu"].ToString().Trim() });
            }

            return menus;
        }
    }
}