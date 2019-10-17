using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class RewardCollectInfoViewModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        public string KeyType { get; set; }
        public string KeyValue { get; set; }
        public long RewardID { get; set; }
        public bool IsWonPrice { get; set; }
        public bool IsComplete { get; set; }
        public bool CompleteTime { get; set; }
        public string CompleteData { get; set; }
        public bool IsProcess { get; set; }
        public string ProcessRemark { get; set; }
        public DateTime? ProcessTime { get; set; }
        public string Remark { get; set; }
        public int CollectCount { get; set; }
        public DateTime LatestCollectTime { get; set; }
        public DateTime CreateTime { get; set; }

        public IEnumerable<string> GetFlags(long siteId)
        {
            return WorkV3.Models.DataAccess.UserFlagDAO.GetFlags(siteId, ID.ToString());
        }
    }
}