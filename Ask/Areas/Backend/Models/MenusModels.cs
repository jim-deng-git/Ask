using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using System.ComponentModel.DataAnnotations;
using WorkV3.Areas.Backend.Abstracts;
namespace WorkV3.Areas.Backend.Models
{
    public enum MenusAreaIDType
    {
        All = 0,
        TopNav = 1,
        MainNav = 2,
        FootNav = 3
    }
    public class MenusModels : Menu
    {
        public MenusModels()
        {
            GetChildren();
            Type = 2;
        }

        public byte AreaID { get; set; }
        public string DataType { get; set; }
        public string DataTypeValue { get; set; }
        public byte ShowStatus { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 後臺是否可以刪除Menu選單
        /// </summary>
        public bool DisableDelete { get; set; }

        #region Bind:CardsType       
        public string MenuCode { get; set; }
        public string MenuIcon { get; set; }
        public string MenuURLAction { get; set; }
        public int MenuiFrameH { get; set; }
        public int MenuiFrameW { get; set; }
        public string MenuEditURLAction { get; set; }
        public bool MenuIsNeedSN { get; set; }
        //public string MenuManageURL { get; set; }
        public List<MenuManageURLs> MenuManageURL { get; set; }
        public class MenuManageURLs
        {
            public string icon { get; set; }
            public string Title { get; set; }
            public string URLAction { get; set; }
        }
        #endregion

        #region Get 
        public string GetShowStatusClass()
        {
            string RT = DataAccess.MenusDAO.GetShowStatusClass(ShowStatus);
            return RT;

        }

        public override IEnumerable<Menu> GetChildren()
        {
            if (ID == 0)
                return new List<Menu>();

            Children = DataAccess.MenusDAO.GetChildren(ID);
            return Children;
        }

        public override Menu GetParent()
        {
            try
            {
                return DataAccess.MenusDAO.GetInfo(ID);
            }
            catch(Exception)
            {
                return null;
            }
        }
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