using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberToCompanyModel
    {
        public long MemberID { get; set; }
        public int? CompanyID { get; set; }
        public int? DepartmentID { get; set; }

        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
    }
}