using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 共用的欄位設定 view model
    /// </summary>
    public class ColumnSet
    {
        /// <summary>
        /// 欄位辨視的 key 值
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 欄位標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 欄位類型
        /// 0 InputString, 1 InputInt, 2 InputEmail, 3 InputPassword, 4 Radiobox, 5 Checkbox, 6 DropdownList, 7 InputStringAddCheckBox, 8 DropdownListAddCheckBox, 9 DropdownListAddInputString, 10 DropdownListBankInputString
        /// </summary>
        public object DataType { get; set; }
        /// <summary>
        /// 是否不可空白
        /// </summary>
        public bool IsValidate { get; set; }
        /// <summary>
        /// 排序值
        /// </summary>
        public int SortIndex { get; set; }
    }
    public enum ColumnType
    {
        InputString,
        InputInt,
        InputEmail,
        InputPassword,
        Radiobox,
        Checkbox,
        DropdownList,
        InputStringAddCheckBox,
        DropdownListAddCheckBox,
        DropdownListAddInputString,
        DropdownListBankInputString,
        Address,
        PermanentAddress,
        BankAccount,
        DatePicker,
        HeaderImg,
        BankBook,
        IDCardFront,
        IDCardBack,
        LifeImage
    }
}