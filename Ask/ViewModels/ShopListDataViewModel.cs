using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Common;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    public class ShopListDataViewModel
    {
        public IEnumerable<EshopProductModel> EshopProducts { get; set; }
        public IEnumerable<EshopProductCategoryModel> Categories { get; set; }
        public IEnumerable<ShopThemeModel> EshopProductThemes { get; set; }
        public IEnumerable<InterestModel> Interests { get; set; }
        public Pagination Pager { get; set; }
        public string SiteSN { get; set; }
        public long SiteID { get; set; }
        public string UploadUrl { get; set; }
    }
}