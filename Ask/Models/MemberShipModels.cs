using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class MemberShipModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string IDCard { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; } = true;
        public bool ResetPWD { get; set; } = false;
        /// <summary>
        /// 驗證別, 後來修改需發驗證信, 故加上欄位
        /// </summary>
        public MemberVerifyType VerifyType { get; set; }
        public string VerifyCode { get; set; }
        public string VerifyTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string LastResentVerifyMailTime { get; set; }
        #region 社群登入加的
        public MemberType MemberType { get; set; } = MemberType.Web;
        public string SocialID { get; set; }
        #endregion
        /// <summary>
        /// 商城點數
        /// </summary>
        public float Point { get; set; }
        /// <summary>
        /// 是否同意, 非資料庫欄位
        /// </summary>
        public bool AgreeDesc { get; set; } = false;
        public string RecommandCode { get; set; }
        public string PushRecommandCode { get; set; }
        public string PushRecommandMobile { get; set; }
        public List<Areas.Backend.Models.CategoryModels> IdenityTypeList { get; set; }
        public List<Areas.Backend.Models.CategoryModels> FavorityList { get; set; }
        public List<Areas.Backend.Models.CategoryModels> CareerList { get; set; }
        public List<Areas.Backend.Models.CategoryModels> MarriageList { get; set; }
        public List<Areas.Backend.Models.CategoryModels> EducationList { get; set; }
        public List<ViewModels.MemberCollectionViewModel> CollectionList { get; set; }
        public List<ViewModels.MemberCollectionMenuViewModel> CollectionMenuList { get; set; }
        public string Photo { get; set; } = "";
        public string SocialLargePhoto { get; set; } = "";
        public string Regions { get; set; }
        public string Address { get; set; }
        public string PermanentRegions { get; set; }
        public string PermanentAddress { get; set; }
        [NotMapped]
        public bool sameAddress { get; set; }
        public string Marriage { get; set; }
        public string Education { get; set; }
        public string Career { get; set; }
        public string Company { get; set; }
        public string EmergencyName { get; set; }
        public string EmergencyMobile { get; set; }
        public string EmergencyPhone { get; set; }
        public string EmergencyEmail { get; set; }

        public string ENName { get; set; }
        public string Nickname { get; set; }
        public string BloodType { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Vision { get; set; }
        public string LineID { get; set; }
        public string FacebookURL { get; set; }
        public string ChildrenStatus { get; set; }
        public string MilitaryService { get; set; }
        [NotMapped]
        public int? MilitaryService_Y { get; set; }
        [NotMapped]
        public int? MilitaryService_M { get; set; }
        public string SchoolAndDepartment { get; set; }
        public string ClubExp { get; set; }
        public string WorkExp { get; set; }
        public string ReferrerName { get; set; }
        public string AvailableTime { get; set; }
        public string BankAccount { get; set; }
        public string BankBook { get; set; }
        public string IDcardPhoto { get; set; }
        [NotMapped]
        public string IDcardPhoto_Front { get; set; }
        [NotMapped]
        public string IDcardPhoto_Back { get; set; }
        public string DailyPhoto { get; set; }
        public string MobileVerifyCode { get; set; }

        public bool OrderEpaper { get; set; }
        public bool IsPassportNo { get; set; }
        public bool Unemployed { get; set; }
        public string LegalPersonName { get; set; }
        public string LegalPersonMobile { get; set; }
        public DateTime? ExpireDate { get; set; }
        public ViewModels.PointsRecordViewModel PointsRecord { get; set; }
        public string DeviceToken { get; set; }

        public IEnumerable<string> GetFlags(long siteId)
        {
            return WorkV3.Models.DataAccess.UserFlagDAO.GetFlags(siteId, ID.ToString());
        }
    }

    public class MemberShipSearchModels {
        public string Key { get; set; }
        public long SiteID { get; set; }
        public string Sort { get; set; }
        public DateTime? StartRegTime { get; set; }
        public DateTime? EndRegTime { get; set; }
        public DateTime? StartLoginTime { get; set; }
        public DateTime? EndLoginTime { get; set; }
        public string[] IdenityList { get; set; }
        public string[] FavorityList { get; set; }
    }
    public enum MemberType
    {
        Web = 0,
        FB = 1,
        Twitter = 2,
        Google =3,
        Yahoo = 4,
        Weibo = 5,
        QQ = 6,
        Baidu = 7,
        Line = 8,
        Pinterest = 9,
        Bloger = 10,
        Plurk = 11,
        LinkedIn = 12,
        Renren = 13,
        YouTube = 14,
        Vimeo = 15,
        Instagram = 16,
        Tudou = 17,
        YK = 18,
        Pinkoi = 19,
        Accupass = 20,
        Blog = 21,
        Store = 22,
        Org = 23,
        Channel = 24,
        Album = 25,
        Links = 26,
        Other = 27,
        CopyUrl = 90,
        Collection = 91
    }
    public enum MemberVerifyType
    {
        None = 0,
        Register = 1,
        Modify = 2
    }
}