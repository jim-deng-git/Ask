using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3.Models.Interface
{
    public interface IZoneRepository<TEntity> : IDisposable, IGenericRepository<TEntity>
    {
    }
}
