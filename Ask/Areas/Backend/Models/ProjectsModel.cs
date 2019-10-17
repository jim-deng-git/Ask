using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ProjectsModel
    {
        public long ID { get; set; }
        public string ProjectNumber { get; set; }
        public long SiteID { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string OnSiteContactName { get; set; }
        public string OnSiteContactPhone { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public string Regions { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public int? CompanyID { get; set; }
        public int? DepartmentID { get; set; }

        public string IDStr { get; set; }
    }
}