using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class MemberModels
    {

        public long Id { get; set; }
        public string MemName { get; set; }
        public string NickName { get; set; }
        public string Img { get; set; }
        public string RegSource { get; set; }
        public string Email { get; set; }
        public bool isSysOnly { get; set; }


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
    }


}