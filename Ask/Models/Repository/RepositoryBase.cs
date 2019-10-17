using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models.DataAccess;
using Dapper;
using System.Data.SqlClient;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Transactions;

namespace WorkV3.Models.Repository
{
    public class RepositoryBase<TEntity>: Interface.IGenericRepository<TEntity> where TEntity: new()
    {
        protected string _tableName;

        public RepositoryBase()
        {

        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbConn"].ToString());
        }

        public RepositoryBase(string tableName)
        {
            SetTableName(tableName);
        }

        public void SetTableName(string tableName)
        {
            _tableName = tableName;
        }

        public TEntity GetItem(object value, string columnName = "ID")
        {
            using(SqlConnection conn = GetConnection())
            {
                string sql = $"Select * From {_tableName} Where {columnName} = @Param ";
                return conn.Query<TEntity>(sql, new { Param = value }).SingleOrDefault();
            }
        }

        public TEntity GetItem(Dictionary<string, object> param)
        {
            if (param.Count == 0)
                throw new Exception("參數不得為空");

            string whereClause = "";
            List<string> where = new List<string>();

            foreach(var item in param)
            {
                where.Add($" {item.Key} = @{item.Key} ");
            }
            whereClause = string.Join(" AND ", where);

            using(SqlConnection conn = GetConnection())
            {
                string sql = $"Select * From {_tableName} Where {whereClause} ";
                return conn.Query<TEntity>(sql, param).SingleOrDefault();
            }
        }

        public virtual IEnumerable<TEntity> GetItemsIn(IEnumerable<long> values, string columnName = "ID")
        {
            using (SqlConnection conn = GetConnection())
            {
                List<string> paramNames = new List<string>();
                Dictionary<string, object> param = new Dictionary<string, object>();
                foreach (var item in values.Select((value, idx) => new { idx, value }))
                {
                    var value = item.value;
                    var index = item.idx;
                    string paramName = $"@Param{index}";

                    paramNames.Add(paramName);
                    param.Add(paramName, value);
                }

                string sql = $"Select * From {_tableName} Where {columnName} IN ({string.Join(",", paramNames)}) ";
                return conn.Query<TEntity>(sql, param);
            }
        }

        public IEnumerable<TEntity> GetItems(object value, string columnName = "ID", string orderby = "")
        {
            using (SqlConnection conn = GetConnection())
            {
                string sql = $"Select * From {_tableName} Where {columnName} = @Param ";
                if (!string.IsNullOrEmpty(orderby))
                    sql += orderby;
                return conn.Query<TEntity>(sql, new { Param = value });
            }
        }

        public IEnumerable<TEntity> GetItems(int pageSize, int pageIndex, out int recordCount, object value, string columnName = "ID", string orderby = "")
        {
            string sql = GetItemsSql(value, columnName);

            return GetPagedData(sql, pageSize, pageIndex, out recordCount, new Dictionary<string, object>() { { columnName, value } }, string.IsNullOrEmpty(orderby) ? "" : $" ORDER BY {orderby} ");
        }

        public IEnumerable<TEntity> GetItems(Dictionary<string, object> param, string orderby = "")
        {
            string sql = GetItemSql(param, orderby);

            using (SqlConnection conn = GetConnection())
            {
                return conn.Query<TEntity>(sql, param);
            }
        }

        public IEnumerable<TEntity> GetItems(int pageSize, int pageIndex, out int recordCount, Dictionary<string, object> param, string orderby = "")
        {
            string sql = GetItemSql(param);

            return GetPagedData(sql, pageSize, pageIndex, out recordCount, param, string.IsNullOrEmpty(orderby) ? "" : $" ORDER BY {orderby} ");
        }

        private string GetItemsSql(object value, string columnName = "ID", string orderby = "")
        {
            string sql = $"Select * From {_tableName} Where {columnName} = @{columnName} ";
            if (!string.IsNullOrEmpty(orderby))
                sql += orderby;

            return sql;
        }

        private string GetItemSql(Dictionary<string, object> param, string orderby = "")
        {
            if (param.Count == 0)
                throw new Exception("參數不得為空");

            string whereClause = "";
            List<string> where = new List<string>();

            foreach (var item in param)
            {
                where.Add($" {item.Key} = @{item.Key} ");
            }
            whereClause = string.Join(" AND ", where);

            string sql = $"Select * From {_tableName} Where {whereClause} ";
            if (!string.IsNullOrEmpty(orderby))
                sql += orderby;

            return sql;
        }

