using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class RewardSettingAndPlaceModel
    {
        public List<RewardSettingModel> RewardSettingModel { get; set; }
        public List<RewardPlaceModel> RewardPlaceModel { get; set; }
    }
        [Table(Name = "RewardSetting")]
    public class RewardSettingModel
    {
        public long ID { get; set; }

        public long SiteID { get; set; }

        public string Color { get; set; }

        public int Sort { get; set; } = int.MaxValue;

        public string Title { get; set; }

        public bool IsIssue { get; set; } = true;
        
        public long Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public long Modifier { get; set; }

        public DateTime? ModifyTime { get; set; } 

    }
}