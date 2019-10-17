using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WorkV3.Models;
using WorkV3.Models.DataAccess;

namespace WorkV3.Areas.Backend.Models
{
    [Table(Name = "RewardPlace")]
    public class RewardPlaceModel
    {
        public long ID { get; set; }

        public long SiteID { get; set; }


        public int Sort { get; set; } = int.MaxValue;

        public string Name { get; set; }

        public string Img { get; set; }

        public string Introduce { get; set; }
        
        public string Phone { get; set; }

        public string Regions { get; set; }
        public string Address { get; set; }

        public string ServiceTime { get; set; }

        public bool IsIssue { get; set; } = true;
        
        public long Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public long Modifier { get; set; }

        public DateTime? ModifyTime { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }

       

    }
}