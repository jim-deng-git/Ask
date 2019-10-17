using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 欄位 資料模型
    /// </summary>
    public class RewardCollectViewModel
    {
        /// <summary>
        /// 欄位識別碼
        /// </summary>
        [PKey]
        public long ID { get; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }

        public string KeyType { get; set; }

        public string KeyValue { get; set; }
                
        public long RewardID { get; set; }

        public long RewardPlaceID { get; set; }

        public DateTime CreateTime { get; set; }

    }
    
}