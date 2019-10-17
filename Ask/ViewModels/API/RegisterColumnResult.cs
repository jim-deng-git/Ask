using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 註冊欄位
    /// </summary>
    public class TextBoxColumnResult : ColumnSet
    {
        public object Value { get; set; }
    }

    public class MenuColumnResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public object DataType { get; set; }
        public bool IsValidate { get; set; }
        public int SortIndex { get; set; }
        public object Selected { get; set; }
        public List<Menu> MenuContent { get; set; }
    }

    public class Menu
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    //特殊欄位
    public class TextBoxAddCheckBox : TextBoxColumnResult
    {
        public bool isChecked { get; set; }
        public string CheckBoxValue { get; set; }
    }

    public class MenuAddCheckBox : MenuColumnResult
    {
        public bool isChecked { get; set; }
        public string CheckBoxValue { get; set; }
    }

    public class MenuAddTextBox : ColumnSet
    {
        public object Value { get; set; }
        public object Selected { get; set; }
    }

    public class MenuAddTextBoxAndCheckBox : MenuAddTextBox
    {
        public bool isChecked { get; set; }
        public string CheckBoxValue { get; set; }
    }

    public class ImageBox : TextBoxColumnResult
    {
        public string ImageUrl { get; set; }
    }

    //地址專用
    public class AddressBox : ColumnSet
    {
        public object Value { get; set; }
        public object SelectedList { get; set; }
    }

    //通訊地址專用
    public class PermanentAddressBox : AddressBox
    {
        public bool isChecked { get; set; }
        public string CheckBoxValue { get; set; }
    }


    public enum RegisterColumnResultCode
    {
        [Message("")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("Menu尚未建立")]
        MenuNull,
        [Message("需要登入")]
        NeedLogin,
        Exception
    }
}