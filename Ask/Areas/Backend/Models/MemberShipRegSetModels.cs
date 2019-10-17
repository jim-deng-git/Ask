using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberShipRegSetModels
    {

        public long SiteID { get; set; }
        public bool IsOpenReg { get; set; }
        public MemberShipRegType RegType { get; set; }
        public MemberShipVerifyType VerifyType { get; set; }
        public string LoginType { get; set; }
        public List<MemberShipLoginType> LoginTypeList { get; set; }
        public bool IsNeedAgreeMemberDesc { get; set; }
        public bool IsNeedVerifyCode { get; set; }
        public bool IsAutoEmail { get; set; }
        public string AutoEmailManagers { get; set; }
        public List<MemberShipRegSocialSetModels> RegSocialSets { get; set; }
        public List<MemberShipRegColumnSetModels> RegColumnSets { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string AddFriend { get; set; }
        public bool BackendIsOpen { get; set; }
    }
    public enum MemberShipRegType
    {
        //社群登入+帳密註冊 
        All = 1,
        //僅以社群登入
        Social = 2,
        //僅以帳密註冊
        WebAccount = 3

    }
    public enum MemberShipVerifyType
    {
        //無須驗證
        None = 0,
        //Email 驗證
        Email = 1,
        //手機驗證
        Mobile = 2,
        //後台審核驗證
        AdminReview = 3
    }
    public enum MemberShipLoginType
    {
        //可自行輸入帳號登入
        InputAccount = 0,
        //可以Email登入
        Email = 1,
        //可以Email登入
        Mobile = 2,
        //可以身份證字號登入
        ID = 3
    }

    public enum MemberShipAddFriend
    {
        //APP 開放 QRcode掃描加友
        QRcode = 0,
        //開放線上邀請加友
        Invite = 1,
        //開放系統推薦加友
        System = 2
    }
}