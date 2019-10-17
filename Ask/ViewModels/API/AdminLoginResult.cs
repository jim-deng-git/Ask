using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class AdminLoginResult : GetResult
    {
        /// <summary>
        /// 會員基本資料
        /// </summary>
        public MemberInfo MemberInfo { get; set; }
        /// <summary>
        /// 取得的 token 值, 若認證失敗為空白, token 有效期為 10 分鐘
        /// </summary>
        public string AdminToken { get; set; }
        /// <summary>
        /// MemberToken到期時間
        /// </summary>
        public string Expire { get; set; }
    }
}