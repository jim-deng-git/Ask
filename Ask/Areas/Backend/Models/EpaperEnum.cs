using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 電子報編輯段落內容的圖片垂直位置
    /// </summary>
    public enum ParaMatchPos
    {
        /// <summary>
        /// 圖片在上
        /// </summary>
        top,
        /// <summary>
        /// 圖片在下
        /// </summary>
        bottom,
        /// <summary>
        /// 圖片在標題下方
        /// </summary>
        center,
        /// <summary>
        /// 文繞圖
        /// </summary>
        float_left
    }
    /// <summary>
    /// 電子報編輯段落內容的圖片水平位置
    /// </summary>
    public enum ParaMatchAlign
    {
        /// <summary>
        /// 圖片置左
        /// </summary>
        left,
        /// <summary>
        /// 圖片置右
        /// </summary>
        right,
        /// <summary>
        /// 圖片置中
        /// </summary>
        center
    }
    /// <summary>
    /// 黑名單原因
    /// </summary>
    public enum BlackReason
    {
        /// <summary>
        /// 失敗過多
        /// </summary>
        SendTooManyFail,
        /// <summary>
        /// 被投訴
        /// </summary>
        Complaint,
        /// <summary>
        /// 無使用價值
        /// </summary>
        NoUseValue,
        /// <summary>
        /// 其它
        /// </summary>
        Other
    }
    /// <summary>
    /// 電子報樣式
    /// </summary>
    public enum EpaperType
    {
        /// <summary>
        /// 一般
        /// </summary>
        Normal,
        /// <summary>
        /// 段落
        /// </summary>
        Paragraph,
        /// <summary>
        /// 專家
        /// </summary>
        Pro,
        /// <summary>
        /// 自訂
        /// </summary>
        Custom
    }
    /// <summary>
    /// 發送類型
    /// </summary>
    public enum SendType
    {
        /// <summary>
        /// 篩選對象
        /// </summary>
        Filter,
        /// <summary>
        /// 匯入名單
        /// </summary>
        Import,
        /// <summary>
        /// 手動輸入
        /// </summary>
        Manual
    }
    /// <summary>
    /// 報別開放對象 / 訂閱者類型
    /// </summary>
    public enum OpenObject
    {
        /// <summary>
        /// 一般訂閱者
        /// </summary>
        General,
        /// <summary>
        /// 會員
        /// </summary>
        Member,
        /// <summary>
        /// 全部
        /// </summary>
        All
    }
    /// <summary>
    /// 篩選參數
    /// </summary>
    public enum FilterPara
    {
        /// <summary>
        /// 一般訂閱者
        /// </summary>
        [Description("一般訂閱者")]
        AllUsers,
        /// <summary>
        /// 全部會員
        /// </summary>
        [Description("全部會員")]
        AllMember,
        /// <summary>
        /// 所有喜好
        /// </summary>
        [Description("所有喜好")]
        AllLikes,
        /// <summary>
        /// 所有訂閱
        /// </summary>
        [Description("所有訂閱")]
        AllOrders
    }
    public enum ExcelFormat
    {
        xls,
        xlsx
    }
    /// <summary>
    /// 發送狀況
    /// </summary>
    public enum SendResult
    {
        /// <summary>
        /// 尚未發送
        /// </summary>
        Notyet,
        /// <summary>
        /// 發送成功
        /// </summary>
        Successful,
        /// <summary>
        /// 發送失敗
        /// </summary>
        Fail,
        /// <summary>
        /// 已讀
        /// </summary>
        Readed
    }
    /// <summary>
    /// 電子報分析 報表輸出種類
    /// </summary>
    public enum EpaperAnalysisReport
    {
        /// <summary>
        /// 日報表
        /// </summary>
        [Description("日報表")]
        day,
        /// <summary>
        /// 月報表
        /// </summary>
        [Description("月報表")]
        month,
        /// <summary>
        /// 週報表
        /// </summary>
        [Description("週報表")]
        week
    }
    /// <summary>
    /// 發送狀態
    /// </summary>
    public class SendStatus
    {
        /// <summary>
        /// 開始設定
        /// </summary>
        [Display(Name = "尚未設定")]
        public const string Start = "Start";
        /// <summary>
        /// 設定中
        /// </summary>
        [Display(Name = "設定中")]
        public const string Setting = "Setting";
        /// <summary>
        /// 發送中
        /// </summary>
        [Display(Name = "發送中")]
        public const string Sending = "Sending";
        /// <summary>
        /// 發送完畢
        /// </summary>
        [Display(Name = "發送完畢")]
        public const string Sended = "Sended";
        /// <summary>
        /// 暫停
        /// </summary>
        [Display(Name = "暫停")]
        public const string Pause = "Pause";
        /// <summary>
        /// 系統終止
        /// </summary>
        [Display(Name = "系統終止")]
        public const string SystemBreak = "SystemBreak";
        /// <summary>
        /// 手動終止
        /// </summary>
        [Display(Name = "手動終止")]
        public const string ManualBreak = "ManualBreak";
        /// <summary>
        /// 暫存
        /// </summary>
        [Display(Name = "暫存")]
        public const string Temp = "Temp";
    }
}