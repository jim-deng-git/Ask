using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.ViewModels.Intro
{
    public class IntroListVIewModel
    {
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        public long PageNo { get; set; }
        public List<CardsViewModel> Cards { get; set; }
        public IEnumerable<FormModel> Forms { get; set; }
        public string FormTitle { get; set; }
    }
}