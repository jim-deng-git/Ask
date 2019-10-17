using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WorkV3.Golbal
{
    //public class Category
    //{
    //}
    #region 語系
    public enum WEBLang {

        [Description("zh-Hant")]
        zhHant = 1,

        [Description("en")]
        en = 2,

        [Description("ja")]
        ja = 3


    }
    #endregion

    /// <summary>
    /// 卡片的類別
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 結構型選單
        /// </summary>
        ClassMenu = 0,
        /// <summary>
        /// 模組卡
        /// </summary>
        Card = 1,
        /// <summary>
        /// 集合卡
        /// </summary>
        CardSet = 2,
        /// <summary>
        /// 頁面元件
        /// </summary>
        Component = 3
    }

    /// <summary>
    /// ClickType 選單型態
    /// </summary>
    public enum ClickType
    {
        /// <summary>
        /// 沒有link
        /// </summary>
        NoLink = 0,
        /// <summary>
        /// 直接本頁開啟
        /// </summary>
        PageOpen = 1,
        /// <summary>
        /// 另開新視窗
        /// </summary>
        OpenNewWin = 2,
        /// <summary>
        /// lightbox嶔入(外部網站需允許權限才可使用)
        /// </summary>
        LighboxInclude = 3

    }

    //public enum LinkType
    //{
    //    /// <summary>
    //    /// 內部連結
    //    /// </summary>
    //    InLink = 1,
    //    /// <summary>
    //    /// 內部連結卡片
    //    /// </summary>
    //    InLinkCard = 2,
    //    /// <summary>
    //    /// 外部連結
    //    /// </summary>
    //    OutLink = 3
            
    //}

    /// <summary>
    /// Resource 建立來源
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// 選單
        /// </summary>
        Menu = 1,
        /// <summary>
        /// 卡片資料
        /// </summary>
        Card = 2
    }

    #region syslog 系統紀錄

    /// <summary>
    /// 管理功能代碼
    /// </summary>
    public enum SysMgrNo
    {
        /// <summary>
        /// 權限
        /// </summary>
        Authority = 1,
        /// <summary>
        /// 網站
        /// </summary>
        Site = 2,
        /// <summary>
        /// 選單
        /// </summary>
        Menu = 3,
        /// <summary>
        /// 頁面
        /// </summary>
        Page = 4,
        /// <summary>
        /// App
        /// </summary>
        App = 5
    }

    public enum SysMgrNoName
    {
        /// <summary>
        /// 權限
        /// </summary>
        權限 = 1,
        /// <summary>
        /// 網站
        /// </summary>
        網站 = 2,
        /// <summary>
        /// 選單
        /// </summary>
        選單 = 3
    }
    /// <summary>
    /// 動作代碼
    /// </summary>
    public enum SysActions
    {
        /// <summary>
        /// 登出
        /// </summary>
        Logout = 0,
        /// <summary>
        /// 登入
        /// </summary>
        Login = 1,

        /// <summary>
        /// 新增
        /// </summary>
        Add = 2,
        /// <summary>
        /// 修改
        /// </summary>
        Edit = 3,
        /// <summary>
        /// 刪除
        /// </summary>
        Del = 4,
        /// <summary>
        /// 查詢
        /// </summary>
        Search = 5,

        /// <summary>
        /// 排序
        /// </summary>
        Sort = 6,

        /// <summary>
        /// 審核
        /// </summary>
        Review = 10,
        /// <summary>
        /// 移動
        /// </summary>
        Move = 7
    }

    public enum SysActionsName
    {
        /// <summary>
        /// 登出
        /// </summary>
        登出 = 0,
        /// <summary>
        /// 登入
        /// </summary>
        登入 = 1,

        /// <summary>
        /// 新增
        /// </summary>
        新增 = 2,
        /// <summary>
        /// 修改
        /// </summary>
        修改 = 3,
        /// <summary>
        /// 刪除
        /// </summary>
        刪除 = 4,
        /// <summary>
        /// 查詢
        /// </summary>
        查詢 = 5,

        /// <summary>
        /// 排序
        /// </summary>
        排序 = 6,

        /// <summary>
        /// 審核
        /// </summary>
        審核 = 10
    }

    #endregion

    #region 後台權限

    /// <summary>
    /// GroupPermission 中控管的 menu 類型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 後台的 menu
        /// </summary>
        BackendMenu = 1, 
        /// <summary>
        /// 前台的 menu
        /// </summary>
        FrontendMenu = 2
    }

    /// <summary>
    /// BackendMenu 中存放的 menu 分類
    /// </summary>
    public enum MenuClass
    {
        /// <summary>
        /// 後台選單的網頁選項，主要為前台的選單創建
        /// </summary>
        網頁 = 1, 
        /// <summary>
        /// 後台選單的管理選項，主要為前台的選單功能管理
        /// </summary>
        管理 = 2,
        /// <summary>
        /// 除了網頁及管理選項的之外的選項
        /// </summary>
        一般 = 3
    }

    #endregion

    #region API

    public enum ApiType
    {
        Flaps = 1,
        TaishinCreditCard = 2
    }
    #endregion
}