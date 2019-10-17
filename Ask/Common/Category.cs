using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Common
{
    public enum FieldDefaultType {
        Email = 1,
        電話,
        姓名,
        性別,
        身份證字號,
        手機
    }

    public enum FieldWidth {
        [Description("25%")]
        Quarter = 1,
        [Description("50%")]
        Half = 2,
        [Description("100%")]
        Full = 3
    }

    public enum FieldHeight {
        三行 = 3,
        五行 = 5,
        十行 = 10
    }

    public enum FieldInputFormat {
        [Description("必須為數字")]
        數字 = 1,
        [Description("必須為英文")]
        英文,
        [Description("必須為英文或數字")]
        英文或數字,
        [Description("必須為網址")]
        網址,
        [Description("必須為Email")]
        Email,
        [Description("必須為台灣身份證字號")]
        台灣身份證,
        [Description("必須為中國身份證字號")]
        中國身份證,
        [Description("必須為美國SSN")]
        美國SSN,
        [Description("必須為台灣手機號碼")]
        台灣手機號,
        [Description("必須為中國手機號碼")]
        中國手機號,
        [Description("必須為台灣電話號碼")]
        台灣電話號,
        [Description("必須為台灣統一編號")]
        台灣統一編號
    }

    public enum FieldDateFormat {
        [Description("必須為西元日期")]
        西元日期 = 1,
        [Description("必須為民國日期")]
        民國日期,
        [Description("必須為時間")]
        時間,
        [Description("必須為西元日期+時間")]
        西元日期時間,
        [Description("必須為民國日期+時間")]
        民國日期時間
    }

    public enum FieldFileFormat {
        [Description("限制為文件 (doc、docx、xls、xlsx、ppt、pptx、pdf)")]
        文件 = 1,
        [Description("限制為PDF檔 (pdf)")]
        PDF,
        [Description("限制為壓縮檔 (zip、rar、7z)")]
        壓縮檔,
        [Description("限制為圖檔 (jpg、png、gif)")]
        圖檔,
        [Description("限制為影片 (mp4)")]
        影片,
        [Description("限制為聲音 (mp3)")]
        聲音
    }

    public enum FieldSizeFormat {
        [Description("限制檔案 10mb 以下")]
        小於10mb = 1,
        [Description("限制檔案 5mb 以下")]
        小於5mb,
        [Description("限制檔案 2mb 以下")]
        小於2mb,
        [Description("限制檔案 1mb 以下")]
        小於1mb,
        [Description("限制檔案 500kb 以下")]
        小於500kb,
        [Description("限制檔案 200kb 以下")]
        小於200kb,
        [Description("限制檔案 100kb 以下")]
        小於100kb
    }

    public enum FieldAddressRange {
        [Description("限制國內")]
        國內 = 1,
        [Description("限制省 / 州")]
        省,
        [Description("限制縣 / 市")]
        縣
    }

    public enum FieldRepeatLimit {
        不可重複,
        可重複數次,
        可任意報名
    }

    public enum FormCheckStatus { // 對於自定義表單，僅使用待審核和正取，對應狀態待處理和尚未處理
        待審核,
        正取,
        備取,
        保留,
        不通過,
        前台取消,
        後台取消
    }

    public enum LoginStatus {
        成功,
        用戶名不存在,
        密碼錯誤,
        用戶已禁用,
        EMail未驗證,
        手機未驗證
    }
    /// <summary>
    /// 電子報區塊類型: Head 表頭, Content 內文區, Footer 表尾
    /// </summary>
    public enum EpaperAreaType
    {
        Head = 0,
        Footer = 9,
        Content = 1
    }
    /// <summary>
    /// 電子報關聯模組類型: Article 文章, Event 活動, Questionnaire 問卷
    /// </summary>
    public enum EpaperModuleType
    {
        Article,
        Event,
        Questionnaire
    }
    /// <summary>
    /// 電子報匯入一航訊息: CreateTimeDesc 新創建的幾筆, SortAsc 後台排序在前的幾筆, Random 隨機挑選刊登中的幾筆
    /// </summary>
    public enum EpaperImportType
    {
        CreateTimeDesc,
        SortAsc,
        Random
    }
    /// <summary>
    /// 電子報圖文樣式: 圖片在上, 圖片在下, 圖片在標題下方, 文繞圖
    /// </summary>
    public enum EpaperImgStyle
    {
        Top = 0,
        UnderTitle = 1,
        TextAround = 2,
        Bottom = 9
    }
    /// <summary>
    /// 電子報圖片位置: 圖片置右, 圖片置左, 圖片置中
    /// </summary>
    public enum EpaperImgPosition
    {
        Left = 0,
        Center = 1,
        Right = 2
    }
    /// <summary>
    /// 日期選擇器選項
    /// </summary>
    public enum DateRange
    {
        /// <summary>
        /// 兩週
        /// </summary>
        DoubleWeeks = 1,
        /// <summary>
        /// 一個月
        /// </summary>
        Month = 2,
        /// <summary>
        /// 三個月
        /// </summary>
        ThreeMonths = 3,
        /// <summary>
        /// 半年
        /// </summary>
        HalfYear = 4,
        /// <summary>
        /// 年
        /// </summary>
        Year = 5,
        /// <summary>
        /// 自訂
        /// </summary>
        Custom = 6
    }

    #region 商城

    public enum EshopReceipt
    {
        /// <summary>
        /// 電子、紙本皆提供
        /// </summary>
        [Description("個人發票")]
        Personal = 0,
        /// <summary>
        /// 僅提供電子發票
        /// </summary>
        [Description("捐贈發票")]
        Donation = 1,
        /// <summary>
        /// 僅提供紙本發票
        /// </summary>
        [Description("公司發票")]
        Company = 2
    }
    public enum EshopPersonalReceiptType
    {
        /// <summary>
        /// 電子、紙本皆提供
        /// </summary>
        [Description("電子、紙本皆提供")]
        All = 0,
        /// <summary>
        /// 僅提供電子發票
        /// </summary>
        [Description("僅提供電子發票")]
        EReceipt = 1,
        /// <summary>
        /// 僅提供紙本發票
        /// </summary>
        [Description("僅提供紙本發票")]
        PaperReceipt = 2
    }
    public enum EshopCompanyReceiptType
    {
        /// <summary>
        /// 二、三聯式皆提供
        /// </summary>
        [Description("二、三聯式皆提供")]
        All = 0,
        /// <summary>
        /// 僅提供電子發票
        /// </summary>
        [Description("僅提供二聯式發票")]
        TwoCopies = 1,
        /// <summary>
        /// 僅提供紙本發票
        /// </summary>
        [Description("僅提供三聯式發票")]
        ThreeCopies = 2
    }
    #endregion
}