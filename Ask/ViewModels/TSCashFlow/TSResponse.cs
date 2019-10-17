using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public abstract class TSResponse
    {
        /// <summary>
        /// 格式版本號 固定值：1.0.0 
        /// </summary>
        public string ver { get; set; }
        /// <summary>
        /// 特店代號 (請依台新提供的測試/正式特店代號更新) [CCWork => 999812666555086]
        /// </summary>
        public string mid { get; set; }
        ///// <summary>
        ///// 次特店代號 (非代收代付及大特店請勿傳入此參數) 
        ///// </summary>
        //public string s_mid { get; set; }
        /// <summary>
        /// 端末代號 (請依台新提供的測試/正式端末機代號更新) [CCWork => T0000000]
        /// </summary>
        public string tid { get; set; }
        /// <summary>
        /// 付款類別： 1:信用卡(default) 2:銀聯卡 UnionPay
        /// </summary>
        public int pay_type { get; set; }
        /// <summary>
        /// 交易類型： 1:授權(default) 3:請款 4:取消請款 5:退貨 6:取消退貨（銀聯卡UnionPay無此功能）,  7:查詢 8:取消授權Y
        /// </summary>
        public int tx_type { get; set; }
        /// <summary>
        /// 交易類型： 1:授權(default) 3:請款 4:取消請款 5:退貨 6:取消退貨（銀聯卡UnionPay無此功能）,  7:查詢 8:取消授權Y
        /// </summary>
        public int ret_value { get; set; }
    }
}