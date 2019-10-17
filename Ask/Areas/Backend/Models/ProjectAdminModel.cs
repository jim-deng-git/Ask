using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ProjectAdminModel
    {
        public long ID { get; set; }
        public long ProjectID { get; set; }
        public long MemberID { get; set; }
        public int Position { get; set; }
        public int Sort { get; set; }

        [NotMapped]
        public int CompanyID { get; set; }
        [NotMapped]
        public int DepartmentID { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string LoginID { get; set; }
        [NotMapped]
        public long GroupId { get; set; }
    }

    public enum Position
    {
        [Description("專案負責人")]
        Master = 1,
        [Description("次要負責人")]
        SecondMaster,
        [Description("專案人員")]
        Staff
    }
}