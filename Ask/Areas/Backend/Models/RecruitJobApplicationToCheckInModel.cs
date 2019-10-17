using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class RecruitJobApplicationToCheckInModel
    {
        public long WorkID { get; set; }
        public bool IsFromMobile { get; set; }
        public string CheckTimeStart { get; set; }
        public string CheckTimeEnd { get; set; }
        public DateTime? ActualCheckStart { get; set; }
        public DateTime? ActualCheckEnd { get; set; }
        public int? HourSalary { get; set; }
        public string CheckInPhoto { get; set; }
        public string CheckOutPhoto { get; set; }
        public int Source { get; set; }
        public string Note { get; set; }

        public class ImageModel
        {
            public string ID { get; set; }
            public string FileName { get; set; }
            public string Img { get; set; }
            public bool IsShow { get; set; }
            public bool IsOpenNew { get; set; }
        }
    }
}