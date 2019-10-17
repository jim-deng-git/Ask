using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Interface
{
    public interface IPointRepository<TEntity> 
    {
        TEntity GetPoint(long membershipId);
        bool TransferPoint(long membershipIdFrom, long membershipIdTo, float point);
        bool AddPoint(long membershipId, float point);
        bool IsPointEnough(long membershipId, float point);
        bool SubtractPoint(long membershipId, float point);
    }
}