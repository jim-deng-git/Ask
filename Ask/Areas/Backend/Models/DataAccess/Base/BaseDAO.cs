using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class BaseDAO<T> where T : class
    {
        public static Type Type
        {
            get
            {
                return typeof(T);
            }
            private set { }
        }

        public static string TableName
        {
            get
            {
                return Type.GetAttributeValue((ModelAtteibute m) => m.TableName);
            }
            private set { }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(T model)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                var result = 0;
                var sqlCommand = BuildInsertSqlCommand();
                if (!string.IsNullOrEmpty(sqlCommand))
                    result = cn.Execute(sqlCommand, model);
                else
                    result = 0;
                return result;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(T model)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                var result = 0;
                var sqlCommand = BuildUpdateSqlCommand();
                if (!string.IsNullOrEmpty(sqlCommand))
                    result = cn.Execute(sqlCommand, model);
                else
                    result = 0;
                return result;
            }
        }

        /// <summary>
        /// 建造新增SQL命令
        /// </summary>
        /// <returns></returns>
        private static string BuildInsertSqlCommand()
        {
            List<string> paras = new List<string>();
            foreach (var prop in Type.GetProperties())
            {
                paras.Add($"@{prop.Name}");
            }

            if (string.IsNullOrEmpty(TableName))
                return string.Empty;
            else
                return $"INSERT INTO [{TableName}] VALUES({string.Join(",", paras)});";
        }

        /// <summary>
        /// 建造更新SQL命令
        /// </summary>
        /// <returns></returns>
        private static string BuildUpdateSqlCommand()
        {
            List<string> wheres = new List<string>();
            List<string> paras = new List<string>();

            foreach (var prop in Type.GetProperties())
            {
                var isPKey = Attribute.IsDefined(prop, typeof(PKeyAttribute));

                if (isPKey)
                    wheres.Add($"[{prop.Name}] = @{prop.Name}");
                else
                    paras.Add($"[{prop.Name}] = @{prop.Name}");
            }

            if (string.IsNullOrEmpty(TableName) || wheres.Count == 0)
                return string.Empty;
            else
                return $"UPDATE [{TableName}] SET {string.Join(",", paras)} WHERE {string.Join(" AND ", wheres)};";
        }
    }
}