using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Interface
{
    public interface IChatRoomRepository<TEntity> : IGenericRepository<TEntity>, IDisposable
    {

    }
}