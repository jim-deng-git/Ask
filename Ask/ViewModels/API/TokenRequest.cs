using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 要傳入的 Model
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// 加密後的 Secret key (由公司提供未加密的 key, 再由 app 進行 3DES 加密)
        /// </summary>
        public string Secret { get; set; } = "";
    }
}