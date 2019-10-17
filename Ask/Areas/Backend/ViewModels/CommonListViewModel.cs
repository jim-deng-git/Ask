using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Common;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class CommonListViewModel<TListItem>
    {
        public Pagination Pager { get; set; }
        public IEnumerable<TListItem> Items { get; set; }
        public long SiteID { get; set; }
    }
}