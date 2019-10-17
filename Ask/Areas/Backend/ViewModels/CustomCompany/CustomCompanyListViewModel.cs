using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Common;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.ViewModels.CustomCompany
{
    public class CustomCompanyListViewModel
    {
        public Pagination Pagination { get; set; }
        public IEnumerable<CustomCompanyModel> Companies { get; set; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
    }
}