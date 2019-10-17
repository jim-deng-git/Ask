using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace WorkV3.Common
{
    public class Utility
    {
        public static string GetRootUrl() {
            Uri url = HttpContext.Current.Request.Url;
            string rootUrl = url.GetLeftPart(UriPartial.Authority).TrimEnd('/');
            rootUrl += HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');

            return rootUrl;
        }
        
        public static string GetNewUniqueFileName(string fileName, bool reserveOriginal = false) {
            int index = fileName.LastIndexOf('/');
            if (index == -1)
                index = fileName.LastIndexOf('\\');

            string name = fileName.Substring(index + 1);
            index = name.LastIndexOf('.');

            string newName = string.Empty;
            if (reserveOriginal)
                newName = (index == -1 ? name : name.Substring(0, index)) + "_G";

            newName += Guid.NewGuid().ToString("N");
            if (index != -1)
                newName += name.Substring(index);

            return newName;
        }

        public static T GetSearchValue<T>() where T : class {
            HttpContext context = HttpContext.Current;
            string key = context.Request.Url.AbsolutePath.ToLower();
            return context.Session[key] as T;
        }

        public static void SetSearchValue<T>(T searchVal) where T : class {
            HttpContext context = HttpContext.Current;
            string key = context.Request.Url.AbsolutePath.ToLower();

            context.Session[key] = searchVal;
        }

        public static void ClearSearchValue() {
            HttpContext context = HttpContext.Current;
            string key = context.Request.Url.AbsolutePath.ToLower();

            context.Session.Remove(key);
        }

        public static string GetViewHtml(Controller controller, string viewName, string layoutName) {
            IView view = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, layoutName).View;
            if (view == null)
                return string.Empty;

            using (System.IO.StringWriter writer = new System.IO.StringWriter()) {
                ViewContext viewContext = new ViewContext(controller.ControllerContext, view, controller.ViewData, controller.TempData, writer);
                viewContext.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        #region DataTable 與 Excel 互操作
        /// <summary>
        /// Excel匯入成為Datable
        /// </summary>
        /// <param name="file">匯入的 Excel 檔案</param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelToTable(string file) {
            System.Data.DataTable dt = new System.Data.DataTable();
            IWorkbook workbook;
            string fileExt = System.IO.Path.GetExtension(file).ToLower();
            using (System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                //XSSFWorkbook 用於XLSX格式，HSSFWorkbook 用於XLS格式
                if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
                if (workbook == null) { return null; }
                ISheet sheet = workbook.GetSheetAt(0);

                //表頭 
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++) {
                    object obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty) {
                        dt.Columns.Add(new System.Data.DataColumn("Columns" + i.ToString()));
                    } else
                        dt.Columns.Add(new System.Data.DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据  
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) {
                    System.Data.DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns) {
                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty) {
                            hasValue = true;
                        }
                    }
                    if (hasValue) {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Datable 匯出為 Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file">匯出的 Excel 檔案名稱</param>
        public static void TableToExcel(System.Data.DataTable dt, string file) {
            IWorkbook workbook;
            string fileExt = System.IO.Path.GetExtension(file).ToLower();
            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }
            ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

            //表头  
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++) {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (int i = 0; i < dt.Rows.Count; i++) {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++) {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组  
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }


        /// <summary>
        /// 獲取單元格類型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueType(ICell cell) {
            if (cell == null)
                return null;
            switch (cell.CellType) {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion

        public static bool HavePermission(string menu, long? siteId) {
            Areas.Backend.Models.MemberModels curUser = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent;
            if (curUser == null)
                return false;

            return WorkV3.Golbal.PermissionCheck.havePermission(curUser.Id, menu, siteId);
        }

        // neil 20180508 新增權限判斷
        /// <summary>
        /// 後台，判斷使用者所在群組是否有觀看左側選單指定項目的權限
        /// 主要是為了防止在產生選單時一直抓取 SQL 資料，所以第一個參數給入要判斷的群組相關的權限設定
        /// </summary>
        /// <param name="permissions">使用者所在群組的選單權限列表</param>
        /// <param name="siteId"></param>
        /// <param name="menuType">選單類型：1: 後台選單，2: 前台選單</param>
        /// <param name="menuId">選單ID</param>
        /// <param name="groupId">Group ID，若不指定則為目前登入使用所屬的 Group</param>
        /// <returns>回傳是否有觀看權限</returns>
        public static bool HavePermission(IEnumerable<WorkV3.Areas.Backend.Models.GroupPermissionModels> permissions, long siteId, Areas.Backend.Abstracts.Menu item, long groupId = -1)
        {
            long menuId = item.ID;
            int menuType;

            string typeName = item.GetType().Name;
            switch (typeName)
            {
                case "MenusModels":
                default:
                    menuType = 2;
                    break;
                case "BackendMenuModel":
                    menuType = 1;
                    break;
            }

            Areas.Backend.Models.MemberModels curUser = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent;
            if (curUser == null && groupId == -1)
                return false;

            bool returnValue = false;
            groupId = groupId == -1 ? curUser.GroupId : groupId;
            Areas.Backend.Models.GroupModels group = Areas.Backend.Models.GroupDAO.GetItem(groupId, siteId);

            if (group.GroupType == 0)
                group.GroupType = 1;

            if (group.GroupType == 1)       // 權限全開，使用黑名單設定不能進入的選單
            {
                returnValue = true;

                IEnumerable<WorkV3.Areas.Backend.Models.GroupPermissionModels> filtered = permissions.Where(m => m.MenuType == menuType && m.MenuID == menuId);

                return !filtered.Any();
            }
            else if (group.GroupType == 2)   // 權限全關，使用白名單設定能夠進入的選單
            {
                foreach (Areas.Backend.Models.GroupPermissionModels permission in permissions)
                {
                    // 只要發現 true 直接回傳
                    if (permission.MenuType == menuType && permission.MenuID == menuId)
                        return true;
                }

                // 子選單如果有權限的話，該選單也回傳 true
                IEnumerable<WorkV3.Areas.Backend.Abstracts.Menu> childrenMenu = WorkV3.Areas.Backend.Models.DataAccess.BackendMenuDAO.GetChildren(menuId);
                foreach (WorkV3.Areas.Backend.Abstracts.Menu menu in childrenMenu)
                {
                    // recursive
                    returnValue = HavePermission(permissions, siteId, menu, groupId);

                    if (returnValue)
                        return returnValue;
                }
            }

            return returnValue;
        }
        // shan 20180829 新增後台管理能權限判斷(不分站台的)
        /// <summary>
        /// 後台，判斷使用者所在群組是否有觀看左側選單指定項目的權限
        /// 主要是為了防止在產生選單時一直抓取 SQL 資料，所以第一個參數給入要判斷的群組相關的權限設定
        /// </summary>
        /// <param name="permissions">使用者所在群組的選單權限列表</param>
        /// <param name="siteId"></param>
        /// <param name="menuType">選單類型：1: 後台選單，2: 前台選單</param>
        /// <param name="menuId">選單ID</param>
        /// <param name="groupId">Group ID，若不指定則為目前登入使用所屬的 Group</param>
        /// <returns>回傳是否有觀看權限</returns>
        public static bool HavePermission(IEnumerable<WorkV3.Areas.Backend.Models.GroupPermissionModels> permissions, Areas.Backend.Abstracts.Menu item, long groupId = -1)
        {
            long menuId = item.ID;
            int menuType;

            string typeName = item.GetType().Name;
            switch (typeName)
            {
                case "MenusModels":
                default:
                    menuType = 2;
                    break;
                case "BackendMenuModel":
                    menuType = 1;
                    break;
            }

            Areas.Backend.Models.MemberModels curUser = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent;
            if (curUser == null && groupId == -1)
                return false;

            bool returnValue = false;
            groupId = groupId == -1 ? curUser.GroupId : groupId;
            Areas.Backend.Models.GroupModels group = Areas.Backend.Models.GroupDAO.GetItem(groupId);

            if (group.GroupType == 0)
                group.GroupType = 1;

            if (group.GroupType == 1)       // 權限全開，使用黑名單設定不能進入的選單
            {
                returnValue = true;
                if (permissions == null)
                    return true;
                IEnumerable<WorkV3.Areas.Backend.Models.GroupPermissionModels> filtered = permissions.Where(m => m.MenuType == menuType && m.MenuID == menuId);

                return !filtered.Any();
            }
            else if (group.GroupType == 2)   // 權限全關，使用白名單設定能夠進入的選單
            {
                foreach (Areas.Backend.Models.GroupPermissionModels permission in permissions)
                {
                    // 只要發現 true 直接回傳
                    if (permission.MenuType == menuType && permission.MenuID == menuId)
                        return true;
                }

                // 子選單如果有權限的話，該選單也回傳 true
                IEnumerable<WorkV3.Areas.Backend.Abstracts.Menu> childrenMenu = WorkV3.Areas.Backend.Models.DataAccess.BackendMenuDAO.GetChildren(menuId);
                foreach (WorkV3.Areas.Backend.Abstracts.Menu menu in childrenMenu)
                {
                    // recursive
                    returnValue = HavePermission(permissions, menu, groupId);

                    if (returnValue)
                        return returnValue;
                }
            }

            return returnValue;
        }

        public static bool HavePermission(long groupId, long siteId, int menuType, long menuId)
        {
            return WorkV3.Areas.Backend.Models.DataAccess.GroupPermissionDAO.HaverPermission(groupId, siteId, menuType, menuId);
        }

        public static bool HavePermission(int menuType, long menuId, long siteId)
        {
            Areas.Backend.Models.MemberModels member =  Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent;
            if (member == null)
                return false;

            return HavePermission(member.GroupId, siteId, menuType, menuId);
        }

        // neil 20180508 新增權限判斷 end

        public static bool CheckIsLogin() {
            Areas.Backend.Models.MemberModels curUser = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent;
            if (curUser == null)
                return false;
            return true;
        }
    }
}