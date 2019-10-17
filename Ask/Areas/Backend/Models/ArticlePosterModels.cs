using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WorkV3.Areas.Backend.Models
{
    public class ArticlePosterModels
    {
        public long ID { get; set; }
        public long MenuID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Photo { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Intro { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Resume { get; set; }
        public bool IsIssue { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}