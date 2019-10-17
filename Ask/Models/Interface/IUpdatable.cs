using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3.Models.Interface
{
    public interface IUpdatable<TEntity>: IDisposable
    {
        void UpdateItemExcept(TEntity item, string[] updateExceptColumns, string identityColumnName = "ID");
        void UpdateItem(TEntity item, string[] updateColumns, string identityColumnName = "ID");
    }
}
