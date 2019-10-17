using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 自訂廣告管理
    /// </summary>
    public class AdsCustomizeModel
    {
        public long ID { get; set; }
        /// <summary>
        /// 廣告(區)主檔 ID
        /// </summary>
        public long Advertisement_ID { get; set; }
        /// <summary>
        /// 廣告主 ID
        /// </summary>
        public long? Advertisers_ID { get; set; }
        /// <summary>
        /// 廣告說明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 點擊事件
        /// </summary>
        public string ClickEvent { get; set; }
        /// <summary>
        /// 是否手機版圖片引用電腦版
        /// </summary>
        public bool PhonePicFollowPC { get; set; }
        /// <summary>
        /// 是否刊登
        /// </summary>
        public bool IsIssue { get; set; }
        /// <summary>
        /// 電腦板圖片
        /// </summary>
        public string PCPicture { get; set; }
        /// <summary>
        /// 手機板圖片
        /// </summary>
        public string MobilePicture { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        public long CastDivideDay { get; set; }
        public long CastDivideMonth { get; set; }
        /// <summary>
        /// 紀錄自訂廣告編輯時的廣告主ID
        /// </summary>
        public string AdvertiserId { get; set; }
        /// <summary>
        /// 計算所有點擊數
        /// </summary>
        public long? TotalClickCount
        {
            get
            {
                return AdsCustomizeAccountSet.Sum(m => m.QueryClicks);
            }
        }
        public long? TotalVisitCount
        {
            get
            {
                return AdsCustomizeAccountSet.Sum(m => m.QueryVisits);
            }
        }
        /// <summary>
        /// 自訂廣告管理 帳務列表
        /// </summary>
        public IEnumerable<AdsCustomizeAccountSet> AdsCustomizeAccountSet
        {
            get
            {
                //long test = 201803091106;
                return AdvertisementDAO.QueryAccountSetByAdsCustomizeID(ID);
            }
         }
        /// <summary>
        /// 查詢廣告主
        /// </summary>
        /// <returns></returns>
        public AdvertisersModel QueryAndGetAdvertiser
        {
            get
            {
                AdvertisersModel Data = AdvertisementDAO.GetAdvertisersItem(Advertisers_ID ?? 0);
                if (Data != null)
                    return Data;
                else
                    return new AdvertisersModel();
            }
        }
        /// <summary>
        /// 查詢所屬的廣告主檔資訊
        /// </summary>
        public AdvertisementModel QueryRelationAdvertisement
        {
            get
            {
                if(ID != 0 && Advertisement_ID != 0)
                {
                    AdvertisementModel data ;
                    data = AdvertisementDAO.GetAdvertisementItem(Advertisement_ID);
                    if(data != null)
                        return data;
                }
                return new AdvertisementModel();
            }
        }
        public AdsCustomizeToLinkModel GetLinkData
        {
            get
            {
                return AdvertisementDAO.GetAdsCustomizeLinkItem(ID);
       
            }
        }
        public AdsCustomizeToVideoModel GetVideoData
        {
            get
            {
                return AdvertisementDAO.GetAdsCustomizeVideoItem(ID);
            }
        }
    }
}