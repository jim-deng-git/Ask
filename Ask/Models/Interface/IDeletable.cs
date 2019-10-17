using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3.Models.Interface
{
    public interface IDeletable<TEntity>: IDisposable
    {
        int Delete(IEnumerable<long> id, string columnName = "ID");
        int Delete(object value, string columnName = "ID");
    }
}
