using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class RecruitJobToOrganizerModel
    {
        public long ID { get; set; }
        public long RecruitJobID { get; set; }
        public string OrganizerID { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}