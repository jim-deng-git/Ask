using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WorkV3.Models
{
    [Table(Name = "MainVision")]
    public class MainVisionModel
    {
        [PKey]
        [Required]
        public long ID { get; set; }

        [Required]
        public long CardNo { get; set; }

        public string Type { get; set; }

        //[Required, DisplayFormat(ConvertEmptyStringToNull = false)] //2017-12-19 CC 說改為 Title 非必填
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; }

        [Required]
        public int Sort { get; set; } = int.MaxValue;

        [AllowHtml, DisplayFormat(ConvertEmptyStringToNull = false)]
        // [Required(ErrorMessage = "電腦版必填")]
        public string Img { get; set; } = "";

        [AllowHtml, DisplayFormat(ConvertEmptyStringToNull = false)]
        // [Required(ErrorMessage = "手機版必填")]
        public string ImgM { get; set; } = "";

        [Required(ErrorMessage = "輪播秒數必填")]
        public int Second { get; set; } = 5;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Link { get; set; }

        [Required]
        public bool IsOpenNew { get; set; }

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

        public string VideoID { get; set; }
        public bool VideoImgIsCustom { get; set; }

        public string VideoImg { get; set; }
        public string VideoImgM { get; set; }
        public string Content { get; set; }

        public string Icon { get; set; }

        public string HoverIcon { get; set; }

        public int Clicks { get; set; }

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