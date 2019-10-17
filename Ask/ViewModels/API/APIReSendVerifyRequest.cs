using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class APIReSendVerifyRequest : APISiteRequestBase
    {
        /// <summary>
        /// 會員
        /// </summary>
        public long MemberID { get; set; }
    }
}