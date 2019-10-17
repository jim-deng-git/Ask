using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ProjectAdminToCheckInModel
    {
        public long WorkID { get; set; }
        public bool IsFromMobile { get; set; }
        public string CheckTimeStart { get; set; }
        public string CheckTimeEnd { get; set; }
        public DateTime? ActualCheckStart { get; set; }
        public DateTime? ActualCheckEnd { get; set; }
        public int? HourSalary { get; set; }
        public int? Source { get; set; }
    }

    public enum Source
    {
        [Description("客戶請款")]
        Client = 1,
        [Description("公司吸收")]
        Company
    }
}