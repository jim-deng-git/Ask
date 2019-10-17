using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Models
{
    public class MainVisionDAO : BaseDAO<MainVisionModel>
    {
        public static MainVisionDAO Instance;

        static MainVisionDAO()
        {
            Instance = new MainVisionDAO();
        }

        public static List<MainVisionModel> Get(long? CardNo = null, bool? IsIssue = null)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                List<string> wheres = new List<string>();
                if (CardNo.HasValue)
                {
                    wheres.Add("[CardNo] = @CardNo");
                    param.Add("@CardNo", CardNo);
                }

                if (IsIssue.HasValue)
                {
                    wheres.Add("[IsIssue] = @IsIssue");
                    param.Add("@IsIssue", IsIssue);

                    if (IsIssue.Value)
                    {
                        wheres.Add("([IssueStart] IS NULL OR [IssueStart] < GETDATE())");
                        wheres.Add("([IssueEnd] IS NULL OR [IssueEnd] > GETDATE())");
                    }
                }

                StringBuilder sql = new StringBuilder();
                sql.Append($"SELECT * FROM [{TableName}] ");
                sql.Append($"WHERE { string.Join(" AND ", wheres) } ");
                sql.Append($"ORDER BY Sort, ID ");

                return cn.Query<MainVisionModel>(sql.ToString(), param).ToList();
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

        public static void Click(long id) {
            string sql = "Update MainVision Set Clicks = Clicks + 1 Where ID = " + id;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }
    }
}