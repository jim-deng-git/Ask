using Dapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
//using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.ViewModels;
using WorkV3.Common;
using WorkV3.Models.DataAccess;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    /// <summary>
    /// Epaper DAO
    /// </summary>
    public class EpaperDAO
    {
        #region 共用或未分類
        /// <summary>
        /// 變更刊登狀態
        /// </summary>
        /// <param name="id"></param>
        public static void ToggleIssue(long id)
        {
            CommonDA.ToggleIssue("Epaper", id);
        }
        /// <summary>
        /// 取得身分類型名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetUserTypeName(string UserFilterStr)
        {
            if (string.IsNullOrWhiteSpace(UserFilterStr))
                return new List<string>();

            var array = UserFilterStr.Split(',');
            if (array.Length > 0)
            {
                List<string> result = new List<string>();
                if (array.Any(m => m == FilterPara.AllMember.ToString()) ||
                    (array.Any(m => m == FilterPara.AllMember.ToString()) && array.Any(m => m == FilterPara.AllUsers.ToString())))
                {
                    if (array.Any(m => m == FilterPara.AllMember.ToString()))
                        result.Add(GetEnumDescription<FilterPara>(FilterPara.AllMember.ToString()));
                    if (array.Any(m => m == FilterPara.AllUsers.ToString()))
                        result.Add(GetEnumDescription<FilterPara>(FilterPara.AllUsers.ToString()));
                }
                else
                {
                    array = array.Where(m => m != FilterPara.AllMember.ToString()).ToArray();
                    var source = Backend.Models.DataAccess.CategoryDAO.GetItems(CategoryType.Identity.ToString());
                    foreach (var item in array)
                    {
                        long n;
                        if (long.TryParse(item, out n))
                        {
                            var name = source.Any(m => m.ID == n) ? source.Where(m => m.ID == n).FirstOrDefault().Title : "";
                            if (!string.IsNullOrEmpty(name))
                                result.Add(name);
                        }
                        else if (item == FilterPara.AllUsers.ToString())
                        {
                            result.Add(GetEnumDescription<FilterPara>(FilterPara.AllUsers.ToString()));
                        }
                    }
                }
                return result;

            }
            else
                return new List<string>();
        }
        /// <summary>
        /// 取得列舉 Description Attribute 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(string value)
        {
            Type type = typeof(T);
            string name = Enum.GetNames(type)
                              .Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                              .Select(d => d)
                              .FirstOrDefault();

            //找無相對應的列舉
            if (name == null)
                return string.Empty;
            //找出相對應的欄位
            FieldInfo field = type.GetField(name);
            //取得欄位設定DescriptionAttribute的值
            var customAttribute = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            //無設定Description Attribute, 回傳Enum欄位名稱
            if (customAttribute == null || customAttribute.Length == 0)
                return name;

            //回傳Description Attribute的設定
            return ((System.ComponentModel.DescriptionAttribute)customAttribute[0]).Description;
        }
        /// <summary>
        /// 檢查Email (返回訊息)
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool checkEmailFormat(string Email, out string msg)
        {
            Email = Email ?? string.Empty;
            msg = string.Empty;
            Regex rgx = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            if (!rgx.IsMatch(Email))
            {
                msg = "Email格式錯誤";
                return false;
            }
            else
                return true;
        }
        /// <summary>
        /// 檢查Email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static bool checkEmailFormat(string Email)
        {
            Email = Email ?? string.Empty;
            Regex rgx = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            if (!rgx.IsMatch(Email))
            {
                return false;
            }
            else
                return true;
        }
        /// <summary>
        /// 取得某Column在MemberShipRegColumnSet是否被啟用
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool GetIsOpenInMemberShip(string columnName)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select IsOpen from MemberShipRegColumnSet where TableName='MemberShip' and ColumnName='{columnName.Replace("'", "''")}'";
                var query = conn.Query<bool>(sql);
                if (!query.Any())
                    return false;
                else
                    return query.FirstOrDefault();
            }
        }
        /// <summary>
        /// 查詢會員是否存在 (Membership)
        /// </summary>
        /// <param name="MembershipID"></param>
        /// <returns></returns>
        public static bool CheckMemberIsExist(long MembershipID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "Select 1 From MemberShip Where ID = " + MembershipID;
            return db.GetFirstValue(sql) != null;
        }
        /// <summary>
        /// 檢查double並回傳字串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string checkDoubleAndReturnStr(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "0";
            else
                return value.ToString();
        }
        /// <summary>
        /// 檢查double並回傳 double數值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double checkDoubleAndReturnDouble(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return 0;
            else
                return value;
        }
        /// <summary>
        /// 單純算出月的差異
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        private static int MonthDifference(DateTime Start, DateTime End)
        {
            return Math.Abs((Start.Month - End.Month) + 12 * (Start.Year - End.Year));
        }
        /// <summary>
        /// 算出週的差異
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int GetWeeksDiff(DateTime start, DateTime end)
        {
            start = GetStartOfWeek(start);
            end = GetStartOfWeek(end);
            int days = (int)(end - start).TotalDays;
            return (days / 7) + 1; // Adding 1 to be inclusive
        }
        /// <summary>
        /// 取得該週的第一天
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetStartOfWeek(DateTime input)
        {
            // Using +6 here leaves Monday as 0, Tuesday as 1 etc.
            int dayOfWeek = (((int)input.DayOfWeek) + 6) % 7;
            return input.Date.AddDays(-dayOfWeek);
        }
        /// <summary>
        /// 查詢資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id">KEY</param>
        /// <param name="WhereColumn">KEY COLUMN NAME</param>
        /// <param name="WhereCondition">其他where條件</param>
        /// <returns></returns>
        public static T GetItem<T>(string tableName, long id, string WhereColumn = "ID", string WhereCondition = "")
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = string.Format("Select * From " + tableName + " Where " + WhereColumn + " = " + id + " {0}", WhereCondition);
                return conn.Query<T>(sql).SingleOrDefault();
            }
        }
        /// <summary>
        /// 查詢資料 (IEnumerable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id">KEY</param>
        /// <param name="WhereColumn">KEY COLUMN NAME</param>
        /// <param name="WhereCondition">其他where條件</param>
        /// <returns></returns>
        public static IEnumerable<T> GetItems<T>(string tableName, long id, string WhereColumn = "ID", string WhereCondition = "")
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = string.Format("Select * From " + tableName + " Where " + WhereColumn + " = " + id + " {0}", WhereCondition);
                return conn.Query<T>(sql);
            }
        }
        /// <summary>
        /// 產生excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">資料來源</param>
        /// <param name="excelTyp">產生excel種類</param>
        /// <returns></returns>
        public static IWorkbook GenExcel<T>(IEnumerable<T> source, ExcelFormat excelTyp, string CustomName = null) where T : class
        {
            if (source == null)
                return null;

            IWorkbook wb;
            if (excelTyp == ExcelFormat.xls)
                wb = new HSSFWorkbook();
            else
                wb = new XSSFWorkbook();

            //IWorkbook wb = new HSSFWorkbook(); //xls格式
            //IWorkbook wb = new XSSFWorkbook(); //xlsx格式

            GenWorkSheet(source, CustomName, wb);

            return wb;
        }
        /// <summary>
        /// 產生worksheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="CustomName"></param>
        /// <param name="wb"></param>
        public static void GenWorkSheet<T>(IEnumerable<T> source, string CustomName, IWorkbook wb) where T : class
        {
            ISheet ws = wb.CreateSheet((string.IsNullOrWhiteSpace(CustomName) ? "Sheet1" : CustomName));
            ws.CreateRow(0);
            int _width = 25;
            List<string> propName = new List<string>();
            Type typ = typeof(T);
            PropertyInfo[] props = typ.GetProperties();
            for (int i = 0; i < props.Count(); i++)
            {
                var da = props[i].GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                ws.GetRow(0).CreateCell(i).SetCellValue(da.Name);
                propName.Add(props[i].Name);
                ws.SetColumnWidth(i, (int)((_width + 0.72) * 256));
            }

            for (int i = 1; i <= source.Count(); i++)
            {

                T data = source.ElementAt(i - 1);
                ws.CreateRow(i);
                for (int j = 0; j < propName.Count; j++)
                {
                    string v = typeof(T).GetProperty(propName[j]).GetValue(data, null) as string;
                    ws.GetRow(i).CreateCell(j).SetCellValue(v);
                }
            }
        }
        #endregion

        #region 電子報設定
        /// <summary>
        /// 透過SiteID取得電子報設定資訊
        /// </summary>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static EpaperSettingModel GetEpaperSetting(long SiteID)
        {
            if (SiteID == 0)
                return null;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select * From EpaperSetting Where SiteID = " + SiteID;
                var datas = conn.Query<EpaperSettingModel>(sql);
                if (datas.Count() > 0)
                    return datas.SingleOrDefault();
                else
                    return new EpaperSettingModel()
                    {
                        SiteID = SiteID,
                        EpaperTimeOut = 60,
                        EpaperSendFailRounds = 3,
                        EpaperSendIntervalMin = 40,
                        EpaperSendIntervalMax = 60,
                        EpaperSubscribeLike = 2
                    };
            }
        }
        /// <summary>
        /// 確認電子報設定
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="msg">回傳訊息</param>
        /// <returns></returns>
        public static bool CheckEpaperSetting(long SiteID, out string msg)
        {
            IEnumerable<EpaperSettingModel> source;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select * From EpaperSetting Where SiteID = " + SiteID;
                source = conn.Query<EpaperSettingModel>(sql);
            }

            if (source.Count() == 0)
            {
                msg = "「電子報設定」尚未設定";
                return false;
            }
            EpaperSettingModel setting = source.SingleOrDefault();
            if (string.IsNullOrWhiteSpace(setting.EpaperSmtpServer))
            {
                msg = "「電子報設定」Smtp伺服器尚未設定";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(setting.EpaperEmailAcc))
            {
                msg = "「電子報設定」Email帳號尚未設定";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(setting.EpaperEmailPwd))
            {
                msg = "「電子報設定」Email密碼尚未設定";
                return false;
            }
            else if (setting.EpaperEmailPort == 0)
            {
                msg = "「電子報設定」Port尚未設定";
                return false;
            }
            msg = string.Empty;
            return true;
        }
        /// <summary>
        /// 儲存電子報設定
        /// </summary>
        /// <param name="model"></param>
        public static void SetEpaperSetting(EpaperSettingModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperSetting");
            string check = "Select EpaperOpenToMember From EpaperSetting Where SiteID = " + item.SiteID;
            var GetSettingData = db.GetDataTable(check);
            bool userEpaperOpenToMember = item.EpaperOpenToMember; //使用者端設定的"是否開放會員訂閱"
            bool? dbEpaperOpenToMember = null;//DB端目前設定的"是否開放會員訂閱"
            bool isNew = GetSettingData.Rows.Count == 0;

            if (!isNew)
            {
                //取得db中的"是否開放會員訂閱"設定
                if (GetSettingData.Rows.Count > 0)
                    dbEpaperOpenToMember = (bool)((DataRow)GetSettingData.Rows[0])["EpaperOpenToMember"];
            }
            else
            {
                //檢查來源資料, 如果string為null塞empty
                foreach (var prop in typeof(EpaperSettingModel).GetProperties())
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        object value = prop.GetValue(item, null);
                        if (value == null)
                        {
                            prop.SetValue(item, string.Empty, null);
                        }
                    }
                }
            }

            tableObj.GetDataFromObject(item);

            if (isNew)
            {
                try
                {
                    tableObj.Insert();
                }
                catch
                {
                }
                finally
                {
                    if (userEpaperOpenToMember)
                        SetMemRegColSetForEpaperToOn(item.SiteID);
                }
            }
            else
            {
                tableObj.Remove("SiteID");
                try
                {
                    tableObj.Update(item.SiteID);
                }
                catch
                {
                }
                finally
                {
                    if (dbEpaperOpenToMember != null)
                    {
                        //當使用者將"是否開放會員訂閱"開啟,但DB中為關閉, 則執行
                        if (!(bool)dbEpaperOpenToMember && userEpaperOpenToMember)
                            SetMemRegColSetForEpaperToOn(item.SiteID);
                    }
                }
            }
        }
        /// <summary>
        /// 設定會員註冊設定中的電子報部分為啟用
        /// </summary>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static bool SetMemRegColSetForEpaperToOn(long SiteID)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string check = $"select 1 from MemberShipRegColumnSet where SiteID={SiteID} and TableName='MemberShip' and ColumnName='OrderEpaper' ";
                if (conn.Query<int>(check).Any())
                {
                    string sql = $"update MemberShipRegColumnSet set IsOpen=1 where SiteID={SiteID} and TableName='MemberShip' and ColumnName='OrderEpaper' ";
                    conn.Execute(sql);
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion

        #region 電子報報別
        /// <summary>
        /// 取得電子報報別
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="IssueOnly">是否只取得沒有休刊的部分。預設true</param>
        /// <returns></returns>
        public static List<EpaperTypeModel> GetEpaperType(long siteId, bool IssueOnly = true)
        {
            string sql = $"select * from EpaperType Where SiteID={siteId}";
            if (IssueOnly)
                sql += " and Status = 1";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<EpaperTypeModel>(sql).ToList();
            }
        }
        /// <summary>
        /// 取得電子報報別 (電子報報別管理)
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="siteId"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<EpaperTypeModel> GetEpaperType(int pageSize, int pageIndex, long siteId, out int recordCount)
        {
            List<EpaperTypeModel> list = new List<EpaperTypeModel>();
            string sql = $"Select * From EpaperType where SiteID={siteId} Order by Sort";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new EpaperTypeModel
                {
                    ID = (long)dr["ID"],
                    SiteID = (long)dr["SiteID"],
                    Name = dr["Name"] == null ? "" : dr["Name"].ToString().Trim(),
                    Intro = dr["Intro"] == null ? "" : dr["Intro"].ToString().Trim(),
                    Picture = dr["Picture"] == null ? "" : dr["Picture"].ToString().Trim(),
                    Status = (bool)dr["Status"],
                    Sort = (int)dr["Sort"]
                });
            }
            return list;
        }
        /// <summary>
        /// 電子報報別排序
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="items"></param>
        public static void EpaperTypeSort(long menuId, IEnumerable<SortItem> items)
        {
            CommonDA.Sort("EpaperType", items, string.Empty);
        }
        /// <summary>
        /// 電子報報別刪除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static string EpaperTypeDelete(IEnumerable<long> ids)
        {
            string msg = string.Empty;
            List<long> used = new List<long>();
            if (ids.Count() > 0)
            {
                //檢查欲刪除報別是否已經被使用，如有則排除
                for (int i = 0; i < ids.Count(); i++)
                {
                    if (CheckEpaperTypeUsed(ids.ElementAt(i)))
                    {
                        used.Add(ids.ElementAt(i));
                    }
                }
                ids = ids.Where(m => !used.Contains(m));
            }


            if (ids?.Count() != 0)
            {
                string sql = "Delete From EpaperType Where ID In ({0}) ";
                sql += " Delete From EpaperSubscriberDetail Where EpaperType_ID In ({0})";

                try
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                    {
                        conn.Execute(string.Format(sql, string.Join(", ", ids)));
                    }
                }
                catch
                {
                    return msg;
                }
            }

            if (used.Count() > 0)
            {
                List<string> typeName = new List<string>();
                foreach (var typeID in used)
                {
                    var item = GetEpaperTypeItem(typeID);
                    if (item != null)
                        typeName.Add(item.Name);
                }
                if (typeName.Count > 0)
                    msg = string.Format("報別「{0}」已被引用故無法刪除，建議使用休刊方式停止該報別的發送", string.Join(",", typeName));
            }
            return msg;
        }
        /// <summary>
        /// 取得電子報報別單筆資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EpaperTypeModel GetEpaperTypeItem(long id)
        {
            return CommonDA.GetItem<EpaperTypeModel>("EpaperType", id);
        }
        /// <summary>
        /// 儲存電子報報別
        /// </summary>
        /// <param name="item"></param>
        public static void SetEpaperTypeItem(EpaperTypeModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperType");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From EpaperType Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 檢查報別是否已經被使用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static bool CheckEpaperTypeUsed(long code)
        {
            if (code == 0)
                return false;

            string sql = $"select ID from Epaper where EpaperType_ID='{ code }'";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas.Rows.Count > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 取得報別名稱
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <returns></returns>
        public static string GetEpaperTypeName(long Epaper_ID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = @"select Name from EpaperType where ID=(select EpaperType_ID from Epaper where ID=" + Epaper_ID + ")";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                return dr["Name"].ToString().Trim();
            }
            return string.Empty;
        }
        #endregion

        #region 電子報編輯
        /// <summary>
        /// 取得電子報單筆資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EpaperModel GetEpaperItem(long id)
        {
            EpaperModel epaperModel = CommonDA.GetItem<EpaperModel>("Epaper", id);
            if (epaperModel.Style != WorkV3.Areas.Backend.Models.EpaperType.Custom.ToString())
            {
                List<EpaperAreaModel> HeadAreaModels = GetEpaperAreas(id, EpaperAreaType.Head);
                List<EpaperAreaModel> BodyAreaModels = GetEpaperAreas(id, EpaperAreaType.Content);
                var ParagraphModels = Models.DataAccess.ParagraphDAO.GetItems(id);
                List<EpaperAreaModel> FooterAreaModels = GetEpaperAreas(id, EpaperAreaType.Footer);
                if (HeadAreaModels.Count > 0)
                    epaperModel.HeadAreaModel = HeadAreaModels[0];
                epaperModel.BodyAreaModel = BodyAreaModels;
                if (FooterAreaModels.Count > 0)
                    epaperModel.FooterAreaModel = FooterAreaModels[0];
                epaperModel.ParagraphModels = ParagraphModels;
            }
            if (epaperModel.Style == WorkV3.Areas.Backend.Models.EpaperType.Normal.ToString())
            {
                List<EpaperToModules> EpaperModules = GetEpaperModules(id);
                epaperModel.EpaperModules = EpaperModules;
            }
            return epaperModel;
        }
        /// <summary>
        /// 取得EpaperArea 電子報內容模組
        /// </summary>
        /// <param name="EpaperID"></param>
        /// <param name="AreaType"></param>
        /// <returns></returns>
        public static List<EpaperAreaModel> GetEpaperAreas(long EpaperID, EpaperAreaType AreaType)
        {
            string sql = "select * from EpaperAreas Where EpaperID = @EpaperID AND AreaType=@AreaType ORDER BY Sort ";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<EpaperAreaModel>(sql, new { @EpaperID = EpaperID, @AreaType = AreaType.ToString() }).ToList();
            }
        }
        /// <summary>
        /// 取得 電子報關聯模組 主檔資料
        /// </summary>
        /// <param name="EpaperID"></param>
        /// <returns></returns>
        public static List<EpaperToModules> GetEpaperModules(long EpaperID)
        {
            string sql = "select * from EpaperToModules Where EpaperID = @EpaperID  ORDER BY Sort ASC ";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var ModuleList = conn.Query<EpaperToModules>(sql, new { @EpaperID = EpaperID }).ToList();
                foreach (EpaperToModules module in ModuleList)
                {
                    if (module.ModuleID == EpaperModuleType.Article.ToString())
                        module.PageList = GetEpaperToPages(EpaperModuleType.Article, module.ID);
                    if (module.ModuleID == EpaperModuleType.Event.ToString())
                        module.PageList = GetEpaperToPages(EpaperModuleType.Event, module.ID);
                    if (module.ModuleID == EpaperModuleType.Questionnaire.ToString())
                        module.PageList = GetEpaperToPages(EpaperModuleType.Questionnaire, module.ID);
                }
                return ModuleList;
            }
        }
        /// <summary>
        /// 電子報關聯模組 取得EpaperModule資料
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="EpaperModuleID"></param>
        /// <returns></returns>
        public static List<ViewModels.EpaperToPageViewModel> GetEpaperToPages(EpaperModuleType moduleType, long EpaperModuleID)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@EpaperModuleID", EpaperModuleID);
            List<ViewModels.EpaperToPageViewModel> list = new List<ViewModels.EpaperToPageViewModel>();
            string mainTableName = moduleType.ToString();
            switch (moduleType)
            {
                case EpaperModuleType.Article:
                    mainTableName = "Article";
                    break;
                case EpaperModuleType.Event:
                    mainTableName = "Events";
                    break;
                case EpaperModuleType.Questionnaire:
                    mainTableName = "Questionnaire";
                    break;
            }
            string sql = string.Format(@"Select EpaperToPages.*, {0}.Title, Pages.SiteID AS SiteID, Pages.MenuID AS MenuID, 
                                        Pages.No AS PageNo, Pages.SN, {0}.Icon From EpaperToPages 
	                        JOIN {0} ON {0}.ID=EpaperToPages.PageID
	                        JOIN Cards ON Cards.No={0}.CardNo
	                        JOIN Zones ON Zones.No=Cards.ZoneNo
	                        JOIN Pages ON Pages.No=Zones.PageNo
                            WHERE EpaperToPages.EpaperModuleID=@EpaperModuleID Order by Sort", mainTableName);
            if (moduleType == EpaperModuleType.Questionnaire)
                sql = @"Select EpaperToPages.*, Form.Title, Pages.SiteID AS SiteID, Pages.MenuID AS MenuID, 
                                        Pages.No AS PageNo, Pages.SN, Form.Image AS Icon From EpaperToPages 
	                        JOIN Questionnaire ON Questionnaire.ID=EpaperToPages.PageID
							JOIN Form ON Form.ID=Questionnaire.ID
	                        JOIN Cards ON Cards.No=Questionnaire.CardNo
	                        JOIN Zones ON Zones.No=Cards.ZoneNo
	                        JOIN Pages ON Pages.No=Zones.PageNo
                            WHERE EpaperToPages.EpaperModuleID=@EpaperModuleID Order by Sort";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql, paraList);
            foreach (DataRow dr in datas.Rows)
            {
                ViewModels.EpaperToPageViewModel vModel = new ViewModels.EpaperToPageViewModel()
                {
                    ID = dr["ID"].ToString(),
                    EpaperModuleID = dr["EpaperModuleID"].ToString(),
                    SiteID = (long)dr["SiteID"],
                    MenuID = (long)dr["MenuID"],
                    PageID = (long)dr["PageID"],
                    Title = dr["Title"].ToString(),
                    Url = dr["SN"].ToString(),
                    Img = dr["Icon"].ToString(),
                    Sort = (int)dr["Sort"]
                };
                if (moduleType == EpaperModuleType.Article)
                {
                    if (!string.IsNullOrEmpty(vModel.Img))
                    {
                        ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(vModel.Img);
                        vModel.Img = Golbal.UpdFileInfo.GetVPathByMenuID(vModel.SiteID, vModel.MenuID).TrimEnd('/') + "/" + imgModel.Img;
                    }
                    else
                    {
                        vModel.Img = WorkV3.Models.DataAccess.PagesDAO.GetContentImage(vModel.SiteID, vModel.MenuID, (long)dr["PageNo"]);
                        if (vModel.Img.Contains("\"Img\":"))
                        {
                            ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(vModel.Img);
                            vModel.Img = Golbal.UpdFileInfo.GetVPathByMenuID(vModel.SiteID, vModel.MenuID).TrimEnd('/') + "/" + imgModel.Img;
                        }
                    }
                }

                if (moduleType == EpaperModuleType.Event)
                {
                    WorkV3.Models.EventModels eModel = new WorkV3.Models.EventModels();
                    eModel = WorkV3.Models.DataAccess.EventDAO.GetItem(vModel.PageID);
                    vModel.Img = GetEventCoverImage(vModel.SiteID, vModel.MenuID, eModel);
                }
                if (moduleType == EpaperModuleType.Questionnaire)
                {
                    if (!string.IsNullOrEmpty(vModel.Img))
                    {
                        vModel.Img = Golbal.UpdFileInfo.GetVPathByMenuID(vModel.SiteID, vModel.MenuID).TrimEnd('/') + "/" + vModel.Img;
                    }
                }

                list.Add(vModel);
            }
            return list;
        }
        /// <summary>
        /// 電子報關聯模組 取得各項關聯類型的資料
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="keyWord"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static IEnumerable<ViewModels.EpaperSearchPagesViewModel> GetModulePages(EpaperModuleType moduleType, string keyWord, int pageSize, int pageIndex, out int recordCount)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            List<ViewModels.EpaperSearchPagesViewModel> list = new List<ViewModels.EpaperSearchPagesViewModel>();

            List<string> where = new List<string>();
            string sql = "";
            if (moduleType == EpaperModuleType.Article)
                sql = string.Format(@"Select Article.*, Pages.SiteID AS SiteID, Pages.MenuID AS MenuID, 
                                        Pages.No AS PageNo, Pages.SN AS PageSN,Sites.SN AS SiteSN, Article.Icon From Article 
                            JOIN Cards ON Cards.No=Article.CardNo
	                        JOIN Zones ON Zones.No=Cards.ZoneNo
	                        JOIN Pages ON Pages.No=Zones.PageNo
                            JOIN Sites ON Sites.ID=Pages.SiteID
                            WHERE Article.Title LIKE '%" + keyWord.Replace("'", "") + @"%' {0}
                             Order by Sort ASC,CreateTime DESC", string.Join(" And ", where));//wei 20180713 順序
            if (moduleType == EpaperModuleType.Event)
                sql = string.Format(@"Select Events.*, Pages.SiteID AS SiteID, Pages.MenuID AS MenuID, 
                                        Pages.No AS PageNo, Pages.SN AS PageSN,Sites.SN AS SiteSN, Events.Icon From Events
	                        JOIN Cards ON Cards.No=Events.CardNo
	                        JOIN Zones ON Zones.No=Cards.ZoneNo
	                        JOIN Pages ON Pages.No=Zones.PageNo
                            JOIN Sites ON Sites.ID=Pages.SiteID
                            WHERE Events.Title LIKE '%" + keyWord.Replace("'", "") + @"%' {0}
                             Order by Sort ASC,CreateTime DESC", string.Join(" And ", where));//wei 20180713 順序
            if (moduleType == EpaperModuleType.Questionnaire)
                sql = string.Format(@"Select Questionnaire.*, Form.Title, Pages.SiteID AS SiteID, Pages.MenuID AS MenuID, 
                                        Pages.No AS PageNo, Pages.SN AS PageSN,Sites.SN AS SiteSN, Form.Image AS Icon From Questionnaire
							JOIN Form ON Form.ID=Questionnaire.ID
	                        JOIN Cards ON Cards.No=Questionnaire.CardNo
	                        JOIN Zones ON Zones.No=Cards.ZoneNo
	                        JOIN Pages ON Pages.No=Zones.PageNo
                            JOIN Sites ON Sites.ID=Pages.SiteID
                            WHERE Form.Title LIKE '%" + keyWord.Replace("'", "") + @"%'  {0}
                            Order by Sort ASC,CreateTime DESC", string.Join(" And ", where));//wei 20180713 順序
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            foreach (DataRow dr in datas.Rows)
            {
                UrlHelper urlHelper = new UrlHelper();
                //string url = urlHelper.Action("Index", "Home", new { SiteSN = dr["SiteID"].ToString(), PageSN = dr["PageNo"].ToString() });
                ViewModels.EpaperSearchPagesViewModel vModel = new ViewModels.EpaperSearchPagesViewModel()
                {
                    PageSN = dr["PageSN"].ToString(),
                    SiteSN = dr["SiteSN"].ToString(),
                    ID = dr["ID"].ToString(),
                    SiteID = (long)dr["SiteID"],
                    MenuID = (long)dr["MenuID"],
                    Title = dr["Title"].ToString(),
                    Url = "",
                    Img = dr["Icon"].ToString(),//
                    Sort = (int)dr["Sort"]
                };
                if (moduleType == EpaperModuleType.Article)
                {
                    string typeName = "";
                    var typeList = ArticleDAO.GetItemTypes(long.Parse(vModel.ID));
                    if (typeList != null && typeList.Count() > 0)
                    {
                        foreach (long typeID in typeList)
                        {
                            ArticleTypesModels typeModel = ArticleTypesDAO.GetItem(typeID);
                            typeName += typeModel.Name + "、";
                        }
                        typeName = typeName.Trim('、');
                    }
                    vModel.TypeName = typeName;
                    if (!string.IsNullOrEmpty(vModel.Img))
                    {
                        ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(vModel.Img);
                        vModel.Img = Golbal.UpdFileInfo.GetVPathByMenuID(vModel.SiteID, vModel.MenuID).TrimEnd('/') + "/" + imgModel.Img;
                    }
                    else
                    {
                        vModel.Img = WorkV3.Models.DataAccess.PagesDAO.GetContentImage(vModel.SiteID, vModel.MenuID, (long)dr["PageNo"]);
                        if (vModel.Img.Contains("\"Img\":"))
                        {
                            ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(vModel.Img);
                            vModel.Img = Golbal.UpdFileInfo.GetVPathByMenuID(vModel.SiteID, vModel.MenuID).TrimEnd('/') + "/" + imgModel.Img;
                        }
                    }
                }

                if (moduleType == EpaperModuleType.Event)
                {
                    string typeName = "";
                    var typeList = WorkV3.Models.DataAccess.EventDAO.GetItemTypes(long.Parse(vModel.ID));
                    if (typeList != null && typeList.Count() > 0)
                    {
                        foreach (WorkV3.Models.EventTypesModels typeModel in typeList)
                        {
                            typeName += typeModel.Name + "、";
                        }
                        typeName = typeName.Trim('、');
                    }
                    vModel.TypeName = typeName;
                    WorkV3.Models.EventModels eModel = new WorkV3.Models.EventModels();
                    eModel = WorkV3.Models.DataAccess.EventDAO.GetItem(long.Parse(vModel.ID));
                    vModel.Img = GetEventCoverImage(vModel.SiteID, vModel.MenuID, eModel);
                }
                if (moduleType == EpaperModuleType.Questionnaire)
                {
                    string typeName = "";
                    var typeList = WorkV3.Models.DataAccess.QuestionnaireDAO.GetItemTypes(long.Parse(vModel.ID));

                    if (typeList != null && typeList.Count() > 0)
                    {
                        foreach (long typeID in typeList)
                        {
                            WorkV3.Models.QuestionnaireTypeModels typeModel = WorkV3.Models.DataAccess.QuestionnaireTypeDAO.GetItem(typeID);
                            typeName += typeModel.Name + "、";
                        }
                        typeName = typeName.Trim('、');
                    }
                    vModel.TypeName = typeName;
                    if (!string.IsNullOrEmpty(vModel.Img))
                    {
                        vModel.Img = Golbal.UpdFileInfo.GetVPathByMenuID(vModel.SiteID, vModel.MenuID).TrimEnd('/') + "/" + vModel.Img;
                    }
                }

                list.Add(vModel);
            }
            return list;
        }
        /// <summary>
        /// 儲存電子報內容模組
        /// </summary>
        /// <param name="item"></param>
        public static void SetEpaperAreaItem(EpaperAreaModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperAreas");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From EpaperAreas Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 儲存電子報關聯模組
        /// </summary>
        /// <param name="item"></param>
        public static void SetEpaperModuleItem(EpaperToModules item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperToModules");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From EpaperToModules Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 電子報編輯一般模式 手動導入
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="EpaperModuleID"></param>
        /// <param name="typeList"></param>
        /// <param name="searchWays"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static bool ImportDataFromSetting(EpaperModuleType moduleType, string EpaperModuleID, string[] typeList, EpaperImportType searchWays, int rowCount)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string typeCond = "";
            for (int i = 0; i < typeList.Length; i++)
            {
                typeCond += typeList[i] + ",";
            }
            typeCond = typeCond.Trim(',');
            string sql = "";
            switch (moduleType)
            {
                case EpaperModuleType.Article:
                    sql = string.Format("SELECT top {0} * FROM Article WHERE ID IN (SELECT ArticleID FROM ArticleToType WHERE TypeID IN ({1}))", rowCount.ToString(), typeCond);
                    break;
                case EpaperModuleType.Event:
                    sql = string.Format(" SELECT top {0} * FROM Events WHERE ID IN (SELECT EventID FROM EventToType WHERE TypeID IN ({1}))", rowCount.ToString(), typeCond);
                    break;
                case EpaperModuleType.Questionnaire:
                    sql = string.Format(" SELECT top {0} * FROM Questionnaire WHERE ID IN (SELECT QuestionnaireID FROM QuestionnaireToType WHERE TypeID IN ({1}))", rowCount.ToString(), typeCond);
                    break;
            }
            switch (searchWays)
            {
                case EpaperImportType.CreateTimeDesc:
                    sql += " ORDER BY CreateTime DESC";
                    break;
                case EpaperImportType.SortAsc:
                    sql += " ORDER BY Sort ASC";
                    break;
                case EpaperImportType.Random:
                    sql += " ORDER BY NEWID() ";
                    break;
            }
            string del_old_sql = $" DELETE EpaperToPages WHERE EpaperModuleID={EpaperModuleID} ";
            db.ExecuteNonQuery(del_old_sql);
            DataTable autoLoadTable = db.GetDataTable(sql);
            if (autoLoadTable != null && autoLoadTable.Rows.Count > 0)
            {
                for (int i = 0; i < autoLoadTable.Rows.Count; i++)
                {
                    EpaperToPages item = new EpaperToPages();
                    item.ID = WorkLib.GetItem.NewSN();
                    item.EpaperModuleID = long.Parse(EpaperModuleID);
                    item.PageID = (long)autoLoadTable.Rows[i]["ID"];
                    item.Sort = (i + 1);
                    SQLData.TableObject tableObj = db.GetTableObject("EpaperToPages");
                    tableObj.GetDataFromObject(item);
                    tableObj.Insert();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 電子報編輯一般模式 手動導入
        /// </summary>
        /// <param name="EpaperModuleID"></param>
        /// <param name="selectList"></param>
        /// <param name="epaperToPageIds"></param>
        /// <returns></returns>
        public static bool ImportDataFromSelect(long EpaperModuleID, string[] selectList, string[] epaperToPageIds)
        {
            string[] addArray;//wei 20180720 改順序
            if (epaperToPageIds != null)
            {
                IEnumerable<long> currentPageIds = new List<long>();
                if (epaperToPageIds.Length > 0)
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                    {
                        string query = $"select PageID from EpaperToPages where ID in ({string.Join(",", epaperToPageIds)}) and EpaperModuleID={EpaperModuleID}";
                        currentPageIds = conn.Query<long>(query);
                    }
                }
                 addArray = currentPageIds.Select(m => m.ToString()).Union(selectList).ToArray();//wei 20180720 改順序
            }
            else { addArray = selectList; }//wei 20180720 改順序
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string del_old_sql = $" DELETE EpaperToPages WHERE EpaperModuleID={EpaperModuleID} ";
            db.ExecuteNonQuery(del_old_sql);
            int i = 0;
            foreach (string selectID in addArray)//wei 20180720 改順序
            {
                EpaperToPages item = new EpaperToPages();
                item.ID = WorkLib.GetItem.NewSN();
                item.EpaperModuleID = EpaperModuleID;
                item.PageID = long.Parse(selectID);
                item.Sort = (i + 1);
                SQLData.TableObject tableObj = db.GetTableObject("EpaperToPages");
                tableObj.GetDataFromObject(item);
                tableObj.Insert();
            }
            return true;

        }
        /// <summary>
        /// 電子報編輯一般模式 手動導入 刪除
        /// </summary>
        /// <param name="EpaperToPageID"></param>
        /// <returns></returns>
        public static bool DeleteSelectPage(long EpaperToPageID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string del_old_sql = $" DELETE EpaperToPages WHERE ID={EpaperToPageID} ";
            db.ExecuteNonQuery(del_old_sql);

            return true;

        }
        /// <summary>
        /// 取得活動圖片資訊
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="MenuID"></param>
        /// <param name="eModel"></param>
        /// <returns></returns>
        private static string GetEventCoverImage(long SiteID, long MenuID, WorkV3.Models.EventModels eModel)
        {
            if (eModel != null)
            {
                WorkV3.Models.EventSettingModels setting = WorkV3.Models.DataAccess.EventSettingDAO.GetItem(MenuID);
                string imgSrc = eModel.GetFirstImg(setting);
                if (string.IsNullOrEmpty(imgSrc))
                    return "";
                return WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, MenuID).TrimEnd('/') + "/" + imgSrc;
            }
            return "";
        }
        /// <summary>
        /// 電子報管理 > 取得電子報列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="siteId"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<EpaperModel> GetEpaper(EpaperSearchModel search, int pageSize, int pageIndex, long siteId, out int recordCount)
        {
            List<EpaperModel> list = new List<EpaperModel>();
            recordCount = 0;

            if (search == null)
                return list;

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(search.keyword))
            {
                sb.Append(string.Format(" and Epaper.Title like N'%{0}%'", search.keyword.Replace("'", "''")));
            }
            if (search.Publish_Date_Start != null && search.Publish_Date_End != null)
            {
                sb.Append($" and Epaper.Publish_Date_Start >= '{ search.Publish_Date_Start.ToString("yyyy/MM/dd")}' and Epaper.Publish_Date_Start <= '{ search.Publish_Date_End.ToString("yyyy/MM/dd") }'");
            }
            if (search.EpaperStyle != null)
            {
                if (!search.EpaperStyle.Any(m => m == "All") && search.EpaperStyle.Count() > 0)
                {
                    string[] temp = new string[search.EpaperStyle.Count()];
                    for (int i = 0; i < temp.Count(); i++)
                    {
                        temp[i] = "'" + search.EpaperStyle[i] + "'";
                    }
                    sb.Append(string.Format(" and Epaper.Style in ({0})", string.Join(", ", temp)));
                }
            }
            if (search.EpaperTypeSelect != null)
            {
                if (!search.EpaperTypeSelect.Any(m => m == "All") && search.EpaperTypeSelect.Count() > 0)
                {
                    sb.Append(string.Format(" and Epaper.EpaperType_ID in ({0})", string.Join(", ", search.EpaperTypeSelect)));
                }
            }
            string sql = @"
                            Select 
                            Epaper.*,
                            EpaperType.Name EpaperTypeName,
                            (select top 1 Status from EpaperSend where EpaperSend.Epaper_ID=Epaper.ID order by StatusChangeTime DESC) as SendStatus 
                            From Epaper 
                            left join EpaperType on Epaper.EpaperType_ID=EpaperType.ID
                            where Epaper.SiteID=" + siteId + @" and (Epaper.TemplateName is null or Epaper.TemplateName = '') {0}
                            Order by Epaper.Publish_Date_Start desc,Epaper.CreateTime desc
                            ";
            sql = string.Format(sql, sb.ToString());

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);

            if (datas == null)
                return list;

            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new EpaperModel
                {
                    ID = (long)dr["ID"],
                    SiteID = (long)dr["SiteID"],
                    Title = dr["Title"] == null ? "" : dr["Title"].ToString().Trim(),
                    Contents = dr["Contents"] == null ? "" : dr["Contents"].ToString().Trim(),
                    Publish_Date_Start = dr["Publish_Date_Start"] as DateTime?,
                    Publish_Date_End = dr["Publish_Date_End"] as DateTime?,
                    Style = dr["Style"].ToString().Trim(),
                    EpaperType_ID = (long)dr["EpaperType_ID"],
                    EpaperTypeName = dr["EpaperTypeName"] == null ? string.Empty : dr["EpaperTypeName"].ToString().Trim(),
                    IsCurrent = (bool)dr["IsCurrent"],
                    IsIssue = (bool)dr["IsIssue"],
                    //SendStatus = GetEpaperStatusName(dr["SendStatus"].ToString().Trim()),
                    EpaperSend = GetSendLatelyStatusChange((long)dr["ID"])
                });
            }
            return list;
        }
        /// <summary>
        /// 儲存電子報
        /// </summary>
        /// <param name="item"></param>
        public static void SetEpaperItem(EpaperModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Epaper");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From Epaper Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
            if (item.IsCurrent)
            {
                //當設定為當期時,取消其他同報別的電子報之當期狀態
                CancelIssueStatusInEapaer(item.EpaperType_ID, item.ID);
            }
        }
        /// <summary>
        /// 刪除電子報
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static int EpaperDel(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql = @"Delete From Epaper Where ID In ({0}) 
                           Delete From EpaperAreas Where EpaperID In ({0}) 
                           Delete From EpaperToModules Where EpaperID In ({0})
                           Delete From EpaperSend Where Epaper_ID In ({0})
                           ";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                IEnumerable<long> delEpaperToPagesIDs = conn.Query<long>($"select EpaperToPages.ID from EpaperToPages inner join EpaperToModules on EpaperToModules.ID=EpaperToPages.EpaperModuleID where EpaperToModules.EpaperID in ({string.Join(",", ids) })");
                IEnumerable<long> delEpaperSendListIDs = conn.Query<long>($"select EpaperSendList.ID from EpaperSendList inner join EpaperSend on EpaperSend.ID=EpaperSendList.EpaperSend_ID where EpaperSend.Epaper_ID in ({ string.Join(",", ids) })");
                if (delEpaperToPagesIDs.Count() != 0)
                    sql += $" Delete From EpaperToPages Where ID in ({string.Join(",", delEpaperToPagesIDs)})";
                if (delEpaperSendListIDs.Count() != 0)
                    sql += $" Delete From EpaperSendList Where ID in ({string.Join(",", delEpaperSendListIDs)})";
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }
        /// <summary>
        /// 透過EpaperType_ID取消dbo.Epaper的當期狀態並排除特定Epaper ID
        /// </summary>
        /// <param name="EpaperType_ID">報別ID</param>
        /// <param name="Epaper_ID">排除的Epapaer ID</param>
        static void CancelIssueStatusInEapaer(long EpaperType_ID, long Epaper_ID)
        {
            string sql = $"update Epaper set IsCurrent=0 where EpaperType_ID={EpaperType_ID} and ID<>{Epaper_ID}";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                conn.Execute(sql);
            }
        }
        /// <summary>
        /// 電子報主頁 "樣式"文字顯示
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetStyleName(string code)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(code))
            {
                return result;
            }
            switch (code)
            {
                case "Custom":
                    result = "自訂";
                    break;
                case "Normal":
                    result = "一般";
                    break;
                case "Paragraph":
                    result = "段落";
                    break;
                case "Pro":
                    result = "專家";
                    break;
            }
            return result;
        }
        /// <summary>
        /// 取得電子報範本
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static IEnumerable<WorkV3.Models.TemplateModels> GetTemplates(long siteId)
        {
            string sql = $"SELECT ID, TemplateName, TemplateThumb FROM Epaper WHERE SiteID = { siteId } AND TemplateName <> ''";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            List<WorkV3.Models.TemplateModels> templates = new List<WorkV3.Models.TemplateModels>();
            if (datas == null)
                return templates;
            foreach (DataRow dr in datas.Rows)
            {
                templates.Add(new WorkV3.Models.TemplateModels
                {
                    ID = (long)dr["ID"],
                    Name = dr["TemplateName"].ToString().Trim(),
                    Thumb = dr["TemplateThumb"].ToString().Trim()
                });
            }
            return templates;
        }
        /// <summary>
        /// 電子報段落複製
        /// </summary>
        /// <param name="epaper"></param>
        /// <param name="paragraphs"></param>
        public static void CopyParagraphAndImg(EpaperModel epaper, IEnumerable<WorkV3.Areas.Backend.Models.ParagraphModels> paragraphs)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            foreach (var item in paragraphs)
            {
                var newParaID = WorkLib.GetItem.NewSN();

                item.ID = newParaID;
                item.SourceNo = epaper.ID;
                ParagraphDAO.SetItem(item);

                var imgModels = ResourceImagesDAO.GetItems(item.ID);
                foreach (var imgModel in imgModels)
                {
                    imgModel.ID = WorkLib.GetItem.NewSN();
                    imgModel.SourceNo = newParaID;
                    ResourceImagesDAO.SetItem(imgModel);
                }
            }
        }
        #endregion

        #region 電子報發送
        /// <summary>
        /// 透過EpaperID查詢發送狀態
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <returns></returns>
        public static string GetSendStatusByEpaperID(long Epaper_ID)
        {
            string sql = $"select [Status] from EpaperSend where Epaper_ID= { Epaper_ID }";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var datas = conn.Query<EpaperSend>(sql);
                if (datas.Any())
                    return datas.FirstOrDefault().Status;
                else
                    return null;
            }
        }
        /// <summary>
        /// 發送狀態顯示文字
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="UseClass">回傳套用class style字串的類型。參數0 與 1</param>
        /// <param name="ClassStr">回傳套用class style字串</param>
        /// <param name="SendTime"></param>
        /// <param name="ExpectedSendCount">預計發送數量</param>
        /// <param name="RealSendCount">實際已發送數量</param>
        /// <returns></returns>
        public static string GetEpaperStatusName(string Status, int UseClass, out string ClassStr, DateTime? SendTime = null, 
                                                                                                   int ExpectedSendCount = 0, 
                                                                                                   int RealSendCount = 0)
        {

            if (string.IsNullOrEmpty(Status))
                Status = SendStatus.Start;
            else if (Status == SendStatus.Temp)
                Status = SendStatus.Setting;

            if (UseClass == 0)
                ClassStr = ClassStrByStatus(Status, UseClass);
            else
                ClassStr = ClassStrByStatus(Status, UseClass);

            try
            {
                if (Status == SendStatus.Sending)
                {
                    if (ExpectedSendCount != 0 && RealSendCount != 0 && (RealSendCount <= ExpectedSendCount))
                    {
                        return string.Format("發送 {0}%", Convert.ToInt16(Math.Round(
                                                               (Convert.ToDouble(RealSendCount) / Convert.ToDouble(ExpectedSendCount)) * 100, 0)
                                                             )
                                             );
                    }
                    else if (SendTime != null)
                    {
                        if (DateTime.Now < SendTime)
                            return "尚未發送";
                    }
                }
                if (Status == SendStatus.Pause)
                {
                    if (ExpectedSendCount != 0 && RealSendCount != 0 && (RealSendCount <= ExpectedSendCount))
                    {
                        return string.Format("暫停 {0}%", Convert.ToInt16(Math.Round(
                                                               (Convert.ToDouble(RealSendCount) / Convert.ToDouble(ExpectedSendCount)) * 100, 0)
                                                             )
                                             );
                    }
                }
                return ((DisplayAttribute)typeof(SendStatus).GetMember(Status).First().GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault()).Name;
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 根據狀態 取得Class Style Str
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="UseClass">回傳套用class style字串的類型。參數0 與 1</param>
        /// <returns></returns>
        public static string ClassStrByStatus(string Status, int UseClass)
        {
            string result = "";
            switch (UseClass)
            {
                case 0:
                    switch (Status)
                    {
                        case SendStatus.Start:
                            result = "grey-darken-2";
                            break;
                        case SendStatus.Setting:
                            result = "grey-darken-2";
                            break;
                        case SendStatus.Sending:
                            result = "blue";
                            break;
                        case SendStatus.Sended:
                            result = "grey-darken-4";
                            break;
                        case SendStatus.Pause:
                            result = "red";
                            break;
                        case SendStatus.SystemBreak:
                            result = "grey";
                            break;
                        case SendStatus.ManualBreak:
                            result = "grey";
                            break;
                        case SendStatus.Temp:
                            result = "grey";
                            break;
                        default:
                            result = "grey";
                            break;
                    }
                    break;
                case 1:
                    switch (Status)
                    {
                        case SendStatus.Start:
                            result = "font-grey";
                            break;
                        case SendStatus.Setting:
                            result = "font-grey";
                            break;
                        case SendStatus.Sending:
                            result = "font-blue";
                            break;
                        case SendStatus.Sended:
                            result = "font-black";
                            break;
                        case SendStatus.Pause:
                            result = "font-red";
                            break;
                        case SendStatus.SystemBreak:
                            result = "font-grey";
                            break;
                        case SendStatus.ManualBreak:
                            result = "font-grey";
                            break;
                        case SendStatus.Temp:
                            result = "font-grey";
                            break;
                        default:
                            result = "font-grey";
                            break;
                    }
                    break;
            }
            return result;
        }
        /// <summary>
        /// 取得最近變更的發送主檔
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <returns></returns>
        public static EpaperSend GetSendLatelyStatusChange(long Epaper_ID)
        {
            string sql = $"select top 1 * from EpaperSend where EpaperSend.Epaper_ID={Epaper_ID} order by StatusChangeTime DESC";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var datas = conn.Query<EpaperSend>(sql);
                if (datas.Any())
                {
                    EpaperSend data = datas.FirstOrDefault();
                    return data;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// 取得單一發送主檔
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <param name="EpaperSendID"></param>
        /// <returns></returns>
        public static EpaperSend GetSend(long Epaper_ID, long? EpaperSendID)
        {
            string sql = $"select * from EpaperSend where Epaper_ID= { Epaper_ID }" +
                         "and Status='Temp' {0}";
            sql = string.Format(sql, EpaperSendID != null ? $" and ID={ EpaperSendID }" : "");
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var datas = conn.Query<EpaperSend>(sql);
                if (datas.Any())
                {
                    EpaperSend data = datas.FirstOrDefault();
                    data.manualSendList = GetTempEmailDataBySendID(data.ID);
                    return data;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// 依關鍵字取得寄信列表
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <param name="EpaperSendID"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetSendByKeyword(string keyword, bool excludePause = true)
        {
            string sql = $"select TOP 50 Email from EpaperSubscriber where 1=1 {(excludePause ? "and IsPause=0" : "")} AND Email like '%{keyword}%' order by Email ";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<string>(sql);
            }
        }
        public static EpaperSubscriber GetEpaperSubcriber(long subscriberId)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = " SELECT * FROM EpaperSubcriber WHERE ID = @ID ";
                return conn.Query<EpaperSubscriber>(sql, new { subscriberId }).SingleOrDefault();
            }
        }
        /// <summary>
        /// 設定發送
        /// </summary>
        /// <param name="item"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SetSend(EpaperSend item, out string msg)
        {
            msg = string.Empty;

            if (item.EpaperType_ID == 0 || item.Epaper_ID == 0)
            {
                msg = "處理不成功。遺失電子報ID資訊";
                return false;
            }

            //if (item.ID == 0)
            //    item.ID = WorkLib.GetItem.NewSN();

            item.UserFilter = string.IsNullOrEmpty(item.UserFilterArray) ? FilterPara.AllUsers.ToString() : item.UserFilterArray;
            item.LikeFilter = string.IsNullOrEmpty(item.LikeFilterArray) ? FilterPara.AllLikes.ToString() : item.LikeFilterArray;
            //item.SubscribeFilter = string.IsNullOrEmpty(item.SubscribeFilterArray) ? FilterPara.AllOrders : item.SubscribeFilterArray;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperSend");

            //如果不是暫存，進入發送狀態
            if (!item.IsTemp)
            {
                item.ID = WorkLib.GetItem.NewSN(); //一律新增
                item.Status = SendStatus.Sending;
            }
            else
            {
                if (item.ID == 0)
                    item.ID = WorkLib.GetItem.NewSN();
                item.Status = SendStatus.Temp;
            }

            //如果是馬上發送，將執行發送時間輸入現在時間
            if (item.IsSendNow)
                item.SendTime = DateTime.Now;
            else
                item.SendTime = item.SendTime ?? DateTime.Now; //預約排程這邊應該要有值,如沒有給值,塞現在時間

            item.StatusChangeTime = DateTime.Now; //狀態改變時間
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From EpaperSend Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;

            try
            {
                if (isNew)
                {
                    tableObj["Type"] = item.inputType;
                    tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                    tableObj["CreateTime"] = DateTime.Now;
                    tableObj.Insert();
                }
                else
                {
                    tableObj.Remove("ID");
                    tableObj.Remove("Creator");
                    tableObj.Remove("CreateTime");
                    tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                    tableObj["ModifyTime"] = DateTime.Now;
                    tableObj.Update(item.ID);
                }
                SetSendListIndex(db, item);
            }
            catch (Exception ex)
            {
                msg = ex.ToString().Split('。')[0];
                return false;
            }
            return true;
        }
        /// <summary>
        /// 透過電子報ID 檢查在發送排程中是否有暫存檔
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <returns></returns>
        private static bool CheckTempIsExistInSend(long Epaper_ID)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select 1 from EpaperSend where Epaper_ID={Epaper_ID} and Status='Temp'";
                var result = conn.Query<int>(sql);
                return result.Any();
            }
        }
        /// <summary>
        /// 產生發送清單
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sendData"></param>
        public static void SetSendListIndex(SQLData.Database db, EpaperSend sendData)
        {
            if (sendData.inputType == WorkV3.Areas.Backend.Models.SendType.Filter.ToString())
                SetSendListByFilter(db, sendData);
            else if (sendData.inputType == WorkV3.Areas.Backend.Models.SendType.Manual.ToString())
                SetSendListByManual(db, sendData);
        }
        /// <summary>
        /// 產生發送清單 (by Manual)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sendData"></param>
        public static void SetSendListByManual(SQLData.Database db, EpaperSend sendData)
        {
            long EpaperType_ID = sendData.EpaperType_ID;
            long EpaperSend_ID = sendData.ID;
            long SiteID = sendData.SiteID;

            if (string.IsNullOrWhiteSpace(sendData.manualSendList))
                return;

            string[] array = sendData.manualSendList.Split(';');
            List<string> sendEmailList = new List<string>();
            List<long> subscriber_ids = new List<long>();
            foreach (var emailStr in array)
            {
                if (string.IsNullOrWhiteSpace(emailStr))
                    continue;
                if (!checkEmailFormat(emailStr))
                    continue;

                long sid = CheckEmailInEpaperSubscriber(emailStr.Trim(), SiteID);
                if (sid != 0)
                    subscriber_ids.Add(sid);
                else
                    sendEmailList.Add(emailStr.Trim());
            }

            StringBuilder sb = new StringBuilder();
            foreach (var emailStr in sendEmailList)
            {
                sb.Append($"insert into EpaperSendList (EpaperSend_ID,EpaperSubscriber_ID,TemporarilyEmail) values ({EpaperSend_ID},0,'{emailStr.Replace("'", "''")}') ");
            }
            foreach (var subscriber_id in subscriber_ids)
            {
                sb.Append($"insert into EpaperSendList (EpaperSend_ID,EpaperSubscriber_ID) values ({EpaperSend_ID},{subscriber_id}) ");
            }
            //先清舊資料
            string CleanSql = $"Delete from EpaperSendList where EpaperSend_ID={ EpaperSend_ID }";
            db.ExecuteNonQuery(CleanSql);
            //string test = sb.ToString();
            db.ExecuteNonQuery(sb.ToString());
        }
        /// <summary>
        /// 產生發送清單 (by Filter)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sendData"></param>
        public static void SetSendListByFilter(SQLData.Database db, EpaperSend sendData)
        {
            long EpaperType_ID = sendData.EpaperType_ID;
            long EpaperSend_ID = sendData.ID;
            long SiteID = sendData.SiteID;
            string[] UserFilterArray = sendData.UserFilter.Split(',');
            string[] LikeFilterArray = sendData.LikeFilter.Split(',');

            //身分篩選只有一般訂閱者
            bool IsAllUserOnly = UserFilterArray.Length == 1 && UserFilterArray.Any(m => m == WorkV3.Areas.Backend.Models.FilterPara.AllUsers.ToString());
            //身分篩選包含一般訂閱者(在有選取其他會員身分的情況下) 通常IsAllUserOnly=true時 HaveAllUser應該要false
            bool memberHaveAllUser = UserFilterArray.Length > 1 && UserFilterArray.Any(m => m == WorkV3.Areas.Backend.Models.FilterPara.AllUsers.ToString());
            //取得身分與喜好篩選內容
            string[] users = UserFilterArray.Where(m => m != WorkV3.Areas.Backend.Models.FilterPara.AllUsers.ToString() && m != WorkV3.Areas.Backend.Models.FilterPara.AllMember.ToString()).ToArray();
            string[] likes = LikeFilterArray.Where(m => m != WorkV3.Areas.Backend.Models.FilterPara.AllLikes.ToString()).ToArray();

            #region 在"身份篩選"有選擇會員相關身分情況下、產生身分與喜好的sql查詢條件
            string memberUserSearch = string.Empty;
            string memberLikesSearch = string.Empty;
            if (!IsAllUserOnly)
            {
                if (users.Length > 0)
                    memberUserSearch = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='{1}') ",
                                                        string.Join(",", users),
                                                        WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Identity.ToString());
                if (likes.Length > 0)
                    memberLikesSearch = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='{1}') ",
                                                        string.Join(",", likes),
                                                        WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Favority.ToString());
            }
            #endregion

            #region 在身分篩選有選擇一般訂閱者的情況下 處理喜好篩選查詢
            //TODO: 一般訂閱者喜好尚未實作完全、暫時註解
            string AllUserLikesWhereCodition = string.Empty;
            //if (likes.Length > 0 && (IsAllUserOnly || memberHaveAllUser))
            //    AllUserLikesWhereCodition = string.Format(" and EpaperSubLikesForGeneral.Category_ID in ({0}) ", string.Join(",", likes));
            #endregion

            SQLData.ParameterCollection scrlarVars = new SQLData.ParameterCollection();
            scrlarVars.Add("@SiteID", SiteID);
            scrlarVars.Add("@EpaperType_ID", EpaperType_ID);

            string memberStr = @"
                                select
                                EpaperSubscriber.ID 
                                from
                                (
                                    select 
	                                    EpaperSubscriber.ID
	                                    from EpaperSubscriber 
	                                    left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
	                                    left join MemberShipSetting on MemberShipSetting.MemberShipID = MemberShip.ID
	                                    where EpaperSubscriber.SiteID=@SiteID and EpaperSubscriber.IsPause=0 
                                              and Member_ID is not null " + memberUserSearch + @"
	                                    group by EpaperSubscriber.ID

                                        intersect

                                        select 
	                                    EpaperSubscriber.ID
	                                    from EpaperSubscriber 
	                                    left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
	                                    left join MemberShipSetting on MemberShipSetting.MemberShipID = MemberShip.ID
	                                    where EpaperSubscriber.SiteID=@SiteID and EpaperSubscriber.IsPause=0 
                                        and Member_ID is not null " + memberLikesSearch + @"
	                                    group by EpaperSubscriber.ID

                                ) as EpaperSubscriberFilter
                                inner join EpaperSubscriber on EpaperSubscriberFilter.ID= EpaperSubscriber.ID
                                inner join EpaperSubscriberDetail on EpaperSubscriber.ID=EpaperSubscriberDetail.EpaperSubscriber_ID
                                where EpaperSubscriberDetail.EpaperType_ID=@EpaperType_ID and EpaperSubscriber.IsPause=0
                                group by EpaperSubscriber.ID 
                                ";

            string generalStr = @"
                                select
                                EpaperSubscriber.ID 
                                from (select * from EpaperSubscriber where Member_ID is null) as EpaperSubscriber
                                inner join EpaperSubscriberDetail on EpaperSubscriber.ID=EpaperSubscriberDetail.EpaperSubscriber_ID
                                left join EpaperSubLikesForGeneral on EpaperSubLikesForGeneral.EpaperSubscriber_ID = EpaperSubscriber.ID 
                                where EpaperSubscriberDetail.EpaperType_ID=@EpaperType_ID and EpaperSubscriber.IsPause=0 " + AllUserLikesWhereCodition + @"
                                group by EpaperSubscriber.ID 
                                ";

            List<string> query = new List<string>();
            if (!IsAllUserOnly)
                query.Add($"({memberStr})");
            if (memberHaveAllUser || IsAllUserOnly)
                query.Add($"({generalStr})");

            string GetListSql = string.Join("union", query);

            //產生即將加入發送列表的訂閱者ID
            DataTable list = db.GetDataTable(GetListSql, scrlarVars);

            List<long> subscriber_ids = new List<long>();
            if (list != null)
            {
                foreach (DataRow dr in list.Rows)
                {
                    subscriber_ids.Add((long)dr["ID"]);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var subscriber_id in subscriber_ids)
            {
                sb.Append($"insert into EpaperSendList (EpaperSend_ID,EpaperSubscriber_ID) values ({EpaperSend_ID},{subscriber_id}) ");
            }
            //先清舊資料
            string CleanSql = $"Delete from EpaperSendList where EpaperSend_ID={ EpaperSend_ID }";
            db.ExecuteNonQuery(CleanSql);
            //string test = sb.ToString();
            db.ExecuteNonQuery(sb.ToString());
        }
        /// <summary>
        /// 查詢發送列表筆數
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <param name="EpaperSendID"></param>
        /// <param name="forTemp"></param>
        /// <returns></returns>
        public static int QuerySendListCountByEpaperID(long Epaper_ID, long EpaperSendID, bool forTemp = false)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            string condition = "";
            if (!forTemp)
                condition = $" and EpaperSend.ID={EpaperSendID}";
            else
                condition = " and EpaperSend.Status='Temp'";

            string sql = @"Select Count(*) 
                           From EpaperSendList 
                           inner join EpaperSend on EpaperSend.ID=EpaperSendList.EpaperSend_ID 
                           Where EpaperSend.Epaper_ID =" + Epaper_ID + " {0}";

            sql = string.Format(sql, condition);
            int count = db.GetFirstValue(sql) != null ? Convert.ToInt32(db.GetFirstValue(sql)) : 0;
            return count;
        }
        /// <summary>
        /// 透過EpaperID取得發送資料
        /// </summary>
        /// <param name="Epaper_ID"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSend> GetEpaperSendByEpaperID(long Epaper_ID)
        {
            //TODO: 發送列表分頁
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"select EpaperSend.*, " +
                         $"(select SiteID from Epaper where ID={Epaper_ID}) as EpaperSiteID " +
                         $"from EpaperSend where Epaper_ID={Epaper_ID} and Status <> 'Temp' order by CreateTime desc";
            DataTable dt = db.GetDataTable(sql);
            List<EpaperSend> list = new List<EpaperSend>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new EpaperSend()
                    {
                        ID = (long)dr["ID"],
                        SiteID = (long)dr["EpaperSiteID"],
                        Type = dr["Type"].ToString().Trim(),
                        UserFilter = dr["UserFilter"] != null ? dr["UserFilter"].ToString().Trim() : string.Empty,
                        LikeFilter = dr["LikeFilter"] != null ? dr["LikeFilter"].ToString().Trim() : string.Empty,
                        SubscribeFilter = dr["SubscribeFilter"] != null ? dr["SubscribeFilter"].ToString().Trim() : string.Empty,
                        IsSendNow = (bool)dr["IsSendNow"],
                        SendTime = dr["SendTime"] as DateTime?,
                        SendTimeEnd = dr["SendTimeEnd"] as DateTime?,
                        Status = dr["Status"].ToString().Trim(),
                        StatusChangeTime = dr["StatusChangeTime"] as DateTime?,
                        Epaper_ID = (long)dr["Epaper_ID"],
                        manualSendList = GetTempEmailDataBySendID((long)dr["ID"])
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 透過EpaperSend ID 取得臨時發送Email
        /// </summary>
        /// <param name="EpaperSend_ID"></param>
        /// <returns></returns>
        static string GetTempEmailDataBySendID(long EpaperSend_ID)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select TemporarilyEmail from EpaperSendList where EpaperSend_ID={EpaperSend_ID} and EpaperSubscriber_ID = 0 and TemporarilyEmail is not NULL";
                var result = conn.Query<string>(sql);
                if (result.Count() > 0)
                    return string.Join(";", result);
                else
                    return string.Empty;
            }
        }
        /// <summary>
        /// 產生預估時間字串
        /// </summary>
        /// <param name="StartDT">起始時間</param>
        /// <param name="IntervalsSecond">間隔秒數</param>
        /// <param name="DatasCount">資料筆數</param>
        /// <param name="ShowType">顯示模式: 1顯示起始結束時間 2僅顯示結束時間</param>
        /// <returns></returns>
        public static string GetEstimatedTimeStr(int IntervalsSecond, int DatasCount, int ShowType, DateTime? StartDT = null)
        {
            if (DatasCount == 0)
            {
                return string.Empty;
            }
            if (StartDT == null)
            {
                StartDT = DateTime.Now;
            }
            if (ShowType == 0)
            {
                ShowType = 2;
            }
            string result = string.Empty;
            string start_str = string.Empty;
            string end_str = string.Empty;

            string start_week = System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames[(byte)Convert.ToDateTime(StartDT).DayOfWeek].Substring(2, 1);
            start_str = string.Format("{0} ({1}) {2}", StartDT.ToString("yyyy/MM/dd"), start_week, StartDT.ToString("HH:mm"));

            DateTime end_dt = ((DateTime)StartDT).AddSeconds(IntervalsSecond * DatasCount);
            string end_week = System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames[(byte)end_dt.DayOfWeek].Substring(2, 1);
            end_str = string.Format("{0} ({1}) {2}", end_dt.ToString("yyyy/MM/dd"), end_week, end_dt.ToString("HH:mm"));

            switch (ShowType)
            {
                case 1:
                    result = string.Format("{0} ~ {1}", start_str, end_str);
                    break;
                case 2:
                    result = end_str;
                    break;
            }
            return result;

        }
        /// <summary>
        /// 產生實際時間字串
        /// </summary>
        /// <param name="StartDT">起始時間</param>
        /// <param name="EndDT">結束時間</param>
        /// <param name="ShowType">顯示模式: 1顯示起始結束時間 2僅顯示結束時間</param>
        /// <returns></returns>
        public static string GetRealTimeStr(DateTime StartDT, DateTime EndDT, int ShowType)
        {
            if (StartDT == default(DateTime))
            {
                return string.Empty;
            }
            if (EndDT == default(DateTime))
            {
                return string.Empty;
            }
            string result = string.Empty;
            string start_str = string.Empty;
            string end_str = string.Empty;

            string start_week = System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames[(byte)StartDT.DayOfWeek].Substring(2, 1);
            start_str = string.Format("{0} ({1}) {2}", StartDT.ToString("yyyy/MM/dd"), start_week, StartDT.ToString("HH:mm"));

            string end_week = System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames[(byte)EndDT.DayOfWeek].Substring(2, 1);
            end_str = string.Format("{0} ({1}) {2}", EndDT.ToString("yyyy/MM/dd"), end_week, EndDT.ToString("HH:mm"));

            switch (ShowType)
            {
                case 1:
                    result = string.Format("{0} ~ {1}", start_str, end_str);
                    break;
                case 2:
                    result = end_str;
                    break;
            }
            return result;
        }
        /// <summary>
        /// 根據狀態顯示相對應的預估/實際時間
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="DatasCount"></param>
        /// <param name="ShowType"></param>
        /// <param name="SendTime"></param>
        /// <param name="SendTimeEnd"></param>
        /// <returns></returns>
        public static string GenEstimOrRealTimStr(long SiteID, string Status, int DatasCount, int ShowType, DateTime? SendTime = null, DateTime? SendTimeEnd = null)
        {
            if (string.IsNullOrEmpty(Status))
            {
                return string.Empty;
            }

            EpaperSettingModel Setting = GetEpaperSetting(SiteID);
            int maxTime = Setting.EpaperSendIntervalMax > Setting.EpaperSendIntervalMin ? Setting.EpaperSendIntervalMax : Setting.EpaperSendIntervalMin;

            if (Status == SendStatus.Start || Status == SendStatus.Setting)
            {
                //產生預估時間
                return GetEstimatedTimeStr(maxTime, DatasCount, ShowType, SendTime);
            }
            else if (Status == SendStatus.Sending)
            {
                if (SendTime == null)
                    return string.Empty;
                //產生預估時間
                return GetEstimatedTimeStr(maxTime, DatasCount, ShowType, Convert.ToDateTime(SendTime));
            }
            else if (Status == SendStatus.Pause)
            {
                if (SendTime == null)
                    return string.Empty;
                //產生預估時間
                return GetEstimatedTimeStr(maxTime, DatasCount, ShowType, Convert.ToDateTime(SendTime));
            }
            else if (Status == SendStatus.Sended || Status == SendStatus.ManualBreak || Status == SendStatus.SystemBreak)
            {
                //產生實際時間
                if (SendTime == null || SendTimeEnd == null)
                    return string.Empty;
                else
                    return GetRealTimeStr(Convert.ToDateTime(SendTime), Convert.ToDateTime(SendTimeEnd), ShowType);
            }

            return string.Empty;
        }
        /// <summary>
        /// 取得已發送數量
        /// </summary>
        /// <param name="EpaperSend_ID"></param>
        /// <returns></returns>
        public static int GetRealSendCount(long EpaperSend_ID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = @"select Count(*) from EpaperSendList where IsSended=1 and EpaperSend_ID=" + EpaperSend_ID;
            int count = db.GetFirstValue(sql) != null ? Convert.ToInt32(db.GetFirstValue(sql)) : 0;
            return count;
        }
        /// <summary>
        /// 發送測試
        /// </summary>
        /// <param name="AddrList"></param>
        public static void SendTest(long SiteID, string[] AddrList, string url, string EpaperTitle)
        {
            string ErrorMsg = string.Empty;
            string Subject = string.Format("[測試發送]{0}", EpaperTitle);
            string Body = RenderHtmlContentByUrl(url);
            string Style = string.Empty;
            string MailBodyTemplate = string.Empty;

            try
            {
                SendMail(
                        SiteID,
                        string.Join(",", AddrList),
                        Subject,
                        Body,
                        out ErrorMsg);
            }
            catch (Exception ex)
            {
                EpaperLogDAO.WriteLog(Type: EpaperLogType.Exception, Message: "TestMail");
            }

        }
        /// <summary>
        /// 發送信件
        /// </summary>
        /// <param name="MailTo"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="Style"></param>
        /// <param name="MailBodyTemplate"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        static bool SendMail(long SiteID, string MailTo, string Subject, string Body, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            MailMessage msg = new MailMessage();

            EpaperSettingModel Setting = GetEpaperSetting(SiteID);

            msg.To.Add(MailTo);
            //msg.To.Add(string.Join(",", MailTo.ToArray()));
            msg.From = new MailAddress(Setting.EpaperEmailFrom, Setting.EpaperDisplayName, System.Text.Encoding.UTF8);
            //郵件標題 
            msg.Subject = Subject;
            //郵件標題編碼  
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            //郵件內容
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
            msg.Priority = MailPriority.Normal;//郵件優先級 
            //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
            SmtpClient MySmtp = new SmtpClient(Setting.EpaperSmtpServer, Setting.EpaperEmailPort);
            //設定帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential(Setting.EpaperEmailAcc, Setting.EpaperEmailPwd);
            //啟用 SSL
            MySmtp.EnableSsl = Setting.EpaperEnabledSSL;

            try
            {
                MySmtp.Send(msg);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.ToString();
                return false;
            }

            return true;
        }
        /// <summary>
        /// 透過網址取得網頁html內容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string RenderHtmlContentByUrl(string url)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AllowAllCertifications);
            }
            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
            using (System.IO.Stream responseStream = response.GetResponseStream())
            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(responseStream))
            {
                return streamReader.ReadToEnd();
            }
        }
        /// <summary>
        /// 忽略SSL所有憑證問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certification"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        static bool AllowAllCertifications(object sender,
                                    System.Security.Cryptography.X509Certificates.X509Certificate certification,
                                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        /// <summary>
        /// 變更暫停寄送狀態
        /// </summary>
        /// <param name="id">EpaperSubscriber.ID</param>
        public static void TogglePauseSend(long id)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Update EpaperSubscriber Set IsPause = 1 - IsPause Where ID = " + id;
                conn.Execute(sql);
            }
        }
        /// <summary>
        /// 刪除電子報發送
        /// </summary>
        /// <param name="EpaperSendID"></param>
        /// <returns></returns>
        public static int EpaperSendDel(long EpaperSendID)
        {
            if (EpaperSendID == 0)
                return 0;

            if (CheckIsSendingNow(EpaperSendID))
                return 0;

            string sql = $"Delete From EpaperSend Where ID={EpaperSendID}";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                IEnumerable<long> delEpaperSendListIds = conn.Query<long>($"select EpaperSendList.ID from EpaperSendList inner join EpaperSend on EpaperSend.ID=EpaperSendList.EpaperSend_ID where EpaperSend.ID= {EpaperSendID}");
                if (delEpaperSendListIds.Count() != 0)
                    sql += $" Delete From EpaperSendList Where ID in ({string.Join(",", delEpaperSendListIds)})";
                num = conn.Execute(sql);
            }

            return num;
        }
        /// <summary>
        /// 檢查發送時間是否已經超過目前時間
        /// </summary>
        /// <param name="EpaperSendID"></param>
        /// <returns></returns>
        static bool CheckIsSendingNow(long EpaperSendID)
        {
            string sql = $"select SendTime from EpaperSend where ID={EpaperSendID}";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                var result = conn.Query<DateTime?>(sql);
                if (result.Any())
                {
                    var sendtime = result.ElementAt(0);
                    if (sendtime <= DateTime.Now)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 變更發送狀態
        /// </summary>
        /// <param name="EpaperSendID"></param>
        /// <param name="action"></param>
        public static void ChangeEpaperSendStatus(long EpaperSendID, string action)
        {
            if (EpaperSendID == 0)
                return;
            if (string.IsNullOrWhiteSpace(action))
                return;
            System.Reflection.FieldInfo[] fields = typeof(SendStatus).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (!fields.Any(m => m.Name == action))
                return;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = string.Format("Update EpaperSend Set Status = '" + action + "',StatusChangeTime=getdate() {0} Where ID = " + EpaperSendID,
                                            action == SendStatus.ManualBreak ? " ,SendTimeEnd=getdate()" : string.Empty);
                conn.Execute(sql);
            }
        }
        /// <summary>
        /// 電子報發送取得發送名單
        /// </summary>
        /// <param name="EpaperSendID"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSubscriber> GetSendPeopleData(long EpaperSendID)
        {
            IEnumerable<long> SubscriberIds = QuerySubscribersByEpaperSendID(EpaperSendID);
            IEnumerable<EpaperSendList> tempdatas = GetTemporarilySendList(EpaperSendID,false);
            if (SubscriberIds.Count() == 0 && tempdatas.Count() == 0)
                return new List<EpaperSubscriber>();

            List<EpaperSubscriber> sub = GetSubscribersList(SubscriberIds);
            List<EpaperSubscriber> tmp = TempDatasToEpaperSubscriber(tempdatas);
            return sub.Concat(tmp).ToList();
        }
        /// <summary>
        /// 手動輸入email/臨時發送email資訊 以EpaperSubscriber List包裝
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        static List<EpaperSubscriber> TempDatasToEpaperSubscriber(IEnumerable<EpaperSendList> datas)
        {
            List<EpaperSubscriber> list = new List<EpaperSubscriber>();
            foreach (var item in datas)
            {
                list.Add(new EpaperSubscriber
                {
                    ID = item.EpaperSubscriber_ID,
                    Email = item.TemporarilyEmail,
                    Likes = "",
                    Detail = new List<EpaperSubscriberDetail>(),
                    MemberShip = new WorkV3.Models.MemberShipModels()
                });
            }
            return list;
        }
        /// <summary>
        /// 取得所有臨時發送Email的資料
        /// </summary>
        /// <param name="EpaperSendID"></param>
        /// <returns></returns>
        static IEnumerable<EpaperSendList> GetTemporarilySendList(long EpaperSendID,bool sent)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string querySubscribers;
                if(sent)
                 querySubscribers = $"select EpaperSendList.* from EpaperSendList inner join EpaperSend on EpaperSend.ID=EpaperSendList.EpaperSend_ID where EpaperSend.ID={EpaperSendID} and EpaperSendList.EpaperSubscriber_ID = 0 and EpaperSendList.IsSended = 'true'";
                else
                 querySubscribers = $"select EpaperSendList.* from EpaperSendList inner join EpaperSend on EpaperSend.ID=EpaperSendList.EpaperSend_ID where EpaperSend.ID={EpaperSendID} and EpaperSendList.EpaperSubscriber_ID = 0";
                return conn.Query<EpaperSendList>(querySubscribers);
            }
        }
        /// <summary>
        /// 透過發送排程EpaperSend.ID取得發送名單(EpaperSendList)
        /// </summary>
        /// <param name="search"></param>
        /// <param name="EpaperSend_ID"></param>
        /// <param name="IsExportExcel"></param>
        /// <param name="siteId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<EpaperSendList> GetSendListBySendID(EpaperSendRecordSearchModel search, long EpaperSend_ID, bool IsExportExcel, long siteId, int pageSize, int pageIndex, out int recordCount,bool sent)
        {
            List<EpaperSendList> SendList = new List<EpaperSendList>();
            recordCount = 0;

            if (search == null)
                return SendList;

            IEnumerable<EpaperSendList> tempEmaildatas = GetTemporarilySendList(EpaperSend_ID,sent);

            StringBuilder sb = new StringBuilder();
            //關鍵字
            if (!string.IsNullOrEmpty(search.keyword))
            {
                //查詢Email、會員帳號、姓名、手機、電話、身分證字號等關鍵字
                string condition = " and ("
                                 + " EpaperSubscriber.Email like N'%{0}%'"
                                 + " or MemberShip.Account like N'%{0}%'"
                                 + " or MemberShip.Phone like N'%{0}%'"
                                 + " or MemberShip.Mobile like N'%{0}%'"
                                 + " or MemberShip.IDCard like N'%{0}%'"
                                 + " or MemberShip.Name like N'%{0}%'"
                                 + ")";
                sb.Append(string.Format(condition, search.keyword.Trim().Replace("'", "''")));
                tempEmaildatas = tempEmaildatas.Where(m => m.TemporarilyEmail.Contains(search.keyword));
            }

            //發送狀況
            if (search.SendResult != null)
            {
                List<string> temp = new List<string>();
                foreach (var item in search.SendResult)
                {
                    if (item == WorkV3.Areas.Backend.Models.SendResult.Notyet.ToString())
                        temp.Add(" EpaperSendList.IsSended=0 and EpaperSendList.SendTime is null ");
                    else if (item == WorkV3.Areas.Backend.Models.SendResult.Successful.ToString())
                        temp.Add(" EpaperSendList.IsSended=1 and EpaperSendList.SendTime is not null ");
                    else if (item == WorkV3.Areas.Backend.Models.SendResult.Readed.ToString())
                        temp.Add(" EpaperSendList.IsRead=1 and EpaperSendList.ReadTime is not null ");
                    else if (item == WorkV3.Areas.Backend.Models.SendResult.Fail.ToString())
                        temp.Add(" EpaperSendList.IsSended=0 and EpaperSendList.SendTime is not null ");
                }
                sb.Append("and (" + string.Join("or", temp) + ")");
            }

            #region 在"身份篩選"有選擇會員相關身分情況下、產生身分與喜好的sql查詢條件
            string memberIdSelectCon = string.Empty;
            string memberFavSelectCon = string.Empty;
            if (search.SubscriberType != OpenObject.General.ToString())
            {
                //會員身分
                if (search.identitySelect != null)
                {
                    if (search.identitySelect.Count() > 0)
                    {
                        memberIdSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Identity')",
                                                            string.Join(",", search.identitySelect));
                    }
                }

                //喜好
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        memberFavSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Favority')",
                                                            string.Join(",", search.favoritySelect));
                    }
                }
            }
            #endregion

            #region 在身分篩選有選擇一般訂閱者的情況下 處理喜好篩選查詢
            string generalFavSelectConStr = string.Empty;
            if (search.SubscriberType != OpenObject.Member.ToString())
            {
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        //TODO: 一般訂閱者喜好尚未實作完全、暫時註解
                        //generalFavSelectConStr = string.Format(" and EpaperSubLikesForGeneral.Category_ID in ({0}) ", string.Join(",", search.favoritySelect));
                    }
                }
            }

            #endregion

            //發送時間
            if (search.SendTimeStart != null && search.SendTimeEnd != null)
            {
                if (search.SendTimeStart < search.SendTimeEnd)
                {
                    var tempStart = search.SendTimeStart;
                    search.SendTimeStart = search.SendTimeEnd;
                    search.SendTimeEnd = tempStart;
                }
                sb.Append($" and (EpaperSendList.SendTime >= {search.SendTimeStart} and EpaperSendList.SendTime <= {search.SendTimeEnd})");
            }
            //調閱報別
            string EpaperTypStr = string.Empty;
            //bool IsHaveEpaperTypeSelect = false;
            if (search.EpaperTypeSelect != null)
            {
                if (search.EpaperTypeSelect.Length > 0)
                {
                    EpaperTypStr = $" and EpaperType_ID In ({string.Join(",", search.EpaperTypeSelect)})";
                    //sHaveEpaperTypeSelect = true;
                }
            }

            string memberStr = @"
                                select
                                EpaperSendList.*
                                from (select * from EpaperSendList where EpaperSendList.EpaperSend_ID = " + EpaperSend_ID + @") as EpaperSendList
                                inner join EpaperSubscriber on EpaperSubscriber.ID = EpaperSendList.EpaperSubscriber_ID
                                inner join
		                                (
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberIdSelectCon + @"
		                                group by EpaperSubscriber.ID 

		                                intersect
			
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberFavSelectCon + @"
		                                group by EpaperSubscriber.ID
		                                ) as EpaperSubscriberFilter on EpaperSubscriberFilter.ID = EpaperSubscriber.ID
                                inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1 {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                                where 1=1 {0}
                                ";

            string generalStr = @"
                                select
                                EpaperSendList.*
                                from (select * from EpaperSendList where EpaperSendList.EpaperSend_ID = " + EpaperSend_ID + @") as EpaperSendList
                                inner join (select * from EpaperSubscriber where Member_ID is null) as EpaperSubscriber on EpaperSubscriber.ID = EpaperSendList.EpaperSubscriber_ID
                                inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1  {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                                left join EpaperSubLikesForGeneral on EpaperSubLikesForGeneral.EpaperSubscriber_ID = EpaperSubscriber.ID
                                where 1=1 " + generalFavSelectConStr + @" {0}
                                ";


            List<string> query = new List<string>();
            if (search.SubscriberType == OpenObject.Member.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({memberStr})", sb.ToString(), EpaperTypStr));
            if (search.SubscriberType == OpenObject.General.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({generalStr})", sb.ToString(), EpaperTypStr));

            string sql = string.Join("union", query);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas;

            if (!IsExportExcel)
                datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            else
                datas = db.GetDataTable(sql);

            if (datas == null)
                return SendList;

            IEnumerable<EpaperSubscriber> SubscriberDatas = GetSendPeopleData(EpaperSend_ID);
            foreach (DataRow dr in datas.Rows)
            {
                SendList.Add(new EpaperSendList
                {
                    ID = (long)dr["ID"],
                    EpaperSend_ID = (long)dr["EpaperSend_ID"],
                    EpaperSubscriber_ID = (long)dr["EpaperSubscriber_ID"],
                    IsSended = (bool)dr["IsSended"],
                    SendTime = dr["SendTime"] as DateTime?,
                    IsRead = (bool)dr["IsRead"],
                    ReadTime = dr["ReadTime"] as DateTime?,
                    EpaperSubscriber = SubscriberDatas.Where(m => m.ID == (long)dr["EpaperSubscriber_ID"]).SingleOrDefault()
                });
            }

            //調整"臨時發送Email"的資料內容, 使適應ViewModel
            foreach (var item in tempEmaildatas)
            {
                item.EpaperSubscriber = new EpaperSubscriber()
                {
                    Email = item.TemporarilyEmail,
                    MemberShip = new WorkV3.Models.MemberShipModels(),
                    Detail = new List<EpaperSubscriberDetail>()
                };
            }


            return SendList.Concat(tempEmaildatas).ToList();

        }
        #endregion

        #region 訂閱者與黑名單
        /// <summary>
        /// 取得訂閱者名單 (訂閱者名單管理)
        /// </summary>
        /// <param name="search"></param>
        /// <param name="IsExportExcel"></param>
        /// <param name="siteId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<EpaperSubscriber> GetSubscriberList(EpaperSubscriberSearchModel search, bool IsExportExcel, long siteId, int pageSize, int pageIndex, out int recordCount)
        {
            List<EpaperSubscriber> list = new List<EpaperSubscriber>();
            recordCount = 0;

            if (search == null)
                return list;

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(search.keyword))
            {
                //查詢Email、會員帳號、姓名、手機、電話、身分證字號等關鍵字
                string condition = " and ("
                                 + " EpaperSubscriber.Email like N'%{0}%'"
                                 + " or MemberShip.Account like N'%{0}%'"
                                 + " or MemberShip.Phone like N'%{0}%'"
                                 + " or MemberShip.Mobile like N'%{0}%'"
                                 + " or MemberShip.IDCard like N'%{0}%'"
                                 + " or MemberShip.Name like N'%{0}%'"
                                 + ")";
                sb.Append(string.Format(condition, search.keyword.Trim().Replace("'", "''")));
            }
            //if (!string.IsNullOrWhiteSpace(search.SubscriberType))
            //{
            //    if (search.SubscriberType == OpenObject.Member.ToString())
            //    {
            //        sb.Append(" and EpaperSubscriber.Member_ID is not null");
            //    }
            //}

            #region 在"身份篩選"有選擇會員相關身分情況下、產生身分與喜好的sql查詢條件
            string memberIdSelectCon = string.Empty;
            string memberFavSelectCon = string.Empty;
            if (search.SubscriberType != OpenObject.General.ToString())
            {
                //會員身分
                if (search.identitySelect != null)
                {
                    if (search.identitySelect.Count() > 0)
                    {
                        memberIdSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Identity')",
                                                            string.Join(",", search.identitySelect));
                    }
                }

                //喜好
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        memberFavSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Favority')",
                                                            string.Join(",", search.favoritySelect));
                    }
                }
            }
            #endregion

            #region 在身分篩選有選擇一般訂閱者的情況下 處理喜好篩選查詢
            string generalFavSelectConStr = string.Empty;
            if (search.SubscriberType != OpenObject.Member.ToString())
            {
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        //TODO: 一般訂閱者喜好尚未實作完全、暫時註解
                        //generalFavSelectConStr = string.Format(" and EpaperSubLikesForGeneral.Category_ID in ({0}) ", string.Join(",", search.favoritySelect));
                    }
                }
            }
            #endregion

            if (search.CreateTime_Start != null && search.CreateTime_End != null)
            {
                DateTime Start = (DateTime)search.CreateTime_Start;
                DateTime End = (DateTime)search.CreateTime_End;
                if (DateTime.Compare(Start, End) > 0)
                {
                    search.CreateTime_Start = End;
                    search.CreateTime_End = Start;
                }

                sb.Append($" and (EpaperSubscriber.CreateTime>='{search.CreateTime_Start.ToString("yyyy/MM/dd HH:mm:ss")}' and EpaperSubscriber.CreateTime<='{search.CreateTime_End.ToString("yyyy/MM/dd HH:mm:ss")}')");
            }
            string EpaperTypStr = string.Empty;
            //bool IsHaveEpaperTypeSelect = false;
            if (search.EpaperTypeSelect != null)
            {
                if (search.EpaperTypeSelect.Length > 0)
                {
                    EpaperTypStr = $" and EpaperType_ID In ({string.Join(",", search.EpaperTypeSelect)})";
                    //IsHaveEpaperTypeSelect = true;
                }
            }

            string memberStr = @"
                                select 
                                EpaperSubscriber.*,
                                MemberShip.*,
                                EpaperSubscriberDetail.EpaperSubscriber_ID
                                from (select * from EpaperSubscriber where Member_ID is not null) as EpaperSubscriber
                                inner join
		                                (
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberIdSelectCon + @"
		                                group by EpaperSubscriber.ID 

		                                intersect
			
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberFavSelectCon + @"
		                                group by EpaperSubscriber.ID
		                                ) as EpaperSubscriberFilter on EpaperSubscriberFilter.ID = EpaperSubscriber.ID
                                    inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1 {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                                where EpaperSubscriber.SiteID =" + siteId + @" and EpaperSubscriber.IsBlack=0 {0}
                                   ";


            string generalStr = @"
                                select 
                                EpaperSubscriber.*,
                                MemberShip.*,
                                EpaperSubscriberDetail.EpaperSubscriber_ID
                                from (select * from EpaperSubscriber where Member_ID is null) as EpaperSubscriber
                                inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1 {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                                left join EpaperSubLikesForGeneral on EpaperSubLikesForGeneral.EpaperSubscriber_ID = EpaperSubscriber.ID
                                where EpaperSubscriber.SiteID =" + siteId + @" and EpaperSubscriber.IsBlack=0 " + generalFavSelectConStr + @" {0}
                                   ";

            List<string> query = new List<string>();
            if (search.SubscriberType == OpenObject.Member.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({memberStr})", sb.ToString(), EpaperTypStr));
            if (search.SubscriberType == OpenObject.General.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({generalStr})", sb.ToString(), EpaperTypStr));

            string sql = string.Join("union", query);
            sql += " order by EpaperSubscriber.CreateTime desc, EpaperSubscriber.ModifyTime desc";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas;
            DataTable datas2;
            if (!IsExportExcel)
                datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            else
                datas = db.GetDataTable(sql);

            if (datas == null)
                return list;
            foreach (DataRow dr in datas.Rows)
            {
               //wei 20180717 顯示喜好 身分
                EpaperSubscriber tmp = new EpaperSubscriber { Member_ID = dr["Member_ID"] == null ? 0 : dr["Member_ID"] as long? };
                if (tmp.Member_ID != null)
                {
                    string Str = @"declare  @favority varchar(8000)=''
                               declare  @identity varchar(8000)=''
                               select @favority=@favority +Categories.Title+' '  from MemberShipSetting inner join MemberShip on MemberShip.ID = MemberShipSetting.MemberShipID
                               inner JOIN Categories on Categories.ID =MemberShipSetting.CategoryID where MemberShip.ID =" + tmp.Member_ID + @" and MemberShipSetting.Type= 'favority'
                               select @identity=@identity +Categories.Title+' '  from MemberShipSetting inner join MemberShip on MemberShip.ID = MemberShipSetting.MemberShipID
                               inner JOIN Categories on Categories.ID =MemberShipSetting.CategoryID where MemberShip.ID =" + tmp.Member_ID + @" and MemberShipSetting.Type= 'identity'
                               select @favority AS 'favority' ,@identity AS 'identity'";
                    datas2 = db.GetDataTable(Str);
                    foreach (DataRow dr2 in datas2.Rows)
                        list.Add(new EpaperSubscriber
                        {
                            ID = (long)dr["ID"],
                            Member_ID = dr["Member_ID"] as long?,
                            Email = dr["Email"].ToString().Trim(),
                            Likes = dr["Likes"] == null ? "" : dr["Likes"].ToString().Trim(),
                            FailureTimes = dr["FailureTimes"] as int?,
                            IsBlack = (bool)dr["IsBlack"],
                            IsPause = (bool)dr["IsPause"],
                            CreateTime = (DateTime)dr["CreateTime"],
                            Detail = GetSubscriberDetail((long)dr["ID"]),
                            identity = dr2["identity"] == null ? "" : dr2["identity"].ToString().Trim(),
                            favority = dr2["favority"] == null ? "" : dr2["favority"].ToString().Trim()
                        });
                }
                else
                {
                    
                        list.Add(new EpaperSubscriber
                        {
                            ID = (long)dr["ID"],
                            Member_ID = dr["Member_ID"] as long?,
                            Email = dr["Email"].ToString().Trim(),
                            Likes = dr["Likes"] == null ? "" : dr["Likes"].ToString().Trim(),
                            FailureTimes = dr["FailureTimes"] as int?,
                            IsBlack = (bool)dr["IsBlack"],
                            IsPause = (bool)dr["IsPause"],
                            CreateTime = (DateTime)dr["CreateTime"],
                            Detail = GetSubscriberDetail((long)dr["ID"]),
                            identity = "",
                            favority = ""
                        });
                }
            }
            return list;
        }
        /// <summary>
        /// 取得訂閱者名單 (訂閱者名單管理)
        /// </summary>
        /// <param name="search"></param>
        /// <param name="IsExportExcel"></param>
        /// <param name="siteId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<string> GetItemsAll(EpaperSubscriberSearchModel search, long siteId)
        {
            List<string> list = new List<string>();

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(search.keyword))
            {
                //查詢Email、會員帳號、姓名、手機、電話、身分證字號等關鍵字
                string condition = " and ("
                                 + " EpaperSubscriber.Email like N'%{0}%'"
                                 + " or MemberShip.Account like N'%{0}%'"
                                 + " or MemberShip.Phone like N'%{0}%'"
                                 + " or MemberShip.Mobile like N'%{0}%'"
                                 + " or MemberShip.IDCard like N'%{0}%'"
                                 + " or MemberShip.Name like N'%{0}%'"
                                 + ")";
                sb.Append(string.Format(condition, search.keyword.Trim().Replace("'", "''")));
            }
            #region 在"身份篩選"有選擇會員相關身分情況下、產生身分與喜好的sql查詢條件
            string memberIdSelectCon = string.Empty;
            string memberFavSelectCon = string.Empty;
            if (search.SubscriberType != OpenObject.General.ToString())
            {
                //會員身分
                if (search.identitySelect != null)
                {
                    if (search.identitySelect.Count() > 0)
                    {
                        memberIdSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Identity')",
                                                            string.Join(",", search.identitySelect));
                    }
                }

                //喜好
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        memberFavSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Favority')",
                                                            string.Join(",", search.favoritySelect));
                    }
                }
            }
            #endregion

            #region 在身分篩選有選擇一般訂閱者的情況下 處理喜好篩選查詢
            string generalFavSelectConStr = string.Empty;
            if (search.SubscriberType != OpenObject.Member.ToString())
            {
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        //TODO: 一般訂閱者喜好尚未實作完全、暫時註解
                        //generalFavSelectConStr = string.Format(" and EpaperSubLikesForGeneral.Category_ID in ({0}) ", string.Join(",", search.favoritySelect));
                    }
                }
            }
            #endregion

            if (search.CreateTime_Start != null && search.CreateTime_End != null)
            {
                DateTime Start = (DateTime)search.CreateTime_Start;
                DateTime End = (DateTime)search.CreateTime_End;
                if (DateTime.Compare(Start, End) > 0)
                {
                    search.CreateTime_Start = End;
                    search.CreateTime_End = Start;
                }

                sb.Append($" and (EpaperSubscriber.CreateTime>='{search.CreateTime_Start.ToString("yyyy/MM/dd HH:mm:ss")}' and EpaperSubscriber.CreateTime<='{search.CreateTime_End.ToString("yyyy/MM/dd HH:mm:ss")}')");
            }
            string EpaperTypStr = string.Empty;
            if (search.EpaperTypeSelect != null)
            {
                if (search.EpaperTypeSelect.Length > 0)
                {
                    EpaperTypStr = $" and EpaperType_ID In ({string.Join(",", search.EpaperTypeSelect)})";
                }
            }

            string memberStr = @"
                                select 
                                EpaperSubscriber.*,
                                MemberShip.*,
                                EpaperSubscriberDetail.EpaperSubscriber_ID
                                from (select * from EpaperSubscriber where Member_ID is not null) as EpaperSubscriber
                                inner join
		                                (
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberIdSelectCon + @"
		                                group by EpaperSubscriber.ID 

		                                intersect
			
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberFavSelectCon + @"
		                                group by EpaperSubscriber.ID
		                                ) as EpaperSubscriberFilter on EpaperSubscriberFilter.ID = EpaperSubscriber.ID
                                    inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1 {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                                where EpaperSubscriber.SiteID =" + siteId + @" and EpaperSubscriber.IsBlack=0 {0}
                                   ";


            string generalStr = @"
                                select 
                                EpaperSubscriber.*,
                                MemberShip.*,
                                EpaperSubscriberDetail.EpaperSubscriber_ID
                                from (select * from EpaperSubscriber where Member_ID is null) as EpaperSubscriber
                                inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1 {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                                left join EpaperSubLikesForGeneral on EpaperSubLikesForGeneral.EpaperSubscriber_ID = EpaperSubscriber.ID
                                where EpaperSubscriber.SiteID =" + siteId + @" and EpaperSubscriber.IsBlack=0 " + generalFavSelectConStr + @" {0}
                                   ";

            List<string> query = new List<string>();
            if (search.SubscriberType == OpenObject.Member.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({memberStr})", sb.ToString(), EpaperTypStr));
            if (search.SubscriberType == OpenObject.General.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({generalStr})", sb.ToString(), EpaperTypStr));

            string sql = string.Join("union", query);
            sql += " order by EpaperSubscriber.CreateTime desc, EpaperSubscriber.ModifyTime desc";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas;
                datas = db.GetDataTable(sql);

            if (datas == null)
                return list;
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    list.Add(dr["ID"].ToString());
                }
            }
            return list;
        }
        /// <summary>
        /// 透過 EpaperSubscriber.ID 取得訂閱清單
        /// </summary>
        /// <param name="EpaperSubscriber_ID"></param>
        /// <returns></returns>
        public static List<EpaperSubscriberDetail> GetSubscriberDetail(long EpaperSubscriber_ID)
        {
            List<EpaperSubscriberDetail> list = new List<EpaperSubscriberDetail>();
            string sql = @"
                        select 
                        EpaperSubscriberDetail.*,
						EpaperType.[Name] EpaperTypeName
                        from EpaperSubscriberDetail
                        inner join EpaperType on EpaperSubscriberDetail.EpaperType_ID=EpaperType.ID
                        where EpaperSubscriberDetail.EpaperSubscriber_ID=" + EpaperSubscriber_ID + @"
                        order by EpaperSubscriberDetail.CreateTime desc
                            ";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new EpaperSubscriberDetail
                {
                    ID = (long)dr["ID"],
                    EpaperSubscriber_ID = (long)dr["EpaperSubscriber_ID"],
                    EpaperType_ID = (long)dr["EpaperType_ID"],
                    CreateTime = (DateTime)dr["CreateTime"],
                    EpaperTypeName = dr["EpaperTypeName"].ToString().Trim()
                });
            }
            return list;
        }
        /// <summary>
        /// 取得黑名單列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<EpaperSubscriber> GetEpaperBlackList(EpaperBlackListSearchModel search, bool IsHiddenInfo, int pageSize, int pageIndex, long siteId, out int recordCount)
        {
            List<EpaperSubscriber> list = new List<EpaperSubscriber>();
            recordCount = 0;

            if (search == null)
                return list;

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(search.keyword))
            {
                //查詢Email、會員帳號、姓名、手機、電話、身分證字號等關鍵字
                string condition = " and ("
                                 + " EpaperSubscriber.Email like N'%{0}%'"
                                 + " or MemberShip.Account like N'%{0}%'"
                                 + " or MemberShip.Phone like N'%{0}%'"
                                 + " or MemberShip.Mobile like N'%{0}%'"
                                 + " or MemberShip.IDCard like N'%{0}%'"
                                 + " or MemberShip.Name like N'%{0}%'"
                                 + ")";
                sb.Append(string.Format(condition, search.keyword.Trim().Replace("'", "''")));
            }

            //if (!string.IsNullOrWhiteSpace(search.SubscriberType))
            //{
            //    if (search.SubscriberType == OpenObject.Member.ToString())
            //    {
            //        sb.Append(" and EpaperSubscriber.Member_ID is not null");
            //    }
            //}

            #region 在"身份篩選"有選擇會員相關身分情況下、產生身分與喜好的sql查詢條件
            string memberIdSelectCon = string.Empty;
            string memberFavSelectCon = string.Empty;
            if (search.SubscriberType != OpenObject.General.ToString())
            {
                //會員身分
                if (search.identitySelect != null)
                {
                    if (search.identitySelect.Count() > 0)
                    {
                        memberIdSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Identity')",
                                                            string.Join(",", search.identitySelect));
                    }
                }

                //喜好
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        memberFavSelectCon = string.Format(" and (MemberShipSetting.CategoryID in ({0}) and MemberShipSetting.[Type]='Favority')",
                                                            string.Join(",", search.favoritySelect));
                    }
                }
            }
            #endregion

            #region 在身分篩選有選擇一般訂閱者的情況下 處理喜好篩選查詢
            string generalFavSelectConStr = string.Empty;
            if (search.SubscriberType != OpenObject.Member.ToString())
            {
                if (search.favoritySelect != null)
                {
                    if (search.favoritySelect.Count() > 0)
                    {
                        //TODO: 一般訂閱者喜好尚未實作完全、暫時註解
                        //generalFavSelectConStr = string.Format(" and EpaperSubLikesForGeneral.Category_ID in ({0}) ", string.Join(",", search.favoritySelect));
                    }
                }
            }
            #endregion

            if (search.FailureTimesStart != null && search.FailureTimesEnd != null)
            {
                if (search.FailureTimesStart < search.FailureTimesEnd)
                {
                    var tempStart = search.FailureTimesStart;
                    search.FailureTimesStart = search.FailureTimesEnd;
                    search.FailureTimesEnd = tempStart;
                }
                sb.Append($" and (EpaperSubscriber.FailureTimes >= {search.FailureTimesStart} and EpaperSubscriber.FailureTimes <= {search.FailureTimesEnd})");
            }
            string EpaperTypStr = string.Empty;
            //bool IsHaveEpaperTypeSelect = false;
            if (search.EpaperTypeSelect != null)
            {
                if (search.EpaperTypeSelect.Length > 0)
                {
                    EpaperTypStr = $" and EpaperType_ID In ({string.Join(",", search.EpaperTypeSelect)})";
                    //IsHaveEpaperTypeSelect = true;
                }
            }

            string memberStr = @"
                                select 
                                EpaperSubscriber.*,
                                MemberShip.*,
                                EpaperSubscriberDetail.EpaperSubscriber_ID
                                from (select * from EpaperSubscriber where Member_ID is not null) as EpaperSubscriber
                                inner join
		                                (
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberIdSelectCon + @"
		                                group by EpaperSubscriber.ID 

		                                intersect
			
		                                select EpaperSubscriber.ID 
                                        from EpaperSubscriber
                                        left join MemberShip on MemberShip.ID=EpaperSubscriber.Member_ID
                                        left join MemberShipSetting on MemberShipSetting.MemberShipID=MemberShip.ID
		                                where Member_ID is not null " + memberFavSelectCon + @"
		                                group by EpaperSubscriber.ID
		                                ) as EpaperSubscriberFilter on EpaperSubscriberFilter.ID = EpaperSubscriber.ID
                                inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1 {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID

                                where EpaperSubscriber.IsBlack = 1 and EpaperSubscriber.SiteID=" + siteId + @" {0}
                                ";

            string generalStr = @"
                                select 
                                EpaperSubscriber.*,
                                MemberShip.*,
                                EpaperSubscriberDetail.EpaperSubscriber_ID
                                from (select * from EpaperSubscriber where Member_ID is null) as EpaperSubscriber
                                inner join 
                                    (select EpaperSubscriber_ID from EpaperSubscriberDetail 
                                    where 1=1 {1}
                                    group by EpaperSubscriber_ID) as EpaperSubscriberDetail on EpaperSubscriberDetail.EpaperSubscriber_ID=EpaperSubscriber.ID
                                left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                                left join EpaperSubLikesForGeneral on EpaperSubLikesForGeneral.EpaperSubscriber_ID = EpaperSubscriber.ID
                                where EpaperSubscriber.SiteID =" + siteId + @" and EpaperSubscriber.IsBlack=1 " + generalFavSelectConStr + @" {0}
                                   ";

            List<string> query = new List<string>();
            if (search.SubscriberType == OpenObject.Member.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({memberStr})", sb.ToString(), EpaperTypStr));
            if (search.SubscriberType == OpenObject.General.ToString() || string.IsNullOrWhiteSpace(search.SubscriberType))
                query.Add(string.Format($"({generalStr})", sb.ToString(), EpaperTypStr));

            string sql = string.Join("union", query);
            sql += " order by EpaperSubscriber.CreateTime desc, EpaperSubscriber.ModifyTime desc";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas;
            DataTable datas2;
            if (!IsHiddenInfo)
                datas = db.GetPageData(sql, pageSize, pageIndex, out recordCount);
            else
                datas = db.GetDataTable(sql);
           
            if (datas == null)
                return list;
            foreach (DataRow dr in datas.Rows)
            {
                //wei 20180717 顯示喜好 身分
                EpaperSubscriber tmp = new EpaperSubscriber { Member_ID = dr["Member_ID"] == null ? 0 : dr["Member_ID"] as long? };
                if (tmp.Member_ID != null)
                {
                    string Str = @"declare  @favority varchar(8000)=''
                               declare  @identity varchar(8000)=''
                               select @favority=@favority +Categories.Title+' '  from MemberShipSetting inner join MemberShip on MemberShip.ID = MemberShipSetting.MemberShipID
                               inner JOIN Categories on Categories.ID =MemberShipSetting.CategoryID where MemberShip.ID =" + tmp.Member_ID + @" and MemberShipSetting.Type= 'favority'
                               select @identity=@identity +Categories.Title+' '  from MemberShipSetting inner join MemberShip on MemberShip.ID = MemberShipSetting.MemberShipID
                               inner JOIN Categories on Categories.ID =MemberShipSetting.CategoryID where MemberShip.ID =" + tmp.Member_ID + @" and MemberShipSetting.Type= 'identity'
                               select @favority AS 'favority' ,@identity AS 'identity'";
                    datas2 = db.GetDataTable(Str);
                    foreach (DataRow dr2 in datas2.Rows)
                        list.Add(new EpaperSubscriber
                        {
                            ID = (long)dr["ID"],
                            Member_ID = dr["Member_ID"] as long?,
                            Email = dr["Email"].ToString().Trim(),
                            Likes = dr["Likes"] == null ? "" : dr["Likes"].ToString().Trim(),
                            FailureTimes = dr["FailureTimes"] as int?,
                            IsBlack = (bool)dr["IsBlack"],
                            IsPause = (bool)dr["IsPause"],
                            CreateTime = (DateTime)dr["CreateTime"],
                            Detail = GetSubscriberDetail((long)dr["ID"]),
                            identity = dr2["identity"] == null ? "" : dr2["identity"].ToString().Trim(),
                            favority = dr2["favority"] == null ? "" : dr2["favority"].ToString().Trim()
                        });
                }
                else
                {

                    list.Add(new EpaperSubscriber
                    {
                        ID = (long)dr["ID"],
                        Member_ID = dr["Member_ID"] as long?,
                        Email = dr["Email"].ToString().Trim(),
                        Likes = dr["Likes"] == null ? "" : dr["Likes"].ToString().Trim(),
                        FailureTimes = dr["FailureTimes"] as int?,
                        IsBlack = (bool)dr["IsBlack"],
                        IsPause = (bool)dr["IsPause"],
                        CreateTime = (DateTime)dr["CreateTime"],
                        Detail = GetSubscriberDetail((long)dr["ID"]),
                        identity = "",
                        favority = ""
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 取得後台Member Email位址
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetMemberAllAddress()
        {
            string sql = $"select Email from Member";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<string>(sql);
            }
        }
        /// <summary>
        /// 取得訂閱者所有Email
        /// </summary>
        /// <param name="excludePause">是否排除"暫停訂閱"的部分</param>
        /// <returns></returns>
        public static IEnumerable<string> GetSubscriberAllAddress(bool excludePause = true)
        {
            string sql = $"select Email from EpaperSubscriber where 1=1 {(excludePause ? "and IsPause=0" : "")}";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<string>(sql);
            }
        }
        /// <summary>
        /// 將特定訂閱者加入黑名單
        /// </summary>
        /// <param name="data">黑名單理由資訊</param>
        /// <param name="BlackIds">訂閱者IDs</param>
        public static void UpdateBlackReasonSubscribers(EpaperSubscriber data, IEnumerable<long> BlackIds)
        {
            if (data == null || BlackIds.Count() == 0)
                return;

            //string sql = "update EpaperSubscriber set IsBlack=1";

            EpaperBlackListInfo black = new EpaperBlackListInfo();
            if (!string.IsNullOrWhiteSpace(data.BlackReason))
                black.BlackReason = data.BlackReason;
            if (data.ViolationTime != null)
                black.ViolationTime = data.ViolationTime;
            if (!string.IsNullOrWhiteSpace(data.ViolationDesc))
                black.ViolationDesc = data.ViolationDesc;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                foreach (long id in BlackIds)
                {
                    string checksql = $"select 1 from EpaperSubscriber where ID={ id } and IsBlack=1";
                    var check = conn.Query<int>(checksql);
                    if (check.Count() > 0)
                        BlackIds = BlackIds.Where(m => m != id);
                }

                if (BlackIds.Count() > 0)
                {
                    foreach (var item in BlackIds)
                    {
                        black.EpaperSubscriber_ID = item;
                        SetEpaperBlackInfo(black);
                    }

                    conn.Execute(
                        string.Format("update EpaperSubscriber set IsBlack=1 where ID in ({0})", string.Join(", ", BlackIds))
                        );
                }
            }
        }
        /// <summary>
        /// 訂閱者名單 > 新增訂閱者
        /// </summary>
        /// <param name="item"></param>
        public static void InsertSubscribers(EpaperSubscriber item, out string msg, string[] EpaperTypeSelect = null)
        {
            msg = string.Empty;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperSubscriber");
            long EpaperSubscriber_ID = CheckEmailInEpaperSubscriber(item.Email, item.SiteID);
            long MemberShip_ID = CheckEmailInMemberShip(item.Email, item.SiteID);
            if (MemberShip_ID != 0)
                item.Member_ID = MemberShip_ID; //在會員資料中有相同email --> 帶入

            if (EpaperSubscriber_ID == 0)
            {
                //新增訂閱者資料
                tableObj.GetDataFromObject(item);
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;
                tableObj.Insert();

                IEnumerable<EpaperTypeModel> epaperType = GetEpaperType(item.SiteID);
                if (EpaperTypeSelect != null)
                {
                    if (EpaperTypeSelect.Length > 0)
                        epaperType = epaperType.Where(m => EpaperTypeSelect.Contains(m.ID.ToString()));
                }

                if (epaperType.Any() && MemberShip_ID == 0) //會員的訂閱不處理
                {
                    string sql = "";
                    foreach (var tye in epaperType)
                    {
                        sql += $"insert into EpaperSubscriberDetail (EpaperSubscriber_ID,EpaperType_ID,CreateTime) values ({ item.ID },{ tye.ID },getdate()) ";
                    }
                    db.ExecuteNonQuery(sql);
                }
            }
            else
            {
                //訂閱者資料已經存在
                if (MemberShip_ID != 0)
                {
                    //訂閱者資料已經存在但會員資料中有相同Email-->更新寫入MemberShip ID (用在先一般訂閱、後加入會員時)
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                    {
                        string sql = $"update EpaperSubscriber set Member_ID={MemberShip_ID},Modifier={MemberDAO.SysCurrent.Id},ModifyTime='{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' where ID={EpaperSubscriber_ID}";
                        conn.Execute(sql);
                    }
                }
                else
                {
                    //有訂閱者資料但不是會員(一般訂閱者),重建訂閱資料
                    var typModel = GetEpaperType(item.SiteID);
                    if (typModel.Count > 0)
                    {
                        IEnumerable<long> EpaperTypIds = typModel.Select(m => m.ID);
                        var alreadyOrderIds = QueryEpaperTypeByEpaperSubscriber(EpaperSubscriber_ID);
                        StringBuilder sb = new StringBuilder();
                        foreach (var id in EpaperTypIds.Where(m => !alreadyOrderIds.Contains(m)))
                        {
                            sb.Append($"insert into EpaperSubscriberDetail (EpaperSubscriber_ID,EpaperType_ID,CreateTime) values ({ EpaperSubscriber_ID },{ id },getdate()) ");
                        }
                        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                        {
                            conn.Execute(sb.ToString());
                        }
                    }
                }
            }
            if (MemberShip_ID != 0)
            {
                msg = "成功儲存訂閱者(該E-mail為會員，請至會員名單設定報別)。";
            }
        }
        /// <summary>
        /// 查詢訂閱者ID訂閱了那些電子報
        /// </summary>
        /// <param name="EpaperSubscriber_ID"></param>
        /// <returns></returns>
        public static IEnumerable<long> QueryEpaperTypeByEpaperSubscriber(long EpaperSubscriber_ID)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string query = $"select EpaperType_ID from EpaperSubscriberDetail where EpaperSubscriber_ID={EpaperSubscriber_ID}";
                return conn.Query<long>(query);
            }
        }
        /// <summary>
        /// 檢查Email在會員資料(MemberShip)中是否被註冊並回傳ID值，否則傳回0
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static long CheckEmailInMemberShip(string email, long SiteID)
        {
            if (string.IsNullOrWhiteSpace(email))
                return 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select ID from MemberShip where SiteID={SiteID} and Email='{email.Replace("'", "''")}'";
                var query = conn.Query<long>(sql);
                if (query.Count() > 0)
                    return query.SingleOrDefault();
                else
                    return 0;
            }
        }
        /// <summary>
        /// 檢查Email在訂閱者資料(EpaperSubscriber)是否被使用並回傳ID值，否則傳回0
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        static long CheckEmailInEpaperSubscriber(string email, long SiteID)
        {
            if (string.IsNullOrWhiteSpace(email))
                return 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select ID from EpaperSubscriber where SiteID={SiteID} and Email='{email.Replace("'", "''")}'";
                var query = conn.Query<long>(sql);
                if (query.Count() > 0)
                    return query.FirstOrDefault();
                else
                    return 0;
            }
        }
        /// <summary>
        /// 刪除訂閱者
        /// </summary>
        /// <param name="ids">EpaperSubscriber.ID</param>
        /// <returns></returns>
        public static int SubscriberDel(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql = @"Delete From EpaperSubscriber Where ID In ({0}) 
                           Delete From EpaperSubscriberDetail Where EpaperSubscriber_ID In ({0}) 
                           Delete From EpaperSendList Where EpaperSubscriber_ID In ({0}) ";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }
        /// <summary>
        /// 黑名單 > 新增黑名單
        /// </summary>
        /// <param name="item"></param>
        public static void InsertBlackListData(EpaperSubscriber item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperSubscriber");
            long EpaperSubscriber_ID = CheckEmailInEpaperSubscriber(item.Email, item.SiteID);
            long MemberShip_ID = CheckEmailInMemberShip(item.Email, item.SiteID);
            if (MemberShip_ID != 0)
                item.Member_ID = MemberShip_ID; //在會員資料中有相同email --> 帶入

            tableObj.GetDataFromObject(item);
            if (EpaperSubscriber_ID == 0)
            {
                //新增訂閱者資料
                EpaperSubscriber_ID = WorkLib.GetItem.NewSN();
                tableObj["ID"] = EpaperSubscriber_ID;
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;
                tableObj.Insert();
            }
            else
            {
                //訂閱者資料已經存在
                if (MemberShip_ID != 0)
                {
                    //訂閱者資料已經存在但會員資料中有相同Email-->更新黑名單狀態並寫入MemberShip ID
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                    {
                        //20181129 Nina 新增黑名單，暫停寄送要預設為勾選。加入, IsPause={(item.IsPause ? 1 : 0)}
                        string sql = $"update EpaperSubscriber set IsBlack={(item.IsBlack ? 1 : 0)}, IsPause={(item.IsPause ? 1 : 0)} , Member_ID={MemberShip_ID},Modifier={MemberDAO.SysCurrent.Id},ModifyTime='{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' where ID={EpaperSubscriber_ID}";
                        conn.Execute(sql);
                    }
                }
                else
                {
                    //訂閱者資料存在但並非會員
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                    {
                        //20181129 Nina 新增黑名單，暫停寄送要預設為勾選。加入, IsPause={(item.IsPause ? 1 : 0)}
                        string sql = $"update EpaperSubscriber set IsBlack={(item.IsBlack ? 1 : 0)}, IsPause={(item.IsPause ? 1 : 0)} ,Modifier={MemberDAO.SysCurrent.Id},ModifyTime='{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' where ID={EpaperSubscriber_ID}";
                        conn.Execute(sql);
                    }
                }
            }
            //新增黑名單資訊
            item.EpaperBlackListInfo.EpaperSubscriber_ID = EpaperSubscriber_ID;
            SetEpaperBlackInfo(item.EpaperBlackListInfo);
        }
        /// <summary>
        /// 移除黑名單
        /// </summary>
        /// <param name="ids"></param>
        public static void RemoveBlackList(IEnumerable<long> ids)
        {
            if (ids == null)
                return;
            if (ids.Count() == 0)
                return;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = string.Format("Update EpaperSubscriber Set IsBlack = 0, IsPause=0, FailureTimes = 0 Where ID in ({0})", string.Join(",", ids));
                sql += $" delete from EpaperBlackListInfo where EpaperSubscriber_ID in ({string.Join(",", ids)})";
                conn.Execute(sql);
            }
        }
        /// <summary>
        /// 透過Send ID 取得有哪些訂閱者ID
        /// </summary>
        /// <param name="EpaperSendID"></param>
        /// <returns></returns>
        static IEnumerable<long> QuerySubscribersByEpaperSendID(long EpaperSendID)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string querySubscribers = $"select EpaperSendList.EpaperSubscriber_ID from EpaperSendList inner join EpaperSend on EpaperSend.ID=EpaperSendList.EpaperSend_ID where EpaperSend.ID={EpaperSendID} and EpaperSendList.EpaperSubscriber_ID <> 0";
                return conn.Query<long>(querySubscribers);
            }
        }
        /// <summary>
        /// 透過訂閱者IDs取得相關資訊
        /// </summary>
        /// <param name="SubscribersIds"></param>
        /// <returns></returns>
        public static List<EpaperSubscriber> GetSubscribersList(IEnumerable<long> SubscribersIds)
        {
            List<EpaperSubscriber> list = new List<EpaperSubscriber>();
            if (SubscribersIds.Count() == 0)
                return list;

            string sql = @"select EpaperSubscriber.* from EpaperSubscriber 
                           left join MemberShip on MemberShip.ID = EpaperSubscriber.Member_ID
                           where EpaperSubscriber.ID in ({0})";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(string.Format(sql, string.Join(",", SubscribersIds)));

            if (datas == null)
                return list;

            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new EpaperSubscriber
                {
                    ID = (long)dr["ID"],
                    Member_ID = dr["Member_ID"] as long?,
                    Email = dr["Email"].ToString().Trim(),
                    Likes = dr["Likes"] == null ? "" : dr["Likes"].ToString().Trim(),
                    FailureTimes = dr["FailureTimes"] as int?,
                    IsBlack = (bool)dr["IsBlack"],
                    IsPause = (bool)dr["IsPause"],
                    CreateTime = (DateTime)dr["CreateTime"],
                    Detail = GetSubscriberDetail((long)dr["ID"]),
                    MemberShip = dr["Member_ID"] is DBNull ? new WorkV3.Models.MemberShipModels() : CommonDA.GetItem<WorkV3.Models.MemberShipModels>("MemberShip", (long)dr["Member_ID"])
                });
            }
            return list;
        }
        /// <summary>
        /// 儲存黑名單資訊(dbo.EpaperBlackListInfo)
        /// </summary>
        /// <param name="item"></param>
        public static void SetEpaperBlackInfo(EpaperBlackListInfo item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperBlackListInfo");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From EpaperBlackListInfo Where EpaperSubscriber_ID = " + item.EpaperSubscriber_ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("EpaperSubscriber_ID");
                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 取得黑名單資訊(dbo.EpaperBlackListInfo)
        /// </summary>
        /// <param name="EpaperSubscriber_ID"></param>
        /// <returns></returns>
        public static EpaperBlackListInfo GetEpaperBlackInfo(long EpaperSubscriber_ID)
        {
            if (EpaperSubscriber_ID == 0)
                return new EpaperBlackListInfo();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select * from EpaperBlackListInfo where EpaperSubscriber_ID={EpaperSubscriber_ID}";
                var result = conn.Query<EpaperBlackListInfo>(sql);
                if (result.Any())
                    return result.FirstOrDefault();
                else
                    return new EpaperBlackListInfo();
            }
        }
        /// <summary>
        /// 刪除黑名單(dbo.EpaperBlackListInfo)
        /// </summary>
        /// <param name="EpaperSubscriber_ID"></param>
        public static void DelEpaperBlackInfo(long EpaperSubscriber_ID)
        {
            if (EpaperSubscriber_ID == 0)
                return;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"Delete from EpaperBlackListInfo where EpaperSubscriber_ID={EpaperSubscriber_ID}";
                conn.Execute(sql);
            }
        }
        /// <summary>
        /// 透過會員ID (dbo.MemberShip) 查詢訂閱
        /// </summary>
        /// <param name="MembershipID"></param>
        /// <returns></returns>
        public static List<EpaperSubscriberDetail> GetSubscriberDetailByMembershipID(long MembershipID)
        {
            List<EpaperSubscriberDetail> list = new List<EpaperSubscriberDetail>();
            string sql = @"
                        select 
                        EpaperSubscriberDetail.*,
						EpaperType.[Name] EpaperTypeName,
                        EpaperType.[ID] EpaperTypeID
                        from EpaperSubscriberDetail
                        inner join EpaperType on EpaperSubscriberDetail.EpaperType_ID=EpaperType.ID
                        inner join EpaperSubscriber on EpaperSubscriber.ID = EpaperSubscriberDetail.EpaperSubscriber_ID
                        order by EpaperSubscriberDetail.CreateTime desc
                            ";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            foreach (DataRow dr in datas.Rows)
            {
                list.Add(new EpaperSubscriberDetail
                {
                    ID = (long)dr["ID"],
                    EpaperSubscriber_ID = (long)dr["EpaperSubscriber_ID"],
                    EpaperType_ID = (long)dr["EpaperType_ID"],
                    CreateTime = (DateTime)dr["CreateTime"],
                    EpaperTypeID = (long)dr["EpaperTypeID"],
                    EpaperTypeName = dr["EpaperTypeName"].ToString().Trim()
                });
            }
            return list;
        }
        /// <summary>
        /// 將訂閱者名單(dbo.EpaperSubscriber)上面的會員ID(Member_ID)設為NULL
        /// </summary>
        /// <param name="EpaperSubscriberID"></param>
        public static void ClearMemberIdInSubscriber(long EpaperSubscriberID)
        {
            if (EpaperSubscriberID == 0)
                return;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"update EpaperSubscriber set Member_ID = NULL where ID={EpaperSubscriberID} ";
                conn.Execute(sql);
            }
        }
        /// <summary>
        /// 確認特定Member ID在訂閱者名單當中是否存在
        /// </summary>
        /// <param name="MemberID"></param>
        /// <returns></returns>
        public static bool CheckMemIDIsExistInSubscriber(long MemberID, out long EpaperSubscriberID)
        {
            EpaperSubscriberID = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"select ID from EpaperSubscriber where Member_ID={MemberID}";
                var result = conn.Query<long>(sql);
                if (result.Any())
                {
                    EpaperSubscriberID = result.ElementAt(0);
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion

        #region 電子報分析

        /// <summary>
        /// 取得訂閱/退訂數量 by Day
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Order">訂閱/退訂</param>
        /// <param name="EpaperTypeId"></param>
        /// <returns></returns>
        public static int GetOrderCountByDateTime(long SiteID, DateTime dt, bool Order, bool IssueOnly = false, IEnumerable<long> EpaperTypeIds = null)
        {
            if (SiteID == 0 || dt == default(DateTime))
                return 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "select count(*) from EpaperSubscribeRecord " +
                             "inner join EpaperSubscriber on EpaperSubscriber.ID=EpaperSubscribeRecord.EpaperSubscriber_ID " +
                             "inner join EpaperType on EpaperType.ID=EpaperSubscribeRecord.EpaperType_ID " +
                             $"where EpaperSubscriber.SiteID={SiteID} and RecordTime='{dt.ToString("yyyy/MM/dd")}' and IsSubscribe={(Order ? "1" : "0")}" +
                             " {0} ";

                string condition = "";
                if (EpaperTypeIds != null)
                {
                    if (EpaperTypeIds.Count() > 0)
                        condition += string.Format(" and EpaperSubscribeRecord.EpaperType_ID in ({0})", string.Join(",", EpaperTypeIds));

                }
                if (IssueOnly)
                    condition += " and EpaperType.Status=1 ";

                sql = string.Format(sql, condition);

                var result = conn.Query<int>(sql);
                if (result.Any())
                    return result.ElementAt(0);
                else
                    return 0;

                //var result = conn.Query<long>(sql);
                //return result.Count();
            }
        }
        /// <summary>
        /// 取得訂閱/退訂數量 for excel report
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="EpaperTypeId"></param>
        /// <param name="Order"></param>
        /// <param name="IsMember"></param>
        /// <returns></returns>
        public static int GetOrderCount(DateTime dt, long SiteID, long EpaperTypeId, bool Order, bool? IsMember)
        {
            if (SiteID == 0)
                return 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "select count(*) from EpaperSubscribeRecord " +
                             "inner join EpaperSubscriber on EpaperSubscriber.ID=EpaperSubscribeRecord.EpaperSubscriber_ID " +
                             $"where EpaperSubscriber.SiteID={SiteID} and RecordTime='{dt.ToString("yyyy/MM/dd")}'" +
                             "{0}";

                string condition = "";
                condition += $" and EpaperSubscribeRecord.IsSubscribe={ ((bool)Order ? "1" : "0") }";
                if (EpaperTypeId != 0)
                    condition += string.Format(" and EpaperSubscribeRecord.EpaperType_ID = {0}", EpaperTypeId);
                if (IsMember != null)
                    condition += $" and EpaperSubscriber.Member_ID { ((bool)IsMember ? "is not null" : "is null") }";

                sql = string.Format(sql, condition);

                var result = conn.Query<int>(sql);
                if (result.Any())
                    return result.ElementAt(0);
                else
                    return 0;
            }
        }
        /// <summary>
        /// 取得時間區間內 訂閱與退訂之每日數量
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="Order">true訂閱/false退訂</param>
        /// <returns></returns>
        public static IEnumerable<int> GetOrderCountList(long SiteID, DateTime start, DateTime end, bool Order, bool IssueOnly = false, IEnumerable<long> EpaperTypeIds = null)
        {
            int dayDiff = new TimeSpan(end.Ticks - start.Ticks).Days;
            List<int> list = new List<int>();

            for (int i = 0; i <= dayDiff; i++)
            {
                //第一筆直接存當日訂閱/無訂閱的總數量
                if (i == 0)
                    list.Add(GetOrderCountByDateTime(SiteID, start.AddDays(i), Order, IssueOnly: IssueOnly, EpaperTypeIds: EpaperTypeIds));
                else
                {
                    int diff = GetDiffWithOrderStatus(SiteID, start.AddDays(i - 1), start.AddDays(i), Order, IssueOnly: IssueOnly, EpaperTypeIds: EpaperTypeIds);
                    list.Add(diff);
                }
            }
            return list;
        }
        /// <summary>
        /// 以 前後datetime值 取得 訂閱/退訂數量
        /// </summary>
        /// <param name="dtPrev"></param>
        /// <param name="dtCurrent"></param>
        /// <param name="Order">true 訂閱 false 退訂</param>
        /// <returns></returns>
        public static int GetDiffWithOrderStatus(long SiteID, DateTime dtPrev, DateTime dtCurrent, bool Order, bool IssueOnly = false, IEnumerable<long> EpaperTypeIds = null)
        {
            DateTime dtNow = DateTime.Now;

            #region 檢查

            //檢查dtCurrent是否晚於DateTime.Now
            int checkDtCurrentLaterThanNow = DateTime.Compare(new DateTime(dtCurrent.Year, dtCurrent.Month, dtCurrent.Day), new DateTime(dtNow.Year, dtNow.Month, dtNow.Day));
            if (checkDtCurrentLaterThanNow > 0)
                return 0;

            //檢查dtCurrent是否比dtPrev還早
            //以new datetime方式是要防止有時分的因素導致干擾compare
            CheckDateTimeStartAndEnd(ref dtPrev, ref dtCurrent, excludeHourSec: true);

            //如果發現有未執行排程(沒有資料)的就傳回0
            if (!Order && !WorkV3.Models.DataAccess.EpaperDAO.CheckAlreadyExeInTheDay(dtPrev))
                return 0;
            if (!WorkV3.Models.DataAccess.EpaperDAO.CheckAlreadyExeInTheDay(dtCurrent))
                return 0;

            #endregion
            //取得前一天的訂閱/退訂數量
            int prev = GetOrderCountByDateTime(SiteID, dtPrev, Order, IssueOnly: IssueOnly, EpaperTypeIds: EpaperTypeIds);
            //取得目前讀取日期的訂閱/退訂數量
            int current = GetOrderCountByDateTime(SiteID, dtCurrent, Order, IssueOnly: IssueOnly, EpaperTypeIds: EpaperTypeIds);
            //差異結果(訂閱:當日減前一天 , 退訂: 前一天減當日 )
            int diff = current - prev;//Order ? current - prev : prev - current;
            //如果差異結果大於0有使用者訂閱/退訂、小於等於0代表沒有人訂閱/退訂(直接給0)
            return diff <= 0 ? 0 : diff;
        }
        /// <summary>
        /// 電子報分析 > 訂閱/退訂 報表欄位參數用
        /// </summary>
        /// <param name="Concat"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        public static void GenChartStepPara(IEnumerable<int> Concat, ref int max, ref int step)
        {
            var source = Concat;
            var sourceMax = source.Max();
            if (sourceMax <= 60)
            {
                max = 60;
                step = 10;
            }
            else if (sourceMax <= 120)
            {
                max = 120;
                step = 20;
            }
            else if (sourceMax <= 1200)
            {
                max = 1200;
                step = 200;
            }
            else if (sourceMax <= 12000)
            {
                max = 12000;
                step = 2000;
            }
            else if (sourceMax <= 120000)
            {
                max = 120000;
                step = 20000;
            }
            else if (sourceMax <= 1200000)
            {
                max = 1200000;
                step = 200000;
            }
            else if (sourceMax <= 12000000)
            {
                max = 12000000;
                step = 2000000;
            }
            else if (sourceMax <= 120000000)
            {
                max = 120000000;
                step = 20000000;
            }
        }
        /// <summary>
        /// 電子報分析 > 訂閱人數顯示
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSubscribeRecordModel> OrderCountDisplay(long SiteID, DateTime start, DateTime end)
        {
            CheckDateTimeStartAndEnd(ref start, ref end);
            var diff = new TimeSpan(end.Ticks - start.Ticks).Days;

            IEnumerable<EpaperSubscribeRecordModel> list = GroupRecord(SiteID, start, diff);
            return list.Where(m => m.IsSubscribe == true);
        }
        /// <summary>
        /// 訂閱紀錄資訊處理
        /// 不重複EpaperSubscriberID+EpaperType的狀況下 取得最後的訂閱狀態
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="start"></param>
        /// <param name="diff"></param>
        /// <returns></returns>
        private static IEnumerable<EpaperSubscribeRecordModel> GroupRecord(long SiteID, DateTime start, int diff)
        {
            List<EpaperSubscribeRecordModel> list = new List<EpaperSubscribeRecordModel>();
            for (int i = 0; i <= diff; i++)
            {
                IEnumerable<EpaperSubscribeRecordModel> result = null;
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    string sql = "select EpaperSubscribeRecord.*, " +
                        "                EpaperType.Status as EpaperTypeStatus, " +
                                        "(case when EpaperSubscriber.Member_ID is null  then 0 else 1 end) as IsMember " +
                                 "from EpaperSubscribeRecord " +
                                 "inner join EpaperSubscriber on EpaperSubscriber.ID=EpaperSubscribeRecord.EpaperSubscriber_ID " +
                                 "inner join EpaperType on EpaperType.ID = EpaperSubscribeRecord.EpaperType_ID " +
                                $"where EpaperSubscriber.SiteID={SiteID} and RecordTime='{start.AddDays(i).ToString("yyyy/MM/dd")}' ";
                    result = conn.Query<EpaperSubscribeRecordModel>(sql);
                }
                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        if (list.Any(m => m.EpaperSubscriber_ID == item.EpaperSubscriber_ID && m.EpaperType_ID == item.EpaperType_ID))
                        {
                            int index = list.FindIndex(m => m.EpaperSubscriber_ID == item.EpaperSubscriber_ID && m.EpaperType_ID == item.EpaperType_ID);
                            EpaperSubscribeRecordModel data = list[index];
                            data.IsSubscribe = item.IsSubscribe;
                            list[index] = data;
                        }
                        else
                            list.Add(item);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 電子報分析 > 退訂人數顯示
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSubscribeRecordModel> UnOrderCountDisplay(long SiteID, DateTime start, DateTime end)
        {
            CheckDateTimeStartAndEnd(ref start, ref end);
            var diff = new TimeSpan(end.Ticks - start.Ticks).Days;

            DateTime? okStart = null;
            for (int i = 0; i < diff; i++)
            {
                var CurrentIsExist = WorkV3.Models.DataAccess.EpaperDAO.CheckAlreadyExeInTheDay(start.AddDays(i));
                if (CurrentIsExist)
                {
                    okStart = start.AddDays(i);
                    break;
                }
            }

            IEnumerable<EpaperSubscribeRecordModel> list1 = new List<EpaperSubscribeRecordModel>();
            list1 = (okStart != null) ? GroupRecord(SiteID, (DateTime)okStart, 0) : GroupRecord(SiteID, start, diff);

            IEnumerable<EpaperSubscribeRecordModel> list2 = new List<EpaperSubscribeRecordModel>();
            list2 = GroupRecord(SiteID, start, diff);

            List<EpaperSubscribeRecordModel> result = new List<EpaperSubscribeRecordModel>();
            foreach (var item in list2)
            {
                if (list1.Any(m => m.EpaperSubscriber_ID == item.EpaperSubscriber_ID && m.EpaperType_ID == item.EpaperType_ID))
                {
                    var prevData = list1.Where(m => m.EpaperSubscriber_ID == item.EpaperSubscriber_ID && m.EpaperType_ID == item.EpaperType_ID).FirstOrDefault();
                    if (prevData.IsSubscribe && !item.IsSubscribe)
                    {
                        result.Add(prevData);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 檢查起訖時間並調換
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="excludeHourSec">以new datetime方式處理。防止時分因素導致干擾Compare</param>
        public static void CheckDateTimeStartAndEnd(ref DateTime start, ref DateTime end, bool excludeHourSec = false)
        {
            int compare = 0;
            if (excludeHourSec)
                compare = DateTime.Compare(new DateTime(end.Year, end.Month, end.Day), new DateTime(start.Year, start.Month, start.Day));
            else
                compare = DateTime.Compare(end, start);

            if (compare < 0)
            {
                DateTime dtTemp = end;
                start = end;
                end = start;
            }
        }
        /// <summary>
        /// 產生電子報分析訂退閱Excel報表
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSubRecExcelIndexModel> GenEpaperSubscribeRecordExcel(long SiteID, DateTime Start, DateTime End, EpaperAnalysisReport typ)
        {
            List<EpaperSubRecExcelIndexModel> listIndexCollection = new List<EpaperSubRecExcelIndexModel>();
            var EpaperTypes = GetEpaperType(SiteID);

            if (EpaperTypes == null)
                return null;

            //檢查End是否比Start還早
            CheckDateTimeStartAndEnd(ref Start, ref End);

            if (typ == EpaperAnalysisReport.day)
            {
                int daydiff = new TimeSpan(End.Ticks - Start.Ticks).Days;

                #region 日報表
                foreach (var EpaperType in EpaperTypes)
                {
                    EpaperSubRecExcelIndexModel listIndex = new EpaperSubRecExcelIndexModel();
                    List<EpaperSubscribeRecordExcelDayModel> list = new List<EpaperSubscribeRecordExcelDayModel>();
                    for (int i = 0; i <= daydiff; i++)
                    {
                        EpaperSubscribeRecordExcelDayModel data = new EpaperSubscribeRecordExcelDayModel();

                        int TotalOrder = 0, TotalUnOrder = 0;

                        //第一筆直接存當日訂閱/無訂閱的總數量
                        if (i == 0)
                        {
                            TotalOrder = GetOrderCountByDateTime(SiteID, Start.AddDays(i), true, IssueOnly: true, EpaperTypeIds: new List<long>() { EpaperType.ID });
                            TotalUnOrder = GetOrderCountByDateTime(SiteID, Start.AddDays(i), false, IssueOnly: true, EpaperTypeIds: new List<long>() { EpaperType.ID });
                        }
                        else
                        {
                            TotalOrder = GetDiffWithOrderStatus(SiteID, Start.AddDays(i - 1), Start.AddDays(i), true, IssueOnly: true, EpaperTypeIds: new List<long>() { EpaperType.ID });
                            TotalUnOrder = GetDiffWithOrderStatus(SiteID, Start.AddDays(i - 1), Start.AddDays(i), false, IssueOnly: true, EpaperTypeIds: new List<long>() { EpaperType.ID });
                        }

                        //int TotalOrder = GetOrderCount(Start.AddDays(i), SiteID, EpaperType.ID, true, null);
                        //int TotalUnOrder = GetOrderCount(Start.AddDays(i), SiteID, EpaperType.ID, false, null);

                        if (TotalOrder == 0 && TotalUnOrder == 0)
                            continue;

                        data.Time = Start.AddDays(i).ToString("yyyy/MM/dd");
                        data.TotalOrder = TotalOrder.ToString();
                        data.TotalUnOrder = TotalUnOrder.ToString();

                        list.Add(data);
                    }
                    listIndex.excelDayModel = list;
                    listIndex.EpaperTypeName = !string.IsNullOrWhiteSpace(EpaperType.Name) ? EpaperType.Name : EpaperType.ID.ToString();
                    listIndexCollection.Add(listIndex);
                }
                #endregion
            }
            else if (typ == EpaperAnalysisReport.month)
            {
                int monthdiff = MonthDifference(Start, End);
                Start = new DateTime(Start.Year, Start.Month, 1); //起始直改成每月一號

                #region 月報表
                foreach (var EpaperType in EpaperTypes)
                {
                    EpaperSubRecExcelIndexModel listIndex = new EpaperSubRecExcelIndexModel();
                    List<EpaperSubscribeRecordExcelMonthModel> list = new List<EpaperSubscribeRecordExcelMonthModel>();

                    for (int i = 0; i <= monthdiff; i++)
                    {
                        EpaperSubscribeRecordExcelMonthModel data = new EpaperSubscribeRecordExcelMonthModel();

                        int TotalOrder = 0, TotalUnOrder = 0;

                        IEnumerable<EpaperSubscribeRecordModel> OrderCountDisplay = EpaperDAO.OrderCountDisplay(SiteID, Start.AddMonths(i), Start.AddMonths(i + 1).AddDays(-1));
                        IEnumerable<EpaperSubscribeRecordModel> UnOrderCountDisplay = EpaperDAO.UnOrderCountDisplay(SiteID, Start.AddMonths(i), Start.AddMonths(i + 1).AddDays(-1));

                        TotalOrder = OrderCountDisplay.Where(m => m.EpaperType_ID == EpaperType.ID).Count();
                        TotalUnOrder = UnOrderCountDisplay.Where(m => m.EpaperType_ID == EpaperType.ID).Count();

                        //int TotalOrder = GetOrderCount(Start.AddDays(i), SiteID, EpaperType.ID, true, null);
                        //int TotalUnOrder = GetOrderCount(Start.AddDays(i), SiteID, EpaperType.ID, false, null);

                        if (TotalOrder == 0 && TotalUnOrder == 0)
                            continue;

                        data.Time = Start.AddMonths(i).ToString("yyyy/MM/dd");
                        data.TotalOrder = TotalOrder.ToString();
                        data.TotalUnOrder = TotalUnOrder.ToString();
                        data.TotalOrderPercent = "";
                        data.TotalUnOrderPercent = "";

                        list.Add(data);
                    }
                    int allTotalOrder = list.Sum(m => Convert.ToInt32(m.TotalOrder));
                    int allTotalUnOrder = list.Sum(m => Convert.ToInt32(m.TotalUnOrder));
                    foreach (var item in list)
                    {
                        var allTotalOrderPercent = GenPercentValue(Convert.ToInt32(item.TotalOrder), allTotalOrder);
                        var allTotalUnOrderPercent = GenPercentValue(Convert.ToInt32(item.TotalUnOrder), allTotalUnOrder);
                        item.TotalOrderPercent = string.Format("{0}%", checkDoubleAndReturnStr(allTotalOrderPercent));
                        item.TotalUnOrderPercent = string.Format("{0}%", checkDoubleAndReturnStr(allTotalUnOrderPercent));
                    }
                    listIndex.excelMonthModel = list;
                    listIndex.EpaperTypeName = !string.IsNullOrWhiteSpace(EpaperType.Name) ? EpaperType.Name : EpaperType.ID.ToString();
                    listIndexCollection.Add(listIndex);
                }
                #endregion
            }
            else if (typ == EpaperAnalysisReport.week)
            {

            }

            return listIndexCollection;
        }
        #endregion

        #region 電子報分析 標線
        /// <summary>
        /// 取得自訂標線資訊
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSubRecStatisticLabelsModel> GetStatisticLabelItems(EpaperSubRecStatisticLabelsSearchModel search, int pageSize, int pageIndex, out int recordCount)
        {
            List<EpaperSubRecStatisticLabelsModel> items = new List<EpaperSubRecStatisticLabelsModel>();
            if (search == null)
            {
                recordCount = 0;
                return items;
            }

            string sql = "Select *  From EpaperSubRecStatisticLabels  Where 1=1 {0} Order By LabelDate Desc";

            List<string> where = new List<string>();

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                where.Add(string.Format(" ( Title Like N'%{0}%' )", search.Keyword.Replace("'", "''")));
            }

            if (search.StartDate.HasValue)
                where.Add(string.Format(" LabelDate >= '{0:yyyy/MM/dd HH:mm}' ", search.StartDate));

            if (search.EndDate.HasValue)
                where.Add(string.Format(" LabelDate <= '{0:yyyy/MM/dd HH:mm}' ", search.EndDate));

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(string.Format(sql, where.Count > 0 ? " And " + string.Join(" And ", where) : ""), pageSize, pageIndex, out recordCount);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new EpaperSubRecStatisticLabelsModel
                    {
                        ID = (long)dr["ID"],
                        Title = dr["Title"].ToString().Trim(),
                        LabelColor = dr["LabelColor"].ToString().Trim(),
                        LabelDate = (DateTime)dr["LabelDate"],
                        ShowStatus = (bool)dr["ShowStatus"],
                        CreateTime = DateTime.Parse(dr["CreateTime"].ToString()),
                        Creator = (long)dr["Creator"],
                        ModifyTime = DateTime.Parse(dr["ModifyTime"].ToString()),
                        Modifier = (long)dr["Modifier"]
                    });
                }
            }

            return items;
        }
        /// <summary>
        /// 取得自訂標線資訊(單筆)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EpaperSubRecStatisticLabelsModel GetStatisticLabelItem(long id)
        {
            return CommonDA.GetItem<EpaperSubRecStatisticLabelsModel>("EpaperSubRecStatisticLabels", id);
        }
        /// <summary>
        /// 寫入自訂標線
        /// </summary>
        /// <param name="item"></param>
        public static void SetStatisticLabelItem(EpaperSubRecStatisticLabelsModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("EpaperSubRecStatisticLabels");
            tableObj.GetDataFromObject(item);
            bool isNew = false;
            string sql = "Select 1 From EpaperSubRecStatisticLabels Where ID = " + item.ID;
            isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Title"] = item.Title;
                tableObj["LabelDate"] = item.LabelDate;
                tableObj["LabelColor"] = item.LabelColor;
                tableObj["ShowStatus"] = item.ShowStatus;
                tableObj["Creator"] = item.Creator;
                tableObj["CreateTime"] = item.CreateTime;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj["Title"] = item.Title;
                tableObj["LabelDate"] = item.LabelDate;
                tableObj["ShowStatus"] = item.ShowStatus;
                tableObj["LabelColor"] = item.LabelColor;
                tableObj["Modifier"] = item.Modifier;
                tableObj["ModifyTime"] = item.ModifyTime;
                tableObj.Update(item.ID);
            }
        }
        /// <summary>
        /// 變更狀態
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ShowStatus"></param>
        public static void SetStatisticLabelItemStatus(long ID, bool ShowStatus)
        {
            EpaperSubRecStatisticLabelsModel item = new EpaperSubRecStatisticLabelsModel();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);
            paraList.Add("@ShowStatus", ShowStatus);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string updateSQL = " UPDATE EpaperSubRecStatisticLabels SET ShowStatus=@ShowStatus WHERE ID=@ID ";
            db.ExecuteNonQuery(updateSQL, paraList);
        }
        /// <summary>
        /// 取得標線
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSubRecStatisticLabelsModel> GetShowLabelLine(DateTime startDate, DateTime endDate)
        {
            List<EpaperSubRecStatisticLabelsModel> items = new List<EpaperSubRecStatisticLabelsModel>();
            string sql = "Select *  From EpaperSubRecStatisticLabels  Where ShowStatus=1 AND LabelDate>=@StartDate AND LabelDate<=@EndDate Order By LabelDate Asc";

            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate);
            paraList.Add("@EndDate", endDate);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql, paraList);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new EpaperSubRecStatisticLabelsModel
                    {
                        ID = (long)dr["ID"],
                        Title = dr["Title"].ToString().Trim(),
                        LabelColor = dr["LabelColor"].ToString().Trim(),
                        LabelDate = (DateTime)dr["LabelDate"],
                        ShowStatus = (bool)dr["ShowStatus"],
                        CreateTime = DateTime.Parse(dr["CreateTime"].ToString()),
                        Creator = (long)dr["Creator"],
                        ModifyTime = DateTime.Parse(dr["ModifyTime"].ToString()),
                        Modifier = (long)dr["Modifier"]
                    });
                }
            }

            return items;
        }
        /// <summary>
        /// 刪除自訂標線
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static int DeleteStatisticLabelItem(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "delete EpaperSubRecStatisticLabels where ID IN ({0})";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }
        #endregion

        #region Excel輸出
        /// <summary>
        /// 隱藏姓名
        /// </summary>
        /// <param name="NameStr"></param>
        /// <returns></returns>
        static string HiddenName(string NameStr)
        {
            if (string.IsNullOrWhiteSpace(NameStr))
                return string.Empty;

            if (NameStr.Length >= 2)
                return NameStr.Remove(1, 1).Insert(1, "O");
            else
                return string.Empty;
        }
        /// <summary>
        /// 隱藏Email
        /// </summary>
        /// <param name="EmailStr"></param>
        /// <returns></returns>
        static string HiddenEmail(string EmailStr)
        {
            if (string.IsNullOrWhiteSpace(EmailStr))
                return string.Empty;

            int AtSymbolIndex = EmailStr.IndexOf('@');
            if (AtSymbolIndex != -1)
            {
                string result = EmailStr.Substring(0, 1) + EmailStr.Substring(AtSymbolIndex - 1).PadLeft(
                                                                      EmailStr.Substring(AtSymbolIndex - 1).Length + AtSymbolIndex - 2,
                                                                      '*');
                return result;
            }
            else
                return string.Empty;
        }
        /// <summary>
        /// 轉成百分比(小數點兩位)
        /// </summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        /// <returns></returns>
        public static double GenPercentValue(int numerator, int denominator)
        {
            double v = 0;
            if (numerator <= denominator)
            {
                v = Math.Round((Convert.ToDouble(numerator) / Convert.ToDouble(denominator)) * 100, 2);
            }
            return v;
        }
        /// <summary>
        /// 透過訂閱者ID列表加入黑名單
        /// </summary>
        /// <param name="EpaperSubscriber_ID"></param>
        public static void AddBlacklistBySubscriberID(IEnumerable<long> EpaperSubscriberIDs)
        {
            if (EpaperSubscriberIDs == null)
                return;
            if (EpaperSubscriberIDs.Count() == 0)
                return;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = string.Format("Update EpaperSubscriber Set IsBlack = 1 Where ID in ({0})", string.Join(",", EpaperSubscriberIDs));
                conn.Execute(sql);
            }
        }
        /// <summary>
        /// 產生發送紀錄輸出excel功能所需內容
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsHiddenInfo"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSendRecordExcelModel> SendRecordToExcelModel(IEnumerable<EpaperSendList> source, bool IsHiddenInfo)
        {
            IEnumerable<EpaperSendRecordExcelModel> list;
            if (source == null)
                return new List<EpaperSendRecordExcelModel>();

            list = source.Select(m => new EpaperSendRecordExcelModel()
            {
                Email = IsHiddenInfo ? HiddenEmail(m.EpaperSubscriber.Email) : m.EpaperSubscriber.Email,
                OrderType = m.EpaperSubscriber.IsMember ? "會員" : "一般訂閱者",
                MemberName = IsHiddenInfo ? HiddenName(m.EpaperSubscriber.MemberShip.Name) : m.EpaperSubscriber.MemberShip.Name,
                EpaperTypeName = string.Join(",", m.EpaperSubscriber.Detail.Select(o => o.EpaperTypeName).ToList()),
                SendStatus = GetSendStatusStrForExportExcel(m.IsSended, m.SendTime),
                ReadStatus = GetRealStatusStrForExportExcel(m.IsRead, m.ReadTime)
            });

            return list;
        }
        /// <summary>
        /// 用在Excel當中的發送狀態文字
        /// </summary>
        /// <param name="IsSended"></param>
        /// <param name="SendTime"></param>
        /// <returns></returns>
        static string GetSendStatusStrForExportExcel(bool IsSended, DateTime? SendTime)
        {
            var str = string.Empty;
            if (!IsSended && SendTime != null)
            {
                str = string.Format("發送失敗 {0}", SendTime.ToString("yyyy /MM/dd HH:mm"));
            }
            else if (IsSended)
            {
                str = string.Format("發送成功 {0}", SendTime.ToString("yyyy/MM/dd HH:mm"));
            }
            return str;
        }
        /// <summary>
        /// 用在Excel當中的讀取狀態文字
        /// </summary>
        /// <param name="IsRead"></param>
        /// <param name="ReadTime"></param>
        /// <returns></returns>
        static string GetRealStatusStrForExportExcel(bool IsRead, DateTime? ReadTime)
        {
            var str = string.Empty;
            if (IsRead)
            {
                str = string.Format("讀取成功 {0}", ReadTime.ToString("yyyy/MM/dd HH:mm"));
            }
            return str;
        }
        /// <summary>
        /// 產生訂閱者名單輸出excel功能所需內容
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsHiddenInfo"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperSubscriberExcelModel> SubscriberToExcelModel(IEnumerable<EpaperSubscriber> source, bool IsHiddenInfo)
        {
            List<EpaperSubscriberExcelModel> list = new List<EpaperSubscriberExcelModel>();
            if (source == null)
                return list;

            foreach (var item in source)
            {
                item.GetMemberShip();

                EpaperSubscriberExcelModel data = new EpaperSubscriberExcelModel();
                data.Email = IsHiddenInfo ? HiddenEmail(item.Email) : item.Email;
                data.OrderType = item.IsMember ? "會員" : "一般訂閱者";
                data.MemberName = IsHiddenInfo ? HiddenName(item.MemberShip.Name) : item.MemberShip.Name;
                data.EpaperTypeName = string.Join(",", item.Detail.Select(o => o.EpaperTypeName).ToList());
                data.OrderTime = item.CreateTime.ToString("yyyy/MM/dd hh:mm:ss");

                list.Add(data);
            }

            return list;
        }
        /// <summary>
        /// 產生黑名單輸出excel功能所需內容
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsHiddenInfo"></param>
        /// <returns></returns>
        public static IEnumerable<EpaperBlackListExcelModel> BlackListToExcelModel(IEnumerable<EpaperSubscriber> source, bool IsHiddenInfo)
        {
            List<EpaperBlackListExcelModel> list = new List<EpaperBlackListExcelModel>();
            if (source == null)
                return list;

            foreach (var item in source)
            {
                item.GetMemberShip();

                EpaperBlackListExcelModel data = new EpaperBlackListExcelModel();
                data.Email = IsHiddenInfo ? HiddenEmail(item.Email) : item.Email;
                data.OrderType = item.IsMember ? "會員" : "一般訂閱者";
                data.MemberName = IsHiddenInfo ? HiddenName(item.MemberShip.Name) : item.MemberShip.Name;
                data.EpaperTypeName = string.Join(",", item.Detail.Select(o => o.EpaperTypeName).ToList());
                data.FailureTimes = item.FailureTimes != null ? item.FailureTimes.ToString() : "0";
                data.IsPause = item.IsPause ? "暫停發送" : "未暫停發送";

                list.Add(data);
            }

            return list;
        }
        #endregion

        #region 前台相關
        /// <summary>
        /// 檢查電子報讀取識別碼並變更狀態
        /// </summary>
        /// <param name="key">EpaperSendList.ReadIdentifier</param>
        public static void EpaerReadIdentify(long key)
        {
            if (key == 0)
                return;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string check = "select 1 from EpaperSendList where IsRead=1 and ReadTime is not null and ReadIdentifier=" + key;
                if (conn.Query<int>(check).Count() == 0)
                {
                    string sql = "Update EpaperSendList Set IsRead = 1, ReadTime=getdate() Where ReadIdentifier=" + key;
                    conn.Execute(sql);
                }
            }
        }
        /// <summary>
        ///  檢查電子報讀取識別碼並變更狀態 (需傳入加密KEY)
        /// </summary>
        /// <param name="encryptKey"></param>
        public static void EpaerReadIdentifyByCryption(string encryptKey)
        {
            if (string.IsNullOrEmpty(encryptKey))
                return;

            string decryptString = string.Empty;

            try
            {
                decryptString = decryption(encryptKey);
            }
            catch
            {
                return;
            }

            long key = 0;
            long.TryParse(decryptString, out key);

            if (key == 0)
                return;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string check = "select 1 from EpaperSendList where IsRead=1 and ReadTime is not null and ReadIdentifier=" + key;
                if (conn.Query<int>(check).Count() == 0)
                {
                    string sql = "Update EpaperSendList Set IsRead = 1, ReadTime=getdate() Where ReadIdentifier=" + key;
                    conn.Execute(sql);
                }
            }
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string encryption(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(str);

            rsa.FromXmlString(EpaperReadIdPublicKey);
            byte[] encryptData = rsa.Encrypt(data, false);
            return Convert.ToBase64String(encryptData);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="EncryptDada"></param>
        /// <returns></returns>
        static string decryption(string cryptionStr)
        {
            if (string.IsNullOrWhiteSpace(cryptionStr))
                return string.Empty;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] data = Convert.FromBase64String(cryptionStr);

            rsa.FromXmlString(EpaperReadIdPrivateKey);
            var decrtpyData = rsa.Decrypt(data, false);
            return Encoding.UTF8.GetString(decrtpyData);
        }
        /// <summary>
        ///  取消訂閱 (需傳入加密KEY)
        /// </summary>
        /// <param name="encryptKey"></param>
        public static void EpaerCancelOrderByCryption(string encryptKey, out string msg)
        {
            msg = "取消訂閱成功";

            if (string.IsNullOrEmpty(encryptKey))
                return;

            string decryptString = string.Empty;

            try
            {
                decryptString = decryption(encryptKey);
            }
            catch
            {
                return;
            }

            string[] array = decryptString.Split('|');
            if (array.Length == 2)
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    string check = $"select 1 From EpaperSubscriberDetail where EpaperSubscriber_ID={array[0]} and EpaperType_ID={array[1]}";
                    if (conn.Query<int>(check).Any())
                    {
                        string sql = $"Delete From EpaperSubscriberDetail where EpaperSubscriber_ID={array[0]} and EpaperType_ID={array[1]}";
                        conn.Execute(sql);
                    }
                    else
                        msg = "您已經取消訂閱!";
                }
            }
        }
        #endregion

        //--------------------------------------------
        /// <summary>
        /// Html插入點字串 (我已收到電子報)
        /// </summary>
        public static readonly string insertReceivedClickPoint = "<!--我已收到電子報插入點-->";
        /// <summary>
        /// Html插入點字串 (取消訂閱)
        /// </summary>
        public static readonly string insertCancelOrderClickPoint = "<!--取消訂閱插入點-->";
        /// <summary>
        /// 訂閱電子報偵測點 (start)
        /// </summary>
        public static readonly string SubscribeEpaperStartPoint = "<!-- 訂閱電子報 start -->";
        /// <summary>
        /// Html插入點字串 (取消訂閱)
        /// </summary>
        public static readonly string SubscribeEpaperEndPoint = "<!-- 訂閱電子報 end -->";
        /// <summary>
        /// 電子報讀取識別私鑰XML
        /// </summary>
        static readonly string EpaperReadIdPrivateKey = "<RSAKeyValue><Modulus>qi1DePV6UpHWHjEi/Nc2vLepg2Ns9Wy9ZQP2JAGknBxjc7yizbFA/zGKfTI4WJUk8IWHQMOZA5KnIy5VR/wC3N3c7WFhu6w/1Nh/EughDXIdqDmy/MPHmuj9d1u7iXcX1WRPPVSopnIA/HEyN9vKtrvBNTubNcRl/wbrftEfdwc=</Modulus><Exponent>AQAB</Exponent><P>49y0ZwP4ZNQ2WnOCG4JdYP1DGtn+l1AGw7E6cPOyGmppvSksPt+S9rybz1hYlSimdpW1J2AVlZ4jt+ZI8mXEmQ==</P><Q>vzD7F3VlTsBXPDlaz4QkhiHTlbYkRXNxXczl/bcaCgCuuqqtXDrPGay5OQP1qn90CyUlGxOJViap5J6+EBq8nw==</Q><DP>fihLJaIYG9M2yLudNJfoFXQDfFFn2OTw6dYtMi5q1J9ILfgmzCTC3KMubQ1P9j2MdKmMo+FZ8f2dbwssJjDcOQ==</DP><DQ>GkvsXT+iXYCK/xeMa/pq46AHcBILTxofv50NDSaACFwrrDUJfyP/lDJzj2oCAh9hoJ7NSboYK6dJus4yqhhjVQ==</DQ><InverseQ>JCvccw/sJpheZ62F+rZykrOnvykWTYEJXilv0dxoPTo11f8cXNdze+AhjuqXC19DM4l72SWnMsEMSmK4sotD7g==</InverseQ><D>OZYBjA/9aa2B+EuGXGrzUd8QlK5zc4VtfZ5ej4aizlTo7oQ5z0MZDIIWLBFvccJecqmswCSwRb49orEOGMLVsJvw/y8mlI4OX5mNacmRTwEk7dirG3OKOb6qf57PGFG+tFuN0Yw78TXZHMRkBp5YM+eKuQQw+mGqAk/QBuC7d9E=</D></RSAKeyValue>";
        /// <summary>
        /// 電子報讀取識別公鑰XML
        /// </summary>
        static readonly string EpaperReadIdPublicKey = "<RSAKeyValue><Modulus>qi1DePV6UpHWHjEi/Nc2vLepg2Ns9Wy9ZQP2JAGknBxjc7yizbFA/zGKfTI4WJUk8IWHQMOZA5KnIy5VR/wC3N3c7WFhu6w/1Nh/EughDXIdqDmy/MPHmuj9d1u7iXcX1WRPPVSopnIA/HEyN9vKtrvBNTubNcRl/wbrftEfdwc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    }
}