using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class StorePlanRespository : RepositoryBase<StorePlanModel>, Interface.IStorePlanRespository<StorePlanModel>
    {
        public StorePlanRespository()
        {
            SetTableName("StorePlan");
        }

        public static void Sort(IEnumerable<Common.SortItem> items)
        {
            Areas.Backend.Models.DataAccess.CommonDA.Sort("StorePlan", items, "");
        }
    }
}