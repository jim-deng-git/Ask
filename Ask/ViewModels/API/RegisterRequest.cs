using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RegisterRequest
    {
        /// <summary>
        /// 網站代碼(EX: career)
        /// </summary>
        public string SiteSN { get; set; }
        /// <summary>
        /// 註冊資料(照原格式丟回)
        /// 
        /// 欄位Address 輸入範例 Values = "string" (textbox中的值)  SelectedList = "[1,4,6]" (被選擇的區域ID 照level排列 用[]包起來 逗號分隔)
        /// </summary>
        public List<RegisterData> RegisterData { get; set; }
    }

    /// <summary>
    /// 檔案欄位只輸入檔名(網址需拿掉)
    /// </summary>
    public class RegisterData
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string DataType { get; set; }
        public bool IsValidate { get; set; }
        public int SortIndex { get; set; }
        public object Value { get; set; }
        public object Selected { get; set; }
        public bool isChecked { get; set; }
        public string CheckBoxValue { get; set; }

        public object SelectedList { get; set; }
    }
}