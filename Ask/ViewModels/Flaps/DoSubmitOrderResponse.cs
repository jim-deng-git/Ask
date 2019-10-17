using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class DoSubmitOrderResponse
    {
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 店櫃業績唯一號
        /// </summary>
        public string PosNoSerNo { get; set; }
        /// <summary>
        /// 銷貨單號
        /// </summary>
        public string PisNo { get; set; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string InvoiceNo { get; set; }
    }
}