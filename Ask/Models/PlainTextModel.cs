using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WorkV3.Models
{
    [Table(Name = "PlainText")]
    public class PlainTextModel
    {
        [Required]
        [PKey]
        public long ID { get; set; }

        [Required]
        public long CardNo { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; } = "";

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; } = "";

        [Required]
        public bool IsIssue { get; set; } = true;

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