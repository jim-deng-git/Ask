using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AdsPositionSetViewModel
    {
        public long AdvertisementId { get; set; }
        public long SiteId { get; set; }
        public long MenuId { get; set; }
        public string DataType { get; set; }
        /// <summary>
        /// 列表頁選擇的所有位置
        /// </summary>
        public string[] ListGroup { get; set; }
        /// <summary>
        /// 內容頁選擇的所有位置
        /// </summary>
        public string[] InsideGroup { get; set; }
        /// <summary>
        /// 會員頁選擇的位置
        /// </summary>
        public string[] LoginGroup { get; set; }
        /// <summary>
        /// 列表頁蓋台設定
        /// </summary>
        public string ListOverlayType { get; set; }
        /// <summary>
        /// 內頁蓋台設定
        /// </summary>
        public string InsideOverlayType { get; set; }
        /// <summary>
        /// 會員頁蓋台設定
        /// </summary>
        public string LoginOverlayType { get; set; }
        /// <summary>
        /// 列表頁蓋台設定：出現機率
        /// </summary>
        public int? ListOverlayChance { get; set; }
        /// <summary>
        /// 列表頁蓋台設定：隔多少時間再次出現
        /// </summary>
        public int? ListOverlayRepeatSeconds { get; set; }
        /// <summary>
        /// 列表頁蓋台設定：閒置多少後出現
        /// </summary>
        public int? ListOverlayIdleSeconds { get; set; }
        /// <summary>
        /// 內頁蓋台設定：出現機率
        /// </summary>
        public int? InsideOverlayChance { get; set; }
        /// <summary>
        /// 內頁蓋台設定：隔多少時間再次出現
        /// </summary>
        public int? InsideOverlayRepeatSeconds { get; set; }
        /// <summary>
        /// 內頁蓋台設定：閒置多少後出現
        /// </summary>
        public int? InsideOverlayIdleSeconds { get; set; }
        /// <summary>
        /// 會員頁蓋台設定：出現機率
        /// </summary>
        public int? LoginOverlayChance { get; set; }
        /// <summary>
        /// 會員頁蓋台設定：隔多少時間再次出現
        /// </summary>
        public int? LoginOverlayRepeatSeconds { get; set; }
        /// <summary>
        /// 會員頁蓋台設定：閒置多少後出現
        /// </summary>
        public int? LoginOverlayIdleSeconds { get; set; }
    }
}