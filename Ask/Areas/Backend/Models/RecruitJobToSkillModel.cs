using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class RecruitJobToSkillModel
    {
        public long ID { get; set; }
        public long RecruitJobID { get; set; }
        public string SkillID { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }

        public string Photo { get; set; }
    }
}