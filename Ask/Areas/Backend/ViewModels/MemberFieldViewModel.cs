using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberFieldViewModel
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 欄位類型(名稱、Email...)
        /// </summary>
        public FieldCategory Category { get; set; }
        /// <summary>
        /// 欄位類型(單選,複選,文字輸入)
        /// </summary>
        public FieldType Type { get; set; }
        /// <summary>
        /// 統計資訊
        /// </summary>
        public List<MemberFieldStatisticViewModel> StatisticList { get; set; }
    }
    public enum FieldCategory
    {
        Name,
        Sex,
        Email,
        Birthday,
        Identity,
        Favority,
        MemberType,
        Address,
        Career,
        Company,
        Education,
        EmergencyEmail,
        EmergencyMobile,
        EmergencyName,
        EmergencyPhone,
        IDCard,
        Marriage,
        Mobile,
        OrderEpaper,
        Phone,
        Photo,
        Status,
        SerialNumber,
        PermanentAddress,
        PushRecommandCode,
        PushRecommandMobile,
        LegalPersonName,
        LegalPersonMobile,
        Point,
        ENName,
        Nickname,
        BloodType,
        Height,
        Weight,
        Vision,
        LineID,
        FacebookURL,
        ChildrenStatus,
        MilitaryService,
        SchoolAndDepartment,
        ClubExp,
        WorkExp,
        ReferrerName,
        AvailableTime,
        BankAccount,
        BankBook,
        IDcardPhoto,
        DailyPhoto
    }
    public enum FieldType
    {
        Text,
        Single,
        Check,
        Multiple
    }
}