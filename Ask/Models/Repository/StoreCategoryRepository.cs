using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class StoreCategoryRepository : RepositoryBase<StoreCategoryModel>, Interface.IStoreCategoryRepository<StoreCategoryModel>
    {
        public StoreCategoryRepository()
        {
            SetTableName("StoreCategory");
        }

        public static void Sort(IEnumerable<Common.SortItem> items)
        {
            Areas.Backend.Models.DataAccess.CommonDA.Sort("StoreCategory", items, "");
        }
    }
}