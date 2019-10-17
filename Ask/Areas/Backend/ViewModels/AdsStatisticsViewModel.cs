using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AdsStatisticsViewModel
    {
        /// <summary>
        /// 廣告位置
        /// </summary>
        public long AdsCustomizeID { get; set; }
        public long Advertisement_ID { get; set; } = 0;
        public string GroupPosition { get; set; }
        public string MenuTitle { get; set; }
        public int BrowseCount { get; set; }
        public int ClickCount { get; set; }
        public int FullPrice { get; set; }
        public double CP { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public bool PhonePicFollowPC { get; set; }
        public bool IsIssue { get; set; }
        public string PCPicture { get; set; }
        public string MobilePicture { get; set; }

        public AdvertisementModel QueryRelationAdvertisement
        {
            get
            {
                if (Advertisement_ID != 0)
                {
                    AdvertisementModel data;
                    data = AdvertisementDAO.GetAdvertisementItem(Advertisement_ID);
                    if (data != null)
                        return data;
                }
                return new AdvertisementModel();
            }
        }
    }
}