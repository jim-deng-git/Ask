using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Abstracts;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class GroupPermissionViewModel
    {
        public long SiteID { get; set; }
        public IEnumerable<Menu> menus { get; set; }
    }
}