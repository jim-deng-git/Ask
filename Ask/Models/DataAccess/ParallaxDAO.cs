using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Models
{
    public class ParallaxDAO : BaseDAO<ParallaxModel>
    {
        public static ParallaxDAO Instance;

        static ParallaxDAO()
        {
            Instance = new ParallaxDAO();
        }

        public static List<ParallaxModel> Get(long? CardNo = null, bool? IsIssue = null, SearchModel Search = null)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                List<string> where = new List<string>();
                if (CardNo.HasValue)
                {
                    where.Add("[CardNo] = @CardNo");
                    param.Add("@CardNo", CardNo);
                }

                if (IsIssue.HasValue)
                {
                    where.Add("[IsIssue] = @IsIssue");
                    param.Add("@IsIssue", IsIssue);

                    if (IsIssue.Value)
                    {
                        where.Add("([IssueStart] IS NULL OR [IssueStart] < GETDATE())");
                        where.Add("([IssueEnd] IS NULL OR [IssueEnd] > GETDATE())");
                    }
                }

                #region Search

                if (Search != null)
                {
                    if (!string.IsNullOrWhiteSpace(Search.Key))
                    {
                        List<string> or = new List<string>();
                        or.Add("[Title] like '%' + @SearchKey + '%'");
                        or.Add("[Description] like '%' + @SearchKey + '%'");
                        where.Add($"({string.Join(" OR ", or)})");
                        param.Add("@SearchKey", Search.Key);
                    }

                    if (Search.IssueStart.HasValue)
                    {
                        where.Add("[IsIssue] = 1");
                        where.Add("([IssueStart] IS NULL OR [IssueStart] >= @SearchIssueStart OR [IssueEnd] >= @SearchIssueStart)");
                        param.Add("@SearchIssueStart", Search.IssueStart.Value.AddHours(-12));
                    }

                    if (Search.IssueEnd.HasValue)
                    {
                        where.Add("[IsIssue] = 1");
                        where.Add("([IssueEnd] IS NULL OR [IssueStart] <= @SearchIssueEnd OR [IssueEnd] <= @SearchIssueEnd)");
                        param.Add("@SearchIssueEnd", Search.IssueEnd.Value.AddHours(12));
                    }
                }

                #endregion

                StringBuilder sql = new StringBuilder();
                sql.Append($"SELECT * FROM [{TableName}] ");
                sql.Append($"WHERE { string.Join(" AND ", where) } ");
                sql.Append($"ORDER BY Sort, ID ");

                return cn.Query<ParallaxModel>(sql.ToString(), param).ToList();
            }
        }

        public static void Sort(long CardNo, IEnumerable<SortItem> items)
        {
            CommonDA.Sort(TableName, items, "CardNo = " + CardNo);
        }

        public static void Delete(IEnumerable<long> IDs)
        {
            CommonDA.Delete(TableName, IDs);
        }

        public static void ToggleIssue(long ID)
        {
            CommonDA.ToggleIssue(TableName, ID);
        }
    }
}