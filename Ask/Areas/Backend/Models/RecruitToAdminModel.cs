using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class RecruitToAdminModel
    {
        public long ID { get; set; }
        public long RecruitID { get; set; }
        public string Email { get; set; }
        public long? MemberID { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public int AdminType { get; set; }
        public string MemberIDstr { get; set; }
    }

    /// <summary>
    /// 徵才管理員種類
    /// </summary>
    public enum RecruitAdminType
    {
        /// <summary>
        /// 徵才管理員
        /// </summary>
        RecruitAdmin = 1,
        /// <summary>
        /// 徵才聊天室管理員(客服/單人)
        /// </summary>
        ChatAdminSingle,
        /// <summary>
        /// 徵才聊天室管理員(群組)
        /// </summary>
        ChatAdminGroup
    }
}