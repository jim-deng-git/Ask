using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;
using WorkV3.Common;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ArticleSeriesDAO
    {
        public static IEnumerable<ArticleSeriesModels> GetItems(long menuId, bool hasArticle = true) {
            string articleWhere = string.Empty;
            if (hasArticle)
                articleWhere = "AND Exists(SELECT 1 FROM ArticleToSeries M JOIN Article A ON M.ArticleID = A.ID WHERE M.SeriesID = T.ID AND A.IsIssue = 1 AND (A.IssueStart IS NULL OR A.IssueStart <= GETDATE()) AND (A.IssueEnd IS NULL OR A.IssueEnd >= GETDATE()))";

            string sql = $"Select ID, Name, Color, Icon From ArticleSeries T Where MenuID = { menuId } And IsIssue = 1 { articleWhere } Order By Sort";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticleSeriesModels>(string.Format(sql, menuId));
            }
        }        

        public static int GetArticleCount(long seriesId) {
            string sql =
                "SELECT COUNT(*) FROM ArticleToSeries M JOIN Article A ON M.ArticleID = A.ID " +
                $"WHERE M.SeriesID = { seriesId } AND A.IsIssue = 1 AND(A.IssueStart IS NULL OR A.IssueStart <= GETDATE()) AND(A.IssueEnd IS NULL OR A.IssueEnd >= GETDATE())";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.ExecuteScalar<int>(sql);
            }
        }
    }    
}