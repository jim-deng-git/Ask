using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class RecruitDetailViewModel
    {
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        public Models.RecruitsModel Recruit { get; set; }
        public IEnumerable<Models.RecruitContactModel> RecruitToContact { get; set; }
        public IEnumerable<Models.RecruitJobsModel> Jobs { get; set; }
        public IEnumerable<Models.RecruitTypesModel> Types { get; set; }

    }
}