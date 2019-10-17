using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class OtherTransactionResponseParams
    {
        /// <summary>
        /// 交易結果回應碼
        /// </summary>
        public string ret_code { get; set; }

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string ret_msg { get; set; }

        /// <summary>
        /// 授權碼
        /// </summary>
        public string auth_id_resp { get; set; }

        /// <summary>
        /// 調單號碼
        /// </summary>
        public string rrn { get; set; }

        /// <summary>
        /// 訂單狀態碼
        /// 02 已授權
        /// 03 已請款
        /// 04 請款已清算
        /// 06 已退貨
        /// 08 退貨已清算
        /// 12 訂單已取消
        /// ZP 訂單處理中
        /// ZF 授權失敗
        /// </summary>
        public string order_status { get; set; }

        /// <summary>
        /// 授權方式
        /// SSL:SSL 授權
        /// 3D:3D 驗證
        /// </summary>
        public string auth_type { get; set; }

        /// <summary>
        /// 幣別
        /// </summary>
        public string cur { get; set; }

        /// <summary>
        /// 採購日期
        /// </summary>
        public string purchase_date { get; set; }

        /// <summary>
        /// 交易金額
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string tx_amt { get; set; }

        /// <summary>
        /// 請款金額
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string settle_amt { get; set; }

        /// <summary>
        /// 請款批號
        /// </summary>
        public string settle_seq { get; set; }

        /// <summary>
        /// 請款日期
        /// </summary>
        public string settle_date { get; set; }

        /// <summary>
        /// 退貨金額
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string refund_trans_amt { get; set; }

        /// <summary>
        /// 退貨調單編號
        /// </summary>
        public string refund_rrn { get; set; }

        /// <summary>
        /// 退貨授權碼
        /// </summary>
        public string refund_auth_id_resp { get; set; }

        /// <summary>
        /// 退貨日期
        /// </summary>
        public string refund_date { get; set; }

        /// <summary>
        /// 紅利訂單編號
        /// </summary>
        public string redeem_order_no { get; set; }

        /// <summary>
        /// 折抵點數
        /// </summary>
        public string redeem_pt { get; set; }

        /// <summary>
        /// 折抵金額
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string redeem_amt { get; set; }

        /// <summary>
        /// 實付金額
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string post_redeem_amt { get; set; }

        /// <summary>
        /// 剩餘點數
        /// </summary>
        public string post_redeem_pt { get; set; }

        /// <summary>
        /// 分期訂單號碼
        /// </summary>
        public string install_order_no { get; set; }

        /// <summary>
        /// 分期期數
        /// </summary>
        public string install_period { get; set; }

        /// <summary>
        /// 首期金額
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string install_down_pay { get; set; }

        /// <summary>
        /// 每期金額
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string install_pay { get; set; }

        /// <summary>
        /// 首期手續費
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string install_down_pay_fee { get; set; }

        /// <summary>
        /// 每期手續費
        /// 包含兩位小數，如 100 代表 1.00 元。
        /// </summary>
        public string install_pay_fee { get; set; }
    }
}