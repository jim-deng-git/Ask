using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 顯示位置設定
    /// </summary>
    public class AdsDisplayAreaSetModel
    {
        public long ID { get;set;}
        /// <summary>
        /// 母顯示位置設定ID
        /// </summary>
        public long? ParentAdsDisplayAreaSetID { get; set; }
        /// <summary>
        /// 子資料類型
        /// </summary>
        public string ChildType { get; set; }
        /// <summary>
        /// 廣告區 ID
        /// </summary>
        public long Advertisement_ID { get;set;}
        /// <summary>
        /// 顯示位置
        /// </summary>
        public string GroupPosition { get;set;}
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        /// <summary>
        /// 蓋台設定種類
        /// </summary>
        public string OverlayType { get; set; }
        /// <summary>
        /// 蓋台設定_蓋台機率
        /// </summary>
        public int? OverlayChance { get; set; }
        /// <summary>
        /// 蓋台設定_播放後下次再播放的間隔秒數
        /// </summary>
        public int? OverlayRepeatSeconds { get; set; }
        /// <summary>
        /// 蓋台設定_閒置幾秒後播放
        /// </summary>
        public int? OverlayIdleSeconds { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 內頁設定
        /// </summary>
        public AdsDisplayAreaSetModel InsideAreaSet { get; set; }
        /// <summary>
        /// 會員 - 登入位置
        /// </summary>
        public AdsDisplayAreaSetModel LoginAreaSet { get; set; }

        public string DataType { get; set; }
    }
}