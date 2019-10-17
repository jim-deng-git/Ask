using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AdvertiserStatisticsViewModel
    {
        public long AdvertiserID { get; set; }
        public string CompanyName { get; set; }
        public string ContactInfo { get; set; }
        public int ClickCount { get; set; }
        public int BrowseCount { get; set; }
        public long FeeEstimate { get; set; }
        public double CP { get; set; }
    }
}