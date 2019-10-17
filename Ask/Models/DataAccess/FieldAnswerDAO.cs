using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WorkV3.Models
{
    public class FieldAnswerDAO : BaseDAO<FieldAnswerModel>
    {
        public static FieldAnswerDAO Instance;

        static FieldAnswerDAO()
        {
            Instance = new FieldAnswerDAO();
        }

        public void Create(List<FieldAnswerModel> datas)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try
                {
                    foreach (var data in datas)
                    {
                        connection.Execute(createCommand, data);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public IEnumerable<FieldAnswerModel> GetForBackEnd(IEnumerable<long> applicantIDs, IEnumerable<long> fieldIDs)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try
                {
                    var command = new StringBuilder();
                    command.Append($"SELECT * FROM [{TableName}] ");

                    if (applicantIDs.Count() > 0 && fieldIDs.Count() > 0)
                    {
                        command.Append($"WHERE [ID] IN ({string.Join(",", applicantIDs)})");
                        command.Append($"AND [FieldID] IN ({string.Join(",", fieldIDs)})");
                    }
                    else
                    {
                        command.Append("WHERE 1=0");
                    }

                    return connection.Query<FieldAnswerModel>(command.ToString());
                }
                catch { throw; }
            }
        }
    }
}