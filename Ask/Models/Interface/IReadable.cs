using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3.Models.Interface
{
    public interface IReadable<TEntity>: IDisposable
    {
        TEntity GetItem(object value, string columnName = "ID");
        TEntity GetItem(Dictionary<string, object> param);
        IEnumerable<TEntity> GetItems(object value, string columnName, string orderby = "");
        IEnumerable<TEntity> GetItems(int pageSize, int pageIndex, out int recordCount, object value, string columnName = "ID", string orderby = "");
        IEnumerable<TEntity> GetItems(Dictionary<string, object> param, string orderby = "");
        IEnumerable<TEntity> GetItems(int pageSize, int pageIndex, out int recordCount, Dictionary<string, object> param, string orderby = "");

        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetAll(int pageSize, int pageIndex, out int recordCount, string orderBy = "");
        IEnumerable<TEntity> GetPagedData(string sql, int pageSize, int pageIndex, out int recordCount, Dictionary<string, object> param = null, string orderClause = "");
        ColumnType GetMaxNumber<ColumnType>(string sortColumnName = "Sequence");
    }
}
