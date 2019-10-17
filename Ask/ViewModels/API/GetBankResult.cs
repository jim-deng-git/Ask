using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 銀行代碼及名稱
    /// </summary>
    public class GetBankResult
    {
        /// <summary>
        /// 代碼
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
    }

    public enum GetBankResultCode
    {
        [Message("")]
        Success,
        [Message("查無此銀行")]
        NoBank,
        Exception
    }
}