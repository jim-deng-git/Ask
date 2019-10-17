using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class GetBankRequest
    {
        /// <summary>
        /// 銀行代碼(若代碼為null，撈所有銀行，否則撈該銀行所有分行)
        /// </summary>
        public string BankCode { get; set; }
    }
}