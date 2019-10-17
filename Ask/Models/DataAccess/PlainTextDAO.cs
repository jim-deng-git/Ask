using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WorkV3.Models
{
    public class PlainTextDAO : BaseDAO<PlainTextModel>
    {
        public static PlainTextDAO Instance;

        static PlainTextDAO()
        {
            Instance = new PlainTextDAO();
        }

        public static PlainTextModel Get(long? CardNo = null, bool? IsIssue = null)
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
                }

                StringBuilder sql = new StringBuilder();
                sql.Append($"SELECT * FROM [{TableName}] ");
                sql.Append($"WHERE { string.Join(" AND ", where) } ");

                return cn.Query<PlainTextModel>(sql.ToString(), param).FirstOrDefault();
            }
        }
    }
}