using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.TSCashFlow
{
    /// <summary>
    /// 台新金流驗證回傳資料格式
    /// </summary>
    public class GetAuthRequestParams
    {
        /// <summary>
        /// 客戶端版面類型 1:一般網頁(default) , 2:行動裝置網頁
        /// </summary>
        public string layout { get; set; } = "1";
        /// <summary>
        /// 訂單號碼 
        /// </summary>
        public string order_no { get; set; } = "";
        /// <summary>
        /// 交易金額 包含兩位小數，如 100 代表 1.00 元。  
        /// </summary>
        public string amt { get; set; } = "";
        /// <summary>
        /// 幣別 NTD: 新台幣 (default)
        /// </summary>
        public string cur { get; set; } = "NTD";
        /// <summary>
        /// 訂單說明 此欄允許中文，請以 UTF-8 編碼傳入。 
        /// </summary>
        public string order_desc { get; set; } = "";
        /// <summary>
        /// 授權同步請款標記 0:不同步請款(default) , 1:同步請款, (若使用「TSPG 系統自動請款」作業方式，請設定為0)
        /// </summary>
        public string capt_flag { get; set; } = "0";
        /// <summary>
        /// 回傳訊息標記 0:不查詢交易詳情(default) , 1:查詢交易詳情, 若為 1，則 TSPG 會在傳送交易資料至「指定交易資料回傳網址(result_url)」時，一併傳送本交易之詳細資料。 
        /// </summary>
        public string result_flag { get; set; } = "0";
        /// <summary>
        /// 指定接續網址 
        /// </summary>
        public string post_back_url { get; set; } = "";
        /// <summary>
        /// 指定交易資料回傳網址，須為 https://  
        /// </summary>
        public string result_url { get; set; } = "";
    }
}