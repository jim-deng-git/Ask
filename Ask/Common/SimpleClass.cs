using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Common
{    
    public class Pager
    {
        public int TotalRecord { get; set; }
        public int TotalPage { get; set; }
        public int PageIndex { get; set; }
        public string UrlFmt { get; set; }
        public string BlankMessage { get; set; }
        public List<int> PageNumbers { get; set; }
    }

    public class Pagination
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalRecord { get; set; }

        public int GetItemIndex(int index) {
            return (PageIndex - 1) * PageSize + index + 1;
        }
    }

    public class SitePage
    {
        public long SiteID { get; set; }
        public string SiteSN { get; set; }
        public string SiteName { get; set; }
        public long PageNo { get; set; }
        public string PageSN { get; set; }
        public long MenuID { get; set; }
        public string MenuSN { get; set; }
        public string MenuName { get; set; }

        public string GetUploadUrl() {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebUpdUrl"].TrimEnd('/') + "/" + SiteSN + "/" + MenuSN + "/";
        }
    }
    
    public class IDValue
    {
        public long ID { get; set; }
        public string Value { get; set; }
        public long SiteID { get; set; }
    }

    public class SortItem {
        public long ID { get; set; }
        public int Index { get; set; }
    }

    public class BackSiteListButton
    {
        /// <summary>
        /// 常用複製按鈕
        /// </summary>
        /// <returns></returns>
        public static BackSiteListButton CommonListTableCopyButton()
        {
            var copyButton = new BackSiteListButton();
            copyButton.ActionName = "copy";
            copyButton.Classes = "btn-grey-o square tooltip-info";
            copyButton.DataSelect = true;
            copyButton.Title = "複製";
            copyButton.IconClasses = "cc cc-content-copy";
            return copyButton;
        }
        /// <summary>
        /// 常用排序按鈕
        /// </summary>
        /// <returns></returns>
        public static BackSiteListButton CommonListTableSortButton()
        {
            var sortButton = new BackSiteListButton();
            sortButton.ActionName = "sort";
            sortButton.Classes = "btn-grey-o square tooltip-info";
            sortButton.DataSelect = true;
            sortButton.Title = "排序";
            sortButton.IconClasses = "cc cc-sort";
            return sortButton;
        }
        /// <summary>
        /// 常用新增按鈕
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        public static BackSiteListButton CommonListTableLinkButton(string href = "")
        {
            var addButton = new BackSiteListButton();
            addButton.ActionName = "add";
            addButton.Classes = "btn-grey-darken-4-o square tooltip-info openEdit-c cboxElement tooltipstered";
            addButton.Title = "新增";
            addButton.Href = href;
            addButton.IconClasses = "cc cc-plus";
            addButton.ShowDataAction = false;
            return addButton;
        }
        /// <summary>
        /// 常用刪除按鈕
        /// </summary>
        /// <returns></returns>
        public static BackSiteListButton CommonListTableDelButton()
        {
            var delButton = new BackSiteListButton();
            delButton.ActionName = "del";
            delButton.Classes = "btn-grey-o square tooltip-info btn-del";
            delButton.DataSelect = true;
            delButton.Title = "刪除";
            delButton.IconClasses = "cc cc-trash-o";
            return delButton;
        }
        /// <summary>
        /// 常用搜尋按鈕
        /// </summary>
        /// <returns></returns>
        public static BackSiteListButton CommonListTableSearchButton()
        {
            var searchButton = new BackSiteListButton();
            searchButton.ActionName = "";
            searchButton.Classes = "btn-grey-o square tooltip-info tooltipstered";
            searchButton.Title = "篩選";
            searchButton.IconClasses = "cc cc-search";
            searchButton.ShowDataAction = false;
            searchButton.IdAttribute = "openSearch";
            return searchButton;
        }
        /// <summary>
        ///常用下拉按鈕
        /// </summary>
        /// <param name="dropDownListId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static BackSiteListButton CommonDropDownListButton(string dropDownListId, string title)
        {
            var dropDownListButton = new BackSiteListButton();
            dropDownListButton.ActionName = "";
            dropDownListButton.Classes = "btn-grey-o square tooltip-info dropdown-btn tooltipstered";
            dropDownListButton.Title = title;
            dropDownListButton.IconClasses = "cc cc-playlist-play";
            dropDownListButton.ShowDataAction = false;

            dropDownListButton.DropDownList = new BackSiteListButtonDropDownList(dropDownListId);
            return dropDownListButton;
        }

        private string _href = "";
        /// <summary>
        /// 按鈕顯示文字
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 按鈕 icon
        /// </summary>
        public string IconClasses { get; set; }
        /// <summary>
        /// 按鈕 class
        /// </summary>
        public string Classes { get; set; }
        /// <summary>
        /// 按鈕 id
        /// </summary>
        public string IdAttribute { get; set; }
        /// <summary>
        /// 點擊後是否可選擇資料
        /// </summary>
        public bool DataSelect { get; set; } = false;
        /// <summary>
        /// 是否顯示 data-action
        /// </summary>
        public bool ShowDataAction { get; set; } = true;
        /// <summary>
        /// Component datalist 中使用的 action
        /// </summary>
        public string ActionName { get; set; }
        public string Target { get; set; }
        /// <summary>
        /// 下拉式選單
        /// </summary>
        public BackSiteListButtonDropDownList DropDownList { get; set; }
        /// <summary>
        /// 按鈕連結
        /// </summary>
        public string Href
        {
            get
            {
                return string.IsNullOrEmpty(_href) ? "javascript:;" : _href;
            }
            set
            {
                _href = value;
            }
        }

        public BackSiteListButton()
        {

        }

        public BackSiteListButton(string title, string href, string classes, string icon, string target = "")
        {
            Classes = classes;
            Title = title;
            IconClasses = icon;
            Href = href;
            Target = target;
            ShowDataAction = false;
        }

        public bool AddDropDownListButton(BackSiteListButton button)
        {
            try
            {
                DropDownList.Links.Add(button);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }

    public class BackSiteListButtonDropDownList
    {
        public string IdAttribute { get; set; }
        public List<BackSiteListButton> Links { get; set; } = new List<BackSiteListButton>();

        public BackSiteListButtonDropDownList(string id)
        {
            IdAttribute = id;
        }
    }
}