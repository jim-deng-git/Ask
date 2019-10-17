using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class StorePlanServiceRespository : RepositoryBase<StorePlanServiceModel>, Interface.IStorePlanServiceRespository<StorePlanServiceModel>
    {
        public StorePlanServiceRespository()
        {
            SetTableName("StorePlanService");
        }

        public static void Sort(IEnumerable<Common.SortItem> items, long parentId)
        {
            Areas.Backend.Models.DataAccess.CommonDA.Sort("StorePlanService", items, "ParentID=" + parentId);
        }
    }
}