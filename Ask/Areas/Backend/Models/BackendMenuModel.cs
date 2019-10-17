using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Areas.Backend.Abstracts;

namespace WorkV3.Areas.Backend.Models
{
    public class BackendMenuModel : Menu
    {
        public BackendMenuModel()
        {
            Type = 1;
            if (ID != 0)
                Children = GetChildren();
        }

        public string MenuSN { get; set; }
        /// <summary>
        /// 後台選單分類：1: 網頁(前台選單編輯), 2: 管理(前台選單內容編輯), 3: 後台選單, 9: 系統管理
        /// </summary>
        public int MenuClass { get; set; }
        public string Icon { get; set; }
        public int UrlType { get; set; }
        public string UrlActionName { get; set; }
        public string UrlControllerName { get; set; }
        public string UrlRouteName { get; set; }
        public string Url { get; set; }
        public string QueryString { get; set; }

        public override IEnumerable<Menu> GetChildren()
        {
            Children = BackendMenuDAO.GetChildren(ID, MenuClass);
            return Children;
        }

        public override Menu GetParent()
        {
            try
            {
                return BackendMenuDAO.GetItem((long)ParentID);
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}