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
    public class ArticleTypesDAO
    {
        public static IEnumerable<ArticleTypesModels> GetItems(long menuId, bool hasArticle = true) {
            string articleWhere = string.Empty;
            if (hasArticle)
                articleWhere = "AND Exists(SELECT 1 FROM ArticleToType M JOIN Article A ON M.ArticleID = A.ID WHERE M.TypeID = T.ID AND A.IsIssue = 1 AND (A.IssueStart IS NULL OR A.IssueStart <= GETDATE()) AND (A.IssueEnd IS NULL OR A.IssueEnd >= GETDATE()))";

            string sql = $"Select ID, MenuID, Name, Color, Icon From ArticleTypes T Where MenuID = { menuId } And IsIssue = 1 { articleWhere } Order By Sort";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Query<ArticleTypesModels>(string.Format(sql, menuId));
            }
        }        

        public static int GetArticleCount(long typeId) {
            string sql = 
                "SELECT COUNT(*) FROM ArticleToType M JOIN Article A ON M.ArticleID = A.ID " +
                $"WHERE M.TypeID = { typeId } AND A.IsIssue = 1 AND(A.IssueStart IS NULL OR A.IssueStart <= GETDATE()) AND(A.IssueEnd IS NULL OR A.IssueEnd >= GETDATE())";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.ExecuteScalar<int>(sql);
            }
        }
    }    
}