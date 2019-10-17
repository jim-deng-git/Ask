using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class MemberShipRegSetDAO
    {
        public static MemberShipRegSetModels GetItem(long SiteID)
        {
            string sql = $"SELECT * FROM MemberShipRegSet WHERE  SiteID={SiteID} ";

            DateTime now = DateTime.Now;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas==null || datas.Rows.Count<=0)
            {
                if (Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent != null)
                {
                    MemberShipRegSetModels m = new MemberShipRegSetModels();
                    List<MemberShipLoginType> loginTypeList = new List<MemberShipLoginType>();
                    m.SiteID = SiteID;
                    m.IsOpenReg = false;
                    m.RegType = Areas.Backend.Models.MemberShipRegType.All; // default 
                    m.VerifyType = Areas.Backend.Models.MemberShipVerifyType.None; // default 
                    m.LoginType = "0"; // default 
                    string[] loginTypes = m.LoginType.Split(';');
                    foreach (string loginType in loginTypes)
                    {
                        if (!string.IsNullOrEmpty(loginType))
                        {
                            MemberShipLoginType lType = (MemberShipLoginType)int.Parse(loginType);
                            if (!loginTypeList.Contains(lType))
                                loginTypeList.Add(lType);
                        }
                    }
                    m.LoginTypeList = loginTypeList;
                    m.IsNeedAgreeMemberDesc = false; // default 
                    m.IsNeedVerifyCode = false; // default 
                    m.IsAutoEmail = false; // default 
                    m.AutoEmailManagers = ""; // default 
                    m.Modifier = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                    m.ModifyTime = now;
                    m.Creator = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                    m.CreateTime = now;
                    m.Modifier = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                    m.ModifyTime = now;
                    m.BackendIsOpen = false;
                    SQLData.TableObject tableObj = db.GetTableObject("MemberShipRegSet");
                    tableObj["SiteID"] = m.SiteID;
                    tableObj["IsOpenReg"] = m.IsOpenReg; // default 
                    tableObj["RegType"] = (int)m.RegType; // default 
                    tableObj["VerifyType"] = (int)m.VerifyType; // default 
                    tableObj["LoginType"] = m.LoginType; // default 
                    tableObj["IsNeedAgreeMemberDesc"] = m.IsNeedAgreeMemberDesc; // default 
                    tableObj["IsNeedVerifyCode"] = m.IsNeedVerifyCode; // default 
                    tableObj["IsAutoEmail"] = m.IsAutoEmail; // default 
                    tableObj["AutoEmailManagers"] = m.AutoEmailManagers; // default 
                    tableObj["BackendIsOpen"] = m.BackendIsOpen; // default 
                    tableObj["Creator"] = m.Creator;
                    tableObj["CreateTime"] = m.CreateTime;
                    tableObj["Modifier"] = m.Modifier;
                    tableObj["ModifyTime"] = m.ModifyTime;
                    tableObj.Insert();
                }
                else
                    return null;
            }
            else
            {
                DataRow dr = datas.Rows[0];
                List<MemberShipLoginType> loginTypeList = new List<MemberShipLoginType>();
                MemberShipRegSetModels m = new MemberShipRegSetModels();
                m.SiteID = (long)dr["SiteID"];
                m.IsOpenReg = (bool)dr["IsOpenReg"];
                m.RegType = (Areas.Backend.Models.MemberShipRegType)((int)dr["RegType"]);
                m.VerifyType = (Areas.Backend.Models.MemberShipVerifyType)((int)dr["VerifyType"]);
                m.LoginType = dr["LoginType"].ToString();
                string[] loginTypes = m.LoginType.Split(';');
                m.AddFriend = dr["AddFriend"].ToString();

                foreach (string loginType in loginTypes)
                {
                    if (!string.IsNullOrEmpty(loginType))
                    {
                        MemberShipLoginType lType = (MemberShipLoginType)int.Parse(loginType);
                        if(!loginTypeList.Contains(lType))
                            loginTypeList.Add(lType);
                    }
                }
                m.LoginTypeList = loginTypeList;
                m.RegColumnSets = GetColumnItems(SiteID);
                m.RegSocialSets = GetSocialItems(SiteID);
                m.IsNeedAgreeMemberDesc = (bool)dr["IsNeedAgreeMemberDesc"];
                m.IsNeedVerifyCode = (bool)dr["IsNeedVerifyCode"];
                m.IsAutoEmail = (bool)dr["IsAutoEmail"];
                m.AutoEmailManagers = dr["AutoEmailManagers"].ToString();
                m.Modifier = (long)dr["Modifier"]; 
                m.ModifyTime = (DateTime )dr["ModifyTime"]; 

                return m;
            }

            return null;
        }
        public static bool UpdateItemSet(long SiteID, string columnName, string columnValue)
        {
            string sql = $"UPDATE MemberShipRegSet SET {columnName}='{columnValue.Replace("'","")}' WHERE  SiteID={SiteID} ";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            if (columnName == "IsNeedAgreeMemberDesc")
            {
                AgreeStatementSetModels agreeState = MemberShipSettingDAO.GetStatementItems();
                if (agreeState != null)
                {
                    agreeState.IsNeedAgree = Convert.ToBoolean(columnValue);
                    MemberShipSettingDAO.SetStatementItems(agreeState);
                }
            }
            return true;
        }
        public static List<MemberShipRegSocialSetModels> GetSocialItems(long SiteID)
        {
            List<MemberShipRegSocialSetModels> items = new List<MemberShipRegSocialSetModels>();

            string sql = $"SELECT * FROM MemberShipRegSocialSet WHERE SiteID={SiteID} And BackendIsOpen = 1 ORDER BY Sort ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas == null || datas.Rows.Count <= 0)
            {
                Dictionary<WorkV3.Models.MemberType, string> DefaultSocialList = new Dictionary<WorkV3.Models.MemberType, string>();
                DefaultSocialList.Add(WorkV3.Models.MemberType.FB, "Facebook");
                //DefaultSocialList.Add(WorkV3.Models.MemberType.Twitter, "Twitter"); Joe 20190930 尚無功能
                DefaultSocialList.Add(WorkV3.Models.MemberType.Google, "Google");
                //DefaultSocialList.Add(WorkV3.Models.MemberType.Yahoo, "Yahoo");
                //DefaultSocialList.Add(WorkV3.Models.MemberType.Weibo, "微博");
                //DefaultSocialList.Add(WorkV3.Models.MemberType.QQ, "QQ");
                //DefaultSocialList.Add(WorkV3.Models.MemberType.Baidu, "百度");
                int itemSort = 0;
                foreach (WorkV3.Models.MemberType defaultSocialKey in DefaultSocialList.Keys)
                {
                    itemSort++;
                    MemberShipRegSocialSetModels m = GetSocialItem(SiteID, itemSort, defaultSocialKey, DefaultSocialList[defaultSocialKey]);
                    items.Add(m);
                }
                return items;
            }
            else
            {
                foreach (DataRow dr in datas.Rows)
                {
                    MemberShipRegSocialSetModels m = new MemberShipRegSocialSetModels();
                    m.SiteID = (long)dr["SiteID"];
                    m.SocialType = (WorkV3.Models.MemberType)((int)dr["SocialType"]);
                    m.SocialTitle = dr["SocialTitle"].ToString().Trim();
                    m.IsOpen = Convert.ToBoolean(dr["IsOpen"].ToString());
                    m.Sort = Convert.ToInt32(dr["Sort"].ToString());
                    m.SecretKey = dr["SecretKey"].ToString().Trim();
                    m.AppID = dr["AppID"].ToString().Trim();
                    m.Scope = dr["Scope"].ToString().Trim();
                    m.Creator = (long)dr["Creator"];
                    m.CreateTime = (DateTime)dr["CreateTime"];
                    m.Modifier = (long)dr["Modifier"];
                    m.ModifyTime = (DateTime)dr["ModifyTime"];
                    items.Add(m);
                }
            }
            return items;
        }

        public static MemberShipRegSocialSetModels GetSocialItem(long SiteID, int Sort, WorkV3.Models.MemberType SocialType, string SocialTitle)
        {
            string sql = $"SELECT * FROM MemberShipRegSocialSet WHERE SiteID={SiteID} AND SocialType={(int)SocialType}  ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            DateTime now = DateTime.Now;
            if (datas == null || datas.Rows.Count <= 0)
            {
                MemberShipRegSocialSetModels m = new MemberShipRegSocialSetModels();
                m.SiteID = SiteID;
                m.SocialType = SocialType;
                m.SocialTitle = SocialTitle;
                m.Sort = Sort;
                m.IsOpen = false; // default 
                m.SecretKey = "";
                m.AppID = "";
                m.Scope = "";
                m.Creator = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                m.CreateTime = now;
                m.Modifier = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                m.ModifyTime = now;
                m.BackendIsOpen = false;
                SQLData.TableObject tableObj = db.GetTableObject("MemberShipRegSocialSet");
                tableObj["SiteID"] = m.SiteID;
                tableObj["SocialType"] = (int)m.SocialType;
                tableObj["SocialTitle"] = m.SocialTitle;
                tableObj["IsOpen"] = m.IsOpen;
                tableObj["SecretKey"] = m.SecretKey;
                tableObj["AppID"] = m.AppID;
                tableObj["Scope"] = m.Scope;
                tableObj["Sort"] = m.Sort;
                tableObj["Creator"] = m.Creator;
                tableObj["CreateTime"] = m.CreateTime;
                tableObj["Modifier"] = m.Modifier;
                tableObj["ModifyTime"] = m.ModifyTime;
                tableObj["BackendIsOpen"] = m.BackendIsOpen;
                tableObj.Insert();
                return m;
            }
            else
            {
                DataRow dr = datas.Rows[0];
                MemberShipRegSocialSetModels m = new MemberShipRegSocialSetModels();
                m.SiteID = (long)dr["SiteID"];
                m.SocialType = (WorkV3.Models.MemberType)((int)dr["SocialType"]);
                m.SocialTitle = dr["SocialTitle"].ToString();
                m.IsOpen = Convert.ToBoolean(dr["IsOpen"].ToString());
                m.SecretKey = dr["SecretKey"].ToString();
                m.AppID = dr["AppID"].ToString();
                m.Scope = dr["Scope"].ToString();
                m.Sort = (int)dr["Sort"];
                m.Creator = (long)dr["Creator"];
                m.CreateTime = (DateTime)dr["CreateTime"];
                m.Modifier = (long)dr["Modifier"];
                m.ModifyTime = (DateTime)dr["ModifyTime"];
                return m;
            }
        }
        public static MemberShipRegSocialSetModels GetSocialItem(long SiteID, WorkV3.Models.MemberType SocialType)
        {
            string sql = $"SELECT * FROM MemberShipRegSocialSet WHERE SiteID={SiteID} AND SocialType={(int)SocialType}  ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            DateTime now = DateTime.Now;
            if (datas == null || datas.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                DataRow dr = datas.Rows[0];
                MemberShipRegSocialSetModels m = new MemberShipRegSocialSetModels();
                m.SiteID = (long)dr["SiteID"];
                m.SocialType = (WorkV3.Models.MemberType)((int)dr["SocialType"]);
                m.SocialTitle = dr["SocialTitle"].ToString();
                m.IsOpen = Convert.ToBoolean(dr["IsOpen"].ToString());
                m.SecretKey = dr["SecretKey"].ToString();
                m.AppID = dr["AppID"].ToString();
                m.Scope = dr["Scope"].ToString();
                m.Sort = (int)dr["Sort"];
                m.Creator = (long)dr["Creator"];
                m.CreateTime = (DateTime)dr["CreateTime"];
                m.Modifier = (long)dr["Modifier"];
                m.ModifyTime = (DateTime)dr["ModifyTime"];
                return m;
            }
        }
        public static bool UpdateColumnItemSet(long SiteID, string columnName, bool isOpen, bool isNeedValue)
        {
            string isOpenSet = "0", isNeedValueSet = "0";
            isOpenSet = isOpen ? "1" : "0";
            isNeedValueSet = isNeedValue ? "1" : "0";
            string sql = $"UPDATE MemberShipRegColumnSet SET IsOpen={isOpenSet}, IsNeedValue={isNeedValueSet}  WHERE  SiteID={SiteID} AND ColumnName='{columnName.Replace("'", "")}'";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            return true;
        }
        public static bool UpdateColumnItemOptionSet(long SiteID, string columnName, string otherOption)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@SiteID", SiteID);
            paraList.Add("@ColumnName", columnName);
            paraList.Add("@OtherOption", otherOption);
            string sql = $"UPDATE MemberShipRegColumnSet SET OtherOption=@OtherOption WHERE  SiteID=@SiteID AND ColumnName=@ColumnName ";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql, paraList);
            return true;
        }
        public static bool UpdateColumnItemSort(long SiteID, string columnName, int sort)
        {
            string sql = $"UPDATE MemberShipRegColumnSet SET Sort={sort} WHERE  SiteID={SiteID} AND ColumnName='{columnName.Replace("'", "")}';";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            return true;
        }
        public static List<MemberShipRegColumnSetModels> GetColumnItems(long SiteID)
        {
            string sql = $"SELECT * FROM MemberShipRegColumnSet WHERE SiteID={SiteID} ORDER BY Sort ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas == null || datas.Rows.Count <= 0)
            {                
                Dictionary<string, string> DefaultColumnList = new Dictionary<string, string>();
                DefaultColumnList.Add("Photo", "頭圖");
                DefaultColumnList.Add("Name", "姓名");
                DefaultColumnList.Add("Sex", "性別");
                DefaultColumnList.Add("IDCard", "身分證字號");
                DefaultColumnList.Add("Birthday", "出生年月日");
                DefaultColumnList.Add("Phone", "電話");
                DefaultColumnList.Add("Mobile", "手機");
                DefaultColumnList.Add("Email", "Email");
                DefaultColumnList.Add("Address", "地址");
                DefaultColumnList.Add("Marriage", "婚姻狀況");
                DefaultColumnList.Add("Education", "學歷");
                DefaultColumnList.Add("Career", "職業");
                DefaultColumnList.Add("Company", "公司名稱");
                DefaultColumnList.Add("EmergencyName", "緊急聯絡人姓名");
                DefaultColumnList.Add("EmergencyMobile", "緊急聯絡人手機");
                DefaultColumnList.Add("EmergencyPhone", "緊急聯絡人電話");
                DefaultColumnList.Add("EmergencyEmail", "緊急聯絡人Email");
                DefaultColumnList.Add("Identity", "您的身分");
                DefaultColumnList.Add("Favority", "您的喜好");
                DefaultColumnList.Add("OrderEpaper", "訂閱電子報");
                DefaultColumnList.Add("SerialNumber", "會員序號");
                DefaultColumnList.Add("PermanentAddress", "戶籍地址");
                DefaultColumnList.Add("PushRecommandCode", "推薦人");
                DefaultColumnList.Add("PushRecommandMobile", "推薦人手機");
                DefaultColumnList.Add("LegalPersonName", "法定代理人姓名");
                DefaultColumnList.Add("LegalPersonMobile", "法定代理人手機");
                DefaultColumnList.Add("Point", "您的點數");
                List<MemberShipRegColumnSetModels> items = new List<MemberShipRegColumnSetModels>();

                int itemSort = 0;
                foreach (string defaultColumnKy in DefaultColumnList.Keys)
                {
                    itemSort++;
                    MemberShipRegColumnSetModels m = GetColumnItem(SiteID, itemSort, defaultColumnKy, DefaultColumnList[defaultColumnKy]);
                    items.Add(m);
                }
                return items;
            }
            else
            {
                List<MemberShipRegColumnSetModels> items = new List<MemberShipRegColumnSetModels>();
                foreach (DataRow dr in datas.Rows)
                {
                    List<MemberShipRegOptionModels> optionList = null;
                    if(!string.IsNullOrEmpty(dr["OtherOption"].ToString().Trim()))
                        optionList =  Newtonsoft.Json.JsonConvert.DeserializeObject<List<MemberShipRegOptionModels>>(dr["OtherOption"].ToString().Trim());
                    
                    MemberShipRegColumnSetModels m = new MemberShipRegColumnSetModels();
                    m.SiteID = (long)dr["SiteID"];
                    m.TableName = dr["TableName"].ToString().Trim();
                    m.ColumnName = dr["ColumnName"].ToString().Trim();
                    m.ColumnTitle = dr["ColumnTitle"].ToString().Trim();
                    m.IsOpen = Convert.ToBoolean(dr["IsOpen"].ToString());
                    m.IsNeedValue = Convert.ToBoolean(dr["IsNeedValue"].ToString());
                    m.OtherOption = dr["OtherOption"].ToString().Trim();
                    m.OtherOptionList = optionList;
                    m.Sort = (int)dr["Sort"];
                    m.Creator = (long)dr["Creator"];
                    m.CreateTime = (DateTime)dr["CreateTime"];
                    m.Modifier = (long)dr["Modifier"];
                    m.ModifyTime = (DateTime)dr["ModifyTime"];
                    items.Add(m);
                }
                return items;
            }
        }
        public static MemberShipRegColumnSetModels GetColumnItem(long SiteID, int Sort, string ColumnName, string ColumnTitle)
        {
            string tableName = "MemberShip";
            string sql = $"SELECT * FROM MemberShipRegColumnSet WHERE SiteID={SiteID} AND TableName='{tableName}' AND ColumnName='{ColumnName.Replace("'", "")}' ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            DateTime now = DateTime.Now;
            if (datas == null || datas.Rows.Count <= 0)
            {
                string defaultOtherOption = "";
                if (ColumnName == "Address")
                {
                    List<MemberShipRegOptionModels> optionList = new List<MemberShipRegOptionModels>();
                    optionList.Add(new MemberShipRegOptionModels() {
                        Text = "全球",
                        Value = "Global",
                        Selected = false
                    });
                    optionList.Add(new MemberShipRegOptionModels()
                    {
                        Text = "台灣",
                        Value = "Taiwan",
                        Selected = true
                    });
                    defaultOtherOption = Newtonsoft.Json.JsonConvert.SerializeObject(optionList);
                }
                MemberShipRegColumnSetModels m = new MemberShipRegColumnSetModels();
                m.SiteID = SiteID;
                m.TableName = tableName;
                m.ColumnName = ColumnName;
                m.ColumnTitle = ColumnTitle;
                m.Sort = Sort;
                m.IsOpen = false; // default 
                m.IsNeedValue = false; // default 
                m.OtherOption = defaultOtherOption; // default 
                m.Creator = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                m.CreateTime = now;
                m.Modifier = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                m.ModifyTime = now;
                SQLData.TableObject tableObj = db.GetTableObject("MemberShipRegColumnSet");
                tableObj["SiteID"] = m.SiteID;
                tableObj["TableName"] = m.TableName;
                tableObj["ColumnName"] = m.ColumnName;
                tableObj["ColumnTitle"] = m.ColumnTitle;
                tableObj["IsOpen"] = m.IsOpen;
                tableObj["IsNeedValue"] = m.IsNeedValue;
                tableObj["OtherOption"] = m.OtherOption;
                tableObj["Sort"] = m.Sort;
                tableObj["Creator"] = m.Creator;
                tableObj["CreateTime"] = m.CreateTime;
                tableObj["Modifier"] = m.Modifier;
                tableObj["ModifyTime"] = m.ModifyTime;
                tableObj.Insert();
                return m;
            }
            else
            {
                DataRow dr = datas.Rows[0];
                MemberShipRegColumnSetModels m = new MemberShipRegColumnSetModels();
                m.SiteID = (long)dr["SiteID"];
                m.TableName = dr["TableName"].ToString().Trim();
                m.ColumnName = dr["ColumnName"].ToString().Trim();
                m.ColumnTitle = dr["ColumnTitle"].ToString().Trim();
                m.IsOpen = Convert.ToBoolean(dr["IsOpen"].ToString());
                m.IsNeedValue = Convert.ToBoolean(dr["IsNeedValue"].ToString());
                m.OtherOption = dr["OtherOption"].ToString().Trim();
                if (!string.IsNullOrEmpty(m.OtherOption))
                    m.OtherOptionList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MemberShipRegOptionModels>>(m.OtherOption);
                m.Sort = (int)dr["Sort"];
                m.Creator = (long)dr["Creator"];
                m.CreateTime = (DateTime)dr["CreateTime"];
                m.Modifier = (long)dr["Modifier"];
                m.ModifyTime = (DateTime)dr["ModifyTime"];
                return m;
            }
        }
        public static MemberShipRegColumnSetModels GetColumnItem(long SiteID, string ColumnName)
        {
            string tableName = "MemberShip";
            string sql = $"SELECT * FROM MemberShipRegColumnSet WHERE SiteID={SiteID} AND TableName='{tableName}' AND ColumnName='{ColumnName.Replace("'", "")}' ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            DateTime now = DateTime.Now;
            if (datas == null || datas.Rows.Count <= 0)
                return null;
            else
            {
                DataRow dr = datas.Rows[0];
                MemberShipRegColumnSetModels m = new MemberShipRegColumnSetModels();
                m.SiteID = (long)dr["SiteID"];
                m.TableName = dr["TableName"].ToString().Trim();
                m.ColumnName = dr["ColumnName"].ToString().Trim();
                m.ColumnTitle = dr["ColumnTitle"].ToString().Trim();
                m.IsOpen = Convert.ToBoolean(dr["IsOpen"].ToString());
                m.IsNeedValue = Convert.ToBoolean(dr["IsNeedValue"].ToString());
                m.OtherOption = dr["OtherOption"].ToString().Trim();
                if (!string.IsNullOrEmpty(m.OtherOption))
                    m.OtherOptionList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MemberShipRegOptionModels>>(m.OtherOption);
                m.Sort = (int)dr["Sort"];
                m.Creator = (long)dr["Creator"];
                m.CreateTime = (DateTime)dr["CreateTime"];
                m.Modifier = (long)dr["Modifier"];
                m.ModifyTime = (DateTime)dr["ModifyTime"];
                return m;
            }
        }

        public static void DelMemberShipRegManagers(long SiteID)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@SiteID", SiteID);
            string sql = $"DELETE MemberShipRegEmailManagers  WHERE  SiteID=@SiteID AND IsManager=1 ";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql, paraList);
        }
        public static MemberShipRegEmailManagersModels GetMemberShipRegManagers(long SiteID, long ManagerID)
        {
            string sql = $"SELECT * FROM MemberShipRegEmailManagers WHERE  SiteID={SiteID} AND ManagerID={ManagerID}";

            DateTime now = DateTime.Now;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                MemberShipRegEmailManagersModels m = new MemberShipRegEmailManagersModels();
                m.ID = datas.Rows[0]["ID"].ToString();
                m.SiteID = SiteID;
                m.IsManager = true;
                m.ManagerID = datas.Rows[0]["ManagerID"].ToString();
                m.Email = datas.Rows[0]["Email"].ToString();
                m.Sort = (int)datas.Rows[0]["Sort"];
                return m;
            }

            return null;
        }
        public static MemberShipRegEmailManagersModels GetMemberShipRegManagerEmail(long SiteID, string Email)
        {
            string sql = $"SELECT * FROM MemberShipRegEmailManagers WHERE  SiteID={SiteID} AND Email='{Email}'";

            DateTime now = DateTime.Now;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                MemberShipRegEmailManagersModels m = new MemberShipRegEmailManagersModels();
                m.ID = datas.Rows[0]["ID"].ToString();
                m.SiteID = SiteID;
                m.IsManager = false;
                m.ManagerID = "";
                m.Email = datas.Rows[0]["Email"].ToString();
                m.Sort = (int)datas.Rows[0]["Sort"];
                return m;
            }

            return null;
        }
        public static bool DeleteManager(long ID)
        {
            string sql = $"DELETE MemberShipRegEmailManagers WHERE  ID={ID} ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            return true;
        }
        public static List<MemberShipRegEmailManagersModels> GetMemberShipRegManagers(long SiteID)
        {
            List<MemberShipRegEmailManagersModels> modelList = new List<MemberShipRegEmailManagersModels>();
            string sql = $"SELECT * FROM MemberShipRegEmailManagers WHERE  SiteID={SiteID} ";

            DateTime now = DateTime.Now;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                for (int i = 0; i < datas.Rows.Count; i++)
                {
                    MemberShipRegEmailManagersModels m = new MemberShipRegEmailManagersModels();
                    m.ID = datas.Rows[i]["ID"].ToString();
                    m.SiteID = SiteID;
                    m.IsManager = (bool)datas.Rows[i]["IsManager"];
                    m.ManagerID = datas.Rows[i]["ManagerID"].ToString();
                    m.Email = datas.Rows[i]["Email"].ToString();
                    m.Sort = (int)datas.Rows[i]["Sort"];
                    if (m.IsManager)
                    {
                      MemberModels mem=  Models.DataAccess.ManagerDAO.GetItem(long.Parse(m.ManagerID));
                        if (mem != null)
                        {
                            m.ManagerName = mem.Name;
                            m.Email = mem.Email;
                        }
                    }
                    modelList.Add(m);
                }
            }
            return modelList;
        }
        public static void SetMemberShipRegManagers(MemberShipRegEmailManagersModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("MemberShipRegEmailManagers");
            tableObj.GetDataFromObject(item);
            string sql = $"Select 1 From MemberShipRegEmailManagers Where SiteID={item.SiteID} AND ManagerID='{item.ManagerID}'";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                if (string.IsNullOrEmpty(item.ManagerID))
                    tableObj["ManagerID"] = "";
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        public static bool UpdateSocialItemSet(long SiteID, WorkV3.Models.MemberType socialType, bool isOpen)
        {
            string isOpenSet = "0";
            isOpenSet = isOpen ? "1" : "0";
            string sql = $"UPDATE MemberShipRegSocialSet SET IsOpen={isOpenSet}  WHERE  SiteID={SiteID} AND SocialType={(int)socialType}";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            return true;
        }
        public static bool UpdateSocialItemSet(long SiteID, WorkV3.Models.MemberType socialType, string SecretKey, string AppID, string Scope, bool isOpen)
        {
            string sql = $"UPDATE MemberShipRegSocialSet SET IsOpen=@IsOpen, SecretKey=@SecretKey, AppID=@AppID, Scope=@Scope  WHERE  SiteID=@SiteID AND SocialType=@SocialType";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@SiteID", SiteID);
            paraList.Add("@SocialType", (int)socialType);
            paraList.Add("@IsOpen", isOpen);
            paraList.Add("@SecretKey", SecretKey);
            paraList.Add("@AppID", AppID);
            paraList.Add("@Scope", Scope);
            db.ExecuteNonQuery(sql, paraList);
            return true;
        }
        public static bool UpdateSocialItemSort(long SiteID, WorkV3.Models.MemberType socialType, int sort)
        {
            string sql = $"UPDATE MemberShipRegSocialSet SET Sort={sort} WHERE  SiteID={SiteID} AND SocialType={(int)socialType};";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
            return true;
        }
    }
}