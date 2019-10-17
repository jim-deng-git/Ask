using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Interface
{
    public interface IGenericRepository<TEntity> : ICreatable<TEntity>, IReadable<TEntity>, IUpdatable<TEntity>, IDeletable<TEntity>
    {
    }
}