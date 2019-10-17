using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Models
{
    [Table(Name = "Parallax")]
    public class ParallaxModel
    {
        [PKey]
        [Required]
        public long ID { get; set; }

        [Required]
        public long CardNo { get; set; }

        [Required]
        public int Sort { get; set; } = int.MaxValue;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; } = "";

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; } = "";

        [AllowHtml]
        [Required(ErrorMessage = "電腦版必填")]
        public string Img { get; set; } = "";

        [Required]
        public bool IsIssue { get; set; } = true;

        public DateTime? IssueStart { get; set; }

        public DateTime? IssueEnd { get; set; }

        [Required]
        public bool Period { get; set; } = false;

        [Required]
        public bool Mon { get; set; } = true;

        [Required]
        public bool Tue { get; set; } = true;

        [Required]
        public bool Wed { get; set; } = true;

        [Required]
        public bool Thu { get; set; } = true;

        [Required]
        public bool Fri { get; set; } = true;

        [Required]
        public bool Sat { get; set; } = true;

        [Required]
        public bool Sun { get; set; } = true;

        public DateTime? DailyStart { get; set; }

        public DateTime? DailyEnd { get; set; }

        [Required]
        public bool Website1 { get; set; } = false;

        [Required]
        public bool Website2 { get; set; } = false;

        [Required]
        public long Creator { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public long Modifier { get; set; }

        [Required]
        public DateTime ModifyTime { get; set; }
    }
}