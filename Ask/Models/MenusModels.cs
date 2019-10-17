using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using System.ComponentModel.DataAnnotations;
namespace WorkV3.Models
{
    public enum MenusAreaIDType
    {
        All = 0,
        TopNav = 1,
        MainNav = 2,
        FootNav = 3
    }
    public class MenusModels
    {

        public long Id { get; set; }
        public long SiteID { get; set; }
        public string SiteSN { get; set; }
        public long ParentID { get; set; }
        public byte AreaID { get; set; }
        public string Title { get; set; }
        public string SN { get; set; }
        public string DataType { get; set; }
        public string DataTypeValue { get; set; }
        public byte ShowStatus { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        #region Bind:[CardsType]
        public string MenuCode { get; set; }

        #endregion

        #region Bind:[ResourceLinks]
        public string LinkInfo { get; set; }
        public int RLClickType { get; set; }
        public bool? IsOpenNew { get; set; }
        #endregion

        #region Bind:[ResourceFiles]
        public int RFClickType { get; set; }
        public string FileInfo { get; set; }

        #endregion

   

        #region Bind:[Page]
        public string PageSN { get; set; }
        #endregion

    }

    /// <summary>
    /// Menus.DataType 選單型態
    /// </summary>
    public static class ClassMenuDataValue
    {
        /// <summary>
        /// 同下層第一個連結
        /// </summary>
        public static string Top = "Top";
        /// <summary>
        /// 無連結僅開啟下層
        /// </summary>
        public static string Open = "Open";
    }

    public enum MenuShowStatus
    {
        /// <summary>
        /// 開放
        /// </summary>
        Show = 1,
        /// <summary>
        /// 隱藏
        /// </summary>
        Hide = 2,
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 3


    }
    
    /// <summary>
    /// Menus.ClickType 選單型態
    /// </summary>
    public enum MenuClickType
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
}