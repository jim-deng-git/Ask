using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ProjectAdminWorksModel
    {
        public long ID { get; set; }
        public long ProjectAdminID { get; set; }
        public long RecruitID { get; set; }
        public DateTime CheckDate { get; set; }
        public double Hours { get; set; }
        public int Salary { get; set; }
        public int WeekdayHours { get; set; }
        public int HolidaySalary { get; set; }
        public int FinalSalary { get; set; }
        public string Note { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }   
        
        [NotMapped]
        public long MemberID { get; set; }
    }
}