        public IEnumerable<TEntity> GetAll()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("1", "1");
            return GetItems(param);
        }

        public IEnumerable<TEntity> GetAll(int pageSize, int pageIndex, out int recordCount, string orderBy = "")
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("1", "1");
            return GetItems(pageSize, pageIndex, out recordCount, param, orderBy);
        }

        public IEnumerable<TEntity> GetPagedData(string sql, int pageSize, int pageIndex, out int recordCount, Dictionary<string, object> param = null, string orderClause = "")
        {
            using(SqlConnection conn = GetConnection())
            {
                param = param ?? new Dictionary<string, object>();
                orderClause = string.IsNullOrEmpty(orderClause) ? " ORDER BY (SELECT NULL) " : orderClause;
                string recordCountSql = $@" SELECT SelectedRowNum 
                                            FROM (SELECT ROW_NUMBER() OVER ({orderClause}) as SelectedRowNum, * 
                                                  FROM ( {sql} ) AS result 
                                                  ) AS result2
";
                recordCount = conn.Query<TEntity>(recordCountSql, param).Count();
                int offset = (pageIndex - 1) * pageSize;

                string fullSql = $@" SELECT * 
                                     FROM (SELECT ROW_NUMBER() OVER ({orderClause}) as SelectedRowNum, * 
                                           FROM ( {sql} ) AS result 
                                           ) AS result2
                                     ORDER BY SelectedRowNum OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY
";

                return conn.Query<TEntity>(fullSql, param);
            }
        }

        /// <summary>
        /// 將項目新增至資料庫
        /// </summary>
        /// <param name="item">新增物件</param>
        /// <param name="identityColumnName">識別欄位名稱</param>
        /// <param name="isWithIdentity">新增紀錄時是否寫入識別欄位</param>
        public long CreateItem(TEntity item, string identityColumnName = "ID", bool isWithIdentity = true)
        {
            using (SqlConnection conn = GetConnection())
            {
                IEnumerable<PropertyInfo> info = typeof(TEntity)
                    .GetProperties()
                    .Where(m => m.PropertyType.Name != typeof(IEnumerable<>).Name &&
                        m.PropertyType.Name != typeof(List<>).Name &&
                        m.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && // 不加入被設為 NotMapped 的欄位 
                        (isWithIdentity ? true : m.Name != identityColumnName) &&// 如果不加入 identity 的話就只選擇名稱和 Identity Column Name 不同的欄位
                        m.GetValue(item) != null); // 不加入沒有被寫入值的欄位

                string returnIdSql = "";
                if (isWithIdentity)
                {
                    try
                    {
                        var prop = typeof(TEntity).GetProperty(identityColumnName);
                        returnIdSql = $" SELECT {prop.GetValue(item)} ";
                    }
                    catch (Exception)
                    {
                        returnIdSql = $" SELECT 0 ";
                    }
                }
                else
                    returnIdSql = $" SELECT IDENT_CURRENT ('{_tableName}') as ID";

                string setIdentityInsertOn = isWithIdentity ? $@" if exists(select 1 from sys.columns c where c.object_id = object_id('{_tableName}') and c.is_identity =1)
                            Set Identity_Insert [dbo].[{_tableName}] ON " : "";

                string setIdentityInsertOff = isWithIdentity ? $@" if exists(select 1 from sys.columns c where c.object_id = object_id('{_tableName}') and c.is_identity =1)
                            Set Identity_Insert [dbo].[{_tableName}] OFF " : "";

                string sql = $@" 
                        {setIdentityInsertOn}

                        INSERT INTO {_tableName}({string.Join(", ", info.Select(m => $"[{m.Name}]"))}) 
                        VALUES({string.Join(", ", info.Select(m => $"@{m.Name}"))}) 

                        {setIdentityInsertOff}

                        {returnIdSql}
";

                return conn.Query<long>(sql, item).SingleOrDefault();
            }
        }

        public void UpdateItemExcept(TEntity item, string[] updateExceptColumns, string identityColumnName = "ID")
        {
            using(SqlConnection conn = GetConnection())
            {
                IEnumerable<string> updateInaertColumns = typeof(TEntity).GetProperties()
                    .Where(p=>p.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && !updateExceptColumns.Contains(p.Name))
                    .Select(m => $"{m.Name} = @{m.Name} "  )
                                                                              ;
                
                string sql = $" UPDATE {_tableName} SET {string.Join(", ", updateInaertColumns)} WHERE {identityColumnName} = @{identityColumnName} ";
                //WorkLib.WriteLog.Write(true, sql);
                conn.Execute(sql, item);
            }
        }

        public void UpdateItem(TEntity item, string[] updateColumns, string identityColumnName = "ID")
        {
            using(SqlConnection conn = GetConnection())
            {
                IEnumerable<string> updateInaertColumns = typeof(TEntity).GetProperties()
                    .Where(m => updateColumns.Contains(m.Name)
                            && m.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0).Select(m => $"{m.Name} = @{m.Name} ");
                
                string sql = $" UPDATE {_tableName} SET {string.Join(", ", updateInaertColumns)} WHERE {identityColumnName} = @{identityColumnName} ";

                conn.Execute(sql, item);
            }
        }
        public void UpdateItem(TEntity item, string[] updateColumns, string[] identityColumnName)
        {
            using (SqlConnection conn = GetConnection())
            {
                string whereCond = "";
                IEnumerable<string> updateInaertColumns = typeof(TEntity).GetProperties()
                    .Where(m => updateColumns.Contains(m.Name)
                            && m.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0).Select(m => $"{m.Name} = @{m.Name} ");
                for (int i = 0; i < identityColumnName.Length; i++)
                {
                    whereCond += (string.IsNullOrEmpty(whereCond)?"":" AND ")+string.Format(" {0}=@{0} ", identityColumnName[i]);
                }
                string sql = $" UPDATE {_tableName} SET {string.Join(", ", updateInaertColumns)} WHERE {whereCond} ";

                conn.Execute(sql, item);
            }
        }

        public void SetItem(TEntity item, bool isNew, string[] updateExceptColumns, string identityColumnName = "ID")
        {
            if (isNew)
                CreateItem(item);
            else
                UpdateItemExcept(item, updateExceptColumns, identityColumnName);
        }

        public int Delete(IEnumerable<long> ids, string columnName = "ID")
        {
            if (ids == null || ids.Count() == 0)
                return 0;

            using(SqlConnection conn = GetConnection())
            {
                string sql = $"Delete {_tableName} Where {columnName} In ({string.Join(",", ids)})";
                return conn.Execute(sql);
            }
        }

        public int Delete(object value, string columnName = "ID")
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    string sql = $"Delete {_tableName} Where {columnName} = @Value";
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("Value", value);

                    return conn.Execute(sql, param);
                }
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 複製指定資料
        /// </summary>
        /// <param name="ids">識別欄位資料</param>
        /// <param name="copyColumns">要複製的欄位</param>
        /// <param name="insertValue">複製的欄位要寫入的值</param>
        /// <param name="identityColumnName">識別欄位名稱</param>
        public virtual void CopyItem(IEnumerable<long> ids, string[] copyColumns, Dictionary<string, object> insertValue, string identityColumnName = "ID", bool generateIdentityColumn = true)
        {
            try
            {
                if (copyColumns.Contains(identityColumnName))
                    throw new Exception("不可複製識別欄位");

                IEnumerable<TEntity> items = GetItemsIn(ids, identityColumnName);
                List<TEntity> newItems = new List<TEntity>();

                foreach (var item in items)
                {
                    TEntity newItem = new TEntity();
                    foreach (PropertyInfo info in typeof(TEntity).GetProperties())
                    {
                        if (info.Name == identityColumnName && generateIdentityColumn)
                        {
                            info.SetValue(newItem, WorkLib.GetItem.NewSN());
                        }

                        if (!copyColumns.Contains(info.Name) && !insertValue.ContainsKey(info.Name))
                            continue;

                        if (!insertValue.ContainsKey(info.Name))
                        {
                            var value = info.GetValue(item);
                            info.SetValue(newItem, value);
                        }
                        else
                        {
                            info.SetValue(newItem, insertValue[info.Name]);
                        }
                    }

                    newItems.Add(newItem);
                }

                using (var scope = new TransactionScope())
                {
                    foreach (var item in newItems)
                    {
                        CreateItem(item);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        public virtual void Toggle(long id, string columnName = "IsIssue", string specificColumn = "ID")
        {
            using (SqlConnection conn = GetConnection())
            {
                string sql = $@" UPDATE {_tableName}
                                    SET {columnName} = ({columnName} + 1) % 2
                                 WHERE {specificColumn} = @Value";

                conn.Execute(sql, new { Value = id });
            }
        }

        public virtual void Sort(IEnumerable<Common.SortItem> items, string where = "", string specifyColumnName = "ID", string sortColumnName = "Sort")
        {
            Areas.Backend.Models.DataAccess.CommonDA.Sort(_tableName, items, where, specifyColumnName, sortColumnName);
        }

        public int GetMaxSort(string sortColumnName = "Sort")
        {
            var orderByParameter = Expression.Parameter(typeof(TEntity));
            var orderByExpression = Expression.Lambda<Func<TEntity, int>>(Expression.PropertyOrField(orderByParameter, sortColumnName), orderByParameter).Compile();

            IEnumerable<TEntity> cates = GetAll().OrderByDescending(orderByExpression);

            var cate = cates.FirstOrDefault();
            Type t = typeof(TEntity);
            PropertyInfo prop = t.GetProperty(sortColumnName);
            return cates.Count() == 0 ? 0 : (int)prop.GetValue(cate);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public string CreateItemSql(TEntity item, string identityColumnName = "ID", bool isWithIdentity = true)
        {
            IEnumerable<PropertyInfo> info = typeof(TEntity)
                .GetProperties()
                .Where(m => m.PropertyType.Name != typeof(IEnumerable<>).Name &&
                    m.PropertyType.Name != typeof(List<>).Name &&
                    m.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && // 不加入被設為 NotMapped 的欄位 
                    (isWithIdentity ? true : m.Name != identityColumnName) &&// 如果不加入 identity 的話就只選擇名稱和 Identity Column Name 不同的欄位
                    m.GetValue(item) != null); // 不加入沒有被寫入值的欄位

            string sql = $@" 
                    INSERT INTO {_tableName}({string.Join(", ", info.Select(m => m.Name))}) VALUES({string.Join(", ", info.Select(m => $"@{m.Name}"))}) 
                    ";
            return sql;
        }

        public string UpdateItemSql(TEntity item, string[] updateColumns, string identityColumnName = "ID")
        {
            IEnumerable<string> updateInaertColumns = typeof(TEntity).GetProperties()
                .Where(m => updateColumns.Contains(m.Name)
                        && m.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0).Select(m => $"{m.Name} = @{m.Name} ");

            string sql = $" UPDATE {_tableName} SET {string.Join(", ", updateInaertColumns)} WHERE {identityColumnName} = @{identityColumnName} ";

            return sql;
        }

        public string UpdateItemSql(TEntity item, string[] updateColumns, string[] identityColumnName)
        {
            string whereCond = "";
            IEnumerable<string> updateInaertColumns = typeof(TEntity).GetProperties()
                .Where(m => updateColumns.Contains(m.Name)
                        && m.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0).Select(m => $"{m.Name} = @{m.Name} ");
            for (int i = 0; i < identityColumnName.Length; i++)
            {
                whereCond += (string.IsNullOrEmpty(whereCond) ? "" : " AND ") + string.Format(" {0}=@{0} ", identityColumnName[i]);
            }
            string sql = $" UPDATE {_tableName} SET {string.Join(", ", updateInaertColumns)} WHERE {whereCond} ";

            return sql;
        }

        public string UpdateItemExceptSql(TEntity item, string[] updateExceptColumns, string identityColumnName = "ID")
        {
            IEnumerable<string> updateInaertColumns = typeof(TEntity).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && !updateExceptColumns.Contains(p.Name))
                .Select(m => $"{m.Name} = @{m.Name} ")
                                                                          ;

            string sql = $" UPDATE {_tableName} SET {string.Join(", ", updateInaertColumns)} WHERE {identityColumnName} = @{identityColumnName} ";
            return sql;
        }

        public string DeleteSql(IEnumerable<long> ids, string columnName = "ID")
        {
            if (ids == null || ids.Count() == 0)
                return"";

            string sql = $"Delete {_tableName} Where {columnName} In ({string.Join(",", ids)})";
            return sql;
        }

        public ColumnType GetMaxNumber<ColumnType>(string sortColumnName = "Sort")
        {
            var orderByParameter = Expression.Parameter(typeof(TEntity));
            var orderByExpression = Expression.Lambda<Func<TEntity, ColumnType>>(Expression.PropertyOrField(orderByParameter, sortColumnName), orderByParameter).Compile();

            IEnumerable<TEntity> cates = GetAll().OrderByDescending(orderByExpression);

            var cate = cates.FirstOrDefault();
            Type t = typeof(TEntity);
            PropertyInfo prop = t.GetProperty(sortColumnName);
            return cates.Count() == 0 ? default(ColumnType) : (ColumnType)prop.GetValue(cate);
        }
    }
}