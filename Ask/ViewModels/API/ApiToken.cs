using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    ///  token model
    /// </summary>
    public class ApiToken
    {
        /// <summary>
        /// 開始時間
        /// </summary>
        public string issued { get; set; }
        /// <summary>
        /// 有效時間
        /// </summary>
        public string expire { get; set; }
        /// <summary>
        /// 會員ID
        /// </summary>
        public long? memberId { get; set; }
        public long? siteId { get; set; }
        public bool IsAdmin { get; set; } = false;

        public static ApiToken GetJwtAuthObject()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["Authorization"].ToString()))
            {
                var authHeader = HttpContext.Current.Request.Headers["Authorization"];
                var authBits = authHeader.Replace("Bearer ", "").Replace(" ", "");
                var secret = WorkLib.GetItem.appSet("APISecret").ToString();
                var result = Jose.JWT.Decode<ApiToken>(authBits, System.Text.Encoding.UTF8.GetBytes(secret), Jose.JwsAlgorithm.HS256);
                return result;
            }
            return null;
        }
    }
}