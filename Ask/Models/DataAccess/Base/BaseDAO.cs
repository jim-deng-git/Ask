using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace WorkV3.Models
{
    public class BaseDAO<T>
        where T : class
    {
        #region 保護欄位

        /// <summary>
        /// 連線字串
        /// </summary>
        protected string connectionString;

        /// <summary>
        /// 資料模型類別
        /// </summary>
        protected static Type type;

        /// <summary>
        /// 資料表名稱
        /// </summary>
        protected static string TableName;

        /// <summary>
        /// 新增指令
        /// </summary>
        protected static string createCommand;

        /// <summary>
        /// 更新指令
        /// </summary>
        protected static string updateCommand;

        /// <summary>
        /// 查詢主鍵指令
        /// </summary>
        protected static string getByIDCommand;

        #endregion 保護欄位

        #region 公開屬性

        public string CreateCommand { get { return createCommand; } private set { } }

        public string UpdateCommand { get { return updateCommand; } private set { } }

        public string GetByIDCommand { get { return getByIDCommand; } private set { } }

        #endregion

        #region 建構子

        static BaseDAO()
        {
            type = typeof(T);
            TableName = (type.GetCustomAttributes(typeof(Table), false).FirstOrDefault() as Table).Name;

            #region 指令組裝

            List<string> createfields = new List<string>();
            List<string> updatefields = new List<string>();
            List<string> updateWheres = new List<string>();
            foreach (var property in type.GetProperties())
            {
                if (Attribute.IsDefined(property, typeof(PKey)))
                    updateWheres.Add($"[{property.Name}] = @{property.Name}");

                if (!Attribute.IsDefined(property, typeof(IgnoreCreate)))
                    createfields.Add($"{property.Name}");

                if (!Attribute.IsDefined(property, typeof(IgnoreUpdate)))
                    updatefields.Add($"[{property.Name}] = @{property.Name}");
            }

            createCommand = $"INSERT INTO [{TableName}]([{string.Join("],[", createfields)}]) VALUES (@{string.Join(",@", createfields)}) ";
            updateCommand = $"UPDATE [{TableName}] SET {string.Join(",", updatefields)} WHERE {string.Join(" AND ", updateWheres)} ";
            getByIDCommand = $"SELECT {string.Join(",", updateWheres)} FROM [{TableName}] WHERE {string.Join(" AND ", updateWheres)} ";

            #endregion
        }

        #endregion 建構子

        #region 公開方法

        public IEnumerable<T> Get(Query query)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try
                {
                    var param = new Dictionary<string, object>();
                    var commandText = $"SELECT * FROM [{TableName}] ";
                    commandText += query.CreateCommand(out param);
                    return connection.Query<T>(commandText, param);
                }
                catch { throw; }
            }
        }

        public int Create(T data)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try { return connection.Execute(createCommand, data); } catch { throw; }
            }
        }

        public int Update(T data)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try
                {
                    var r = connection.Query<T>(getByIDCommand, data);

                    if (r.Count() == 0)
                    {
                        return connection.Execute(createCommand, data);
                    }
                    else
                    {
                        return connection.Execute(updateCommand, data);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public int Delete(Query query)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try
                {
                    var param = new Dictionary<string, object>();
                    var commandText = $"DELETE FROM [{TableName}] ";
                    commandText += query.CreateCommand(out param);
                    return connection.Execute(commandText, param);
                }
                catch { throw; }
            }
        }

        #endregion 公開方法
    }
}
