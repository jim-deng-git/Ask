using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class MobileVerifyRequest : APISiteRequestBase
    {
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string Captcha { get; set; }
    }
}