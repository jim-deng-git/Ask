using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Interface
{
    public interface IStoreRepository<TEntity, TSearchEntity> : IDisposable, IToggleable, IGenericRepository<TEntity>
    {
        IEnumerable<TEntity> GetPagedData(TSearchEntity search, int pageSize, int pageIndex, out int recordCount, bool showNotIssued = true);
    }
}