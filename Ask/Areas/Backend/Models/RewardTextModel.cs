using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
   
        [Table(Name = "RewardText")]
    public class RewardTextModel
    {
        public string ID { get; set; }

        public long SiteID { get; set; }

        public string Text { get; set; }

        public long Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public long Modifier { get; set; }

        public DateTime? ModifyTime { get; set; } 

    }
}