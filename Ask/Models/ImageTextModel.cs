using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WorkV3.Models
{
    [Table(Name = "ImageText")]
    public class ImageTextModel
    {
        [PKey]
        [Required]
        public long ID { get; set; }

        [Required]
        public long CardNo { get; set; }

        [Required]
        public int Sort { get; set; } = int.MaxValue;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; }

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        
        [AllowHtml]
        [Required(ErrorMessage = "請上傳圖片")]
        public string Img { get; set; }

        [Required]
        public int ClickType { get; set; } = 0;

        [Required]
        public string ClickEvent { get; set; } = "{}";

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
        public long Creator { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public long Modifier { get; set; }

        [Required]
        public DateTime ModifyTime { get; set; }

        public int Clicks { get; set; }
        public bool ShowClicks { get; set; } = true;
        public int StartClicks { get; set; }
        public string Link { get; set; }
        public bool IsOpenNew { get; set; }        
    }
}