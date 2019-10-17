using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Dapper;
using WorkV3.Areas.Backend.Models;
namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class IntraIPlimitDAO
    {
        public static bool isAllowIP(string IP)
        {
            var currentSettingList = GetItems(false);
            //未設定半筆資料時, 則以預設全部允許判定
            if (currentSettingList == null || currentSettingList.Count() <= 0)
                return true;

            var defaultSetting = currentSettingList.Where(ip => ip.IsSystemSet);
            //理論上一定有一個預設值, 若沒有, 表示系統有問題, 不給登入
            if(defaultSetting== null || defaultSetting.Count()<=0)
                return false;
            bool IsDefaultOpen = (defaultSetting.First().OpenStatus == OpenStatus.Open);
            var allUserSetting = currentSettingList.Where(ip => !ip.IsSystemSet);
            //除了預設的, 沒有其它的, 那就依預設的
            if (allUserSetting == null || allUserSetting.Count() <= 0)
                return IsDefaultOpen;

            ulong ipNum = WorkLib.GetItem.GetIPNum(IP);
            //WorkLib.WriteLog.Write(true, ipNum.ToString() + "," + IP);
            if (IsDefaultOpen) // 預設為全部開啟
            {
                var allDenySetting = currentSettingList.Where(ip => !ip.IsSystemSet && ip.OpenStatus == OpenStatus.Close) ;
                //沒有設為拒絕的, 那就等於全部通過
                if (allDenySetting == null || allDenySetting.Count() <= 0)
                    return true;
                foreach (IntraIPlimitModel item in allDenySetting)
                {
                    if (item.IP_BeginNum.HasValue && item.IP_EndNum.HasValue)
                    {
                        //處於被拒絕的範圍內
                        if ((ulong)item.IP_BeginNum.Value <= ipNum && (ulong)item.IP_EndNum.Value >= ipNum)
                        {
                            return false;
                        }
                    }
                    else if (item.IP_BeginNum.HasValue) // 只設一筆, 那就直接判定
                    {
                        if ((ulong)item.IP_BeginNum.Value == ipNum)
                        {
                            return false;
                        }
                    }
                    else if (item.IP_EndNum.HasValue) // 只設一筆, 那就直接判定
                    {
                        if ((ulong)item.IP_EndNum.Value == ipNum)
                        {
                            return false;
                        }
                    }
                }
            }
            else // 預設為全部拒絕
            {
                var allAllowSetting = currentSettingList.Where(ip => !ip.IsSystemSet && ip.OpenStatus == OpenStatus.Open);
                //沒有設為允許的, 那就等於全部拒絕
                if (allAllowSetting == null || allAllowSetting.Count() <= 0)
                    return true;
                foreach (IntraIPlimitModel item in allAllowSetting)
                {
                    if (item.IP_BeginNum.HasValue && item.IP_EndNum.HasValue)
                    {
                        //處於被允許的範圍內
                        if ((ulong)item.IP_BeginNum.Value <= ipNum && (ulong)item.IP_EndNum.Value >= ipNum)
                        {
                            return true;
                        }
                    }
                    else if (item.IP_BeginNum.HasValue) // 只設一筆, 那就直接判定
                    {
                        if ((ulong)item.IP_BeginNum.Value == ipNum)
                        {
                            return true;
                        }
                    }
                    else if (item.IP_EndNum.HasValue) // 只設一筆, 那就直接判定
                    {
                        if ((ulong)item.IP_EndNum.Value == ipNum)
                        {
                            return true;
                        }
                    }
                }

            }
            return IsDefaultOpen;
        }
        /// <summary>
        /// 檢查資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<string> CheckInsertData(IntraIPlimitModel model)
        {
            model.IP_Begin = model.IP_Begin ?? string.Empty;
            model.IP_End = model.IP_End ?? string.Empty;

            Regex rgx = new Regex(@"^(25[0-5]|2[0-4][0-9]|1?[0-9]{1,2})(.(25[0-5]|2[0-4][0-9]|1?[0-9]{1,2})){3}$");
            List<string> msg = new List<string>();

            if (!rgx.IsMatch(model.IP_Begin))
                msg.Add("起始IP錯誤");
            if (!rgx.IsMatch(model.IP_End))
                msg.Add("結束IP錯誤");

            return msg;
        }

        public static void InsertData(OpenStatus openStatus, string sIP, string eIP, bool IsSystemSet)
        {

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"IF Not EXISTS (SELECT 1 FROM IntraIPlimit WHERE IP_Begin=@IP_Begin and IP_End=@IP_End ) " +
                            " INSERT INTO IntraIPlimit(OpenStatus, IP_Begin, IP_End, IP_BeginNum, IP_EndNum , Creator , CreateTime, IsSystemSet) VALUES(@OpenStatus, @IP_Begin, @IP_End, @IP_BeginNum, @IP_EndNum , @Creator , @CreateTime, @IsSystemSet)";

                string IP_BeginNum = WorkLib.GetItem.GetIPNum(sIP).ToString();
                string IP_EndNum = "0";
                if (!string.IsNullOrEmpty(eIP))
                {
                    IP_EndNum = WorkLib.GetItem.GetIPNum(eIP).ToString();
                }
                long Creator = MemberDAO.SysCurrent.Id;
                DateTime CreateTime = DateTime.Now;

                conn.Execute(sql, new { OpenStatus= (int)openStatus, IP_Begin = sIP, IP_End = eIP, IP_BeginNum = IP_BeginNum, IP_EndNum = IP_EndNum, Creator = Creator, CreateTime = CreateTime, IsSystemSet= IsSystemSet });
            }
        }
        public static IntraIPlimitModel GetItem(long Id)
        {
            return CommonDA.GetItem<IntraIPlimitModel>("IntraIPlimit", Id);
        }
        public static IEnumerable<IntraIPlimitModel> GetItems(bool IsAddInit = true)
        {
            List<IntraIPlimitModel> items = new List<IntraIPlimitModel>();
            

            string sql = "Select * From [IntraIPlimit] Order By IsSystemSet DESC, IP_Begin Asc";

            List<string> where = new List<string>();

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (IsAddInit)
            {
                if (datas.Rows.Count <= 0) // 若都是空的, 自動加入全域的一筆, 本筆不可刪除
                {
                    IntraIPlimitModel initModel = new IntraIPlimitModel();
                    initModel.OpenStatus = OpenStatus.Open;
                    initModel.IP_Begin = "*";
                    initModel.IP_End = "*";
                    initModel.IP_BeginNum = 0;
                    initModel.IP_EndNum = 0;
                    initModel.CreateTime = DateTime.Now;
                    initModel.Creator = MemberDAO.SysCurrent.Id;
                    initModel.IsSystemSet = true;
                    SetItem(initModel);
                    datas = db.GetDataTable(sql);
                }
            }
            foreach (DataRow dr in datas.Rows)
            {
                IntraIPlimitModel item = new IntraIPlimitModel();
                item.ID = int.Parse(dr["ID"].ToString());
                item.OpenStatus = (OpenStatus)(int.Parse(dr["OpenStatus"].ToString()));
                item.IP_Begin = dr["IP_Begin"].ToString().Trim();
                item.IP_End = dr["IP_End"].ToString().Trim();
                item.IP_BeginNum = long.Parse(dr["IP_BeginNum"].ToString());
                item.IP_EndNum = long.Parse(dr["IP_EndNum"].ToString());
                item.Creator = long.Parse(dr["Creator"].ToString());
                item.CreateTime = DateTime.Parse(dr["CreateTime"].ToString());
                item.Modifier = string.IsNullOrEmpty(dr["Modifier"].ToString()) ? (long?)null : long.Parse(dr["Modifier"].ToString());
                item.ModifyTime = string.IsNullOrEmpty(dr["ModifyTime"].ToString()) ? (DateTime?)null : DateTime.Parse(dr["ModifyTime"].ToString());
                item.IsSystemSet = bool.Parse(dr["IsSystemSet"].ToString());
                items.Add(item);
            }

            return items;
        }

        public static void SetItem(IntraIPlimitModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("IntraIPlimit");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From IntraIPlimit Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["OpenStatus"] = item.OpenStatus == OpenStatus.Open ? 1 : 0;
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
        public static int Delete(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string sql =
                "Delete IntraIPlimit Where ID In ({0})\n" ;

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }
            return num;
        }
    }
}