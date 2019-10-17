using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public abstract class TSRequest
    {
        /// <summary>
        /// 傳送端程式類型 , 固定值：rest 
        /// </summary>
        public string sender { get; set; } = "rest";
        /// <summary>
        /// 格式版本號 固定值：1.0.0 
        /// </summary>
        public string ver { get; set; } = "1.0.0";
        /// <summary>
        /// 特店代號 (請依台新提供的測試/正式特店代號更新) [CCWork => 999812666555086]
        /// </summary>
        public string mid { get; set; } = WorkLib.GetItem.appSet("TaishinStoreKey").ToString();
        ///// <summary>
        ///// 次特店代號 (非代收代付及大特店請勿傳入此參數) 
        ///// </summary>
        //public string s_mid { get; set; } = ""
        /// <summary>
        /// 端末代號 (請依台新提供的測試/正式端末機代號更新) [CCWork => T0000000]
        /// </summary>
        public string tid { get; set; } = WorkLib.GetItem.appSet("TaishinStoreNo").ToString();
        /// <summary>
        /// 付款類別： 1:信用卡(default) 2:銀聯卡 UnionPay
        /// </summary>
        public int pay_type { get; set; } = 1;
        /// <summary>
        /// 交易類型： 1:授權(default) 3:請款 4:取消請款 5:退貨 6:取消退貨（銀聯卡UnionPay無此功能）,  7:查詢 8:取消授權Y
        /// </summary>
        public int tx_type { get; set; } = 1;
    }

    public enum TaiShinRequestType
    {
        Auth = 1,           // 授權
        Settle = 3,         // 請款
        CancelSettle = 4,   // 取消請款
        Refund = 5,         // 對請款已清算的訂單退回貨款
        CancelRefund = 6,   // 取消退款
        Query = 7,          // 查詢訂單
        Cancel = 8,         // 對已授權但尚未請款的訂單取消授權
    }
}