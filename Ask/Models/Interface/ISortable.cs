using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Interface
{
    public interface ISortable
    {
        void Sort(IEnumerable<WorkV3.Common.SortItem> items, string where = "", string specifyColumnName = "ID", string sortColumnName = "Sort");
        int GetMaxSort(string sortColumnName = "Sort");
    }
}