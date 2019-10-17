using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class ProjectAdminCheckInsViewModel
    {
        public long ProjectAdminID { get; set; }
        public long RecruitID { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}