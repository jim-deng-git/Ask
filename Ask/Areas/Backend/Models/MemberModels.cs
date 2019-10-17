using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberModels
    {

        public long Id { get; set; }
        public string MemName { get; set; }
        public string LoginID { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string RegSource { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public int MStatus { get; set; }
        public bool isSysOnly { get; set; }
        public bool IsSupremeAuthority { get; set; }
        public bool IsChangedPassword { get; set; }
        public HttpPostedFileBase imgFile { get; set; }
        public int Sort { get; set; }

        #region 登入狀態
        public enum LoginStatus
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success = 1,
            /// <summary>
            /// 登入錯誤
            /// </summary>
            LoginError,
            /// <summary>
            /// 密碼錯誤
            /// </summary>
            PasswordError,
            /// <summary>
            /// 無權限
            /// </summary>
            NoPermission,
            /// <summary>
            /// 帳號停權
            /// </summary>
            suspension


        }
        #endregion

        #region 社群登入網站
        public enum SocialLoginSite
        {
            Facebook,
            Google,
            Twitter,
            LinkedIn
        }
        #endregion

        #region 薪資
        /// <summary>
        /// 到職日
        /// </summary>
        public DateTime? ArriveDate { get; set; }
        /// <summary>
        /// 個人備註
        /// </summary>
        public string PersonalNote { get; set; }
        /// <summary>
        /// 本薪
        /// </summary>
        public int Salary { get; set; }
        /// <summary>
        /// 薪資發放
        /// </summary>
        public int SalaryPaymentType { get; set; }
        /// <summary>
        /// 勞健保津貼
        /// </summary>
        public int LaborAllowance { get; set; }
        /// <summary>
        /// 勞退
        /// </summary>
        public int LaborPension { get; set; }
        /// <summary>
        /// 職務加給
        /// </summary>
        public int JobAdditionPay { get; set; }
        /// <summary>
        /// 執行費
        /// </summary>
        public int ExecutionFee { get; set; }
        /// <summary>
        /// 勞保扣繳
        /// </summary>
        public int LaborInsurance { get; set; }
        /// <summary>
        /// 健保扣繳
        /// </summary>
        public int HealthInsurance { get; set; }
        /// <summary>
        /// 福利金
        /// </summary>
        public int Welfare { get; set; }
        /// <summary>
        /// 請假費
        /// </summary>
        public int LeaveFee { get; set; }
        #endregion
    }

    public class MemberSearch
    {
        public string Keyword { get; set; }
        public int[] MStatus { get; set; }
        public long[] Groups { get; set; }
    }

    public enum SalaryPaymentType
    {
        [Description("銀行")]
        Bank = 1,
        [Description("現金")]
        Cash
    }
}