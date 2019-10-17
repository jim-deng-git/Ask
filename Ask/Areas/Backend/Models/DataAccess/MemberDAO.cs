using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using WorkV3.Areas.Backend.Models;
using System.Data.SqlClient;
using Dapper;
using WorkLib;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class MemberDAO
    {



        #region LoginCheck

        #region 後台
        
        public static MemberModels SysCurrent
        {
            get
            {
                return HttpContext.Current.Session[WebInfo.SysMemSkey] as MemberModels;
            }
        }

        public static MemberModels GetItem(long memberId)
        {
            return CommonDA.GetItem<MemberModels>("Member", memberId);
        }

        public static IEnumerable<MemberModels> GetItems()
        {
            return CommonDA.GetAllItem<MemberModels>("Member");
        }

        public static MemberModels Current(string sessionId)
        {
            return HttpContext.Current.Session[sessionId] as MemberModels;
        }
        public static void SysLogout()
        {
            string sessionId = WorkLib.uCookie.Read("sessionId");
            if (WorkLib.uCookie.Read("sessionId") != "")
            {
                HttpContext.Current.Session.Remove(sessionId);
                HttpContext.Current.Session.Remove(WebInfo.SysMemSkey);            
              
            }else
            {
                HttpContext.Current.Session.RemoveAll();
            }         
          
        }

        public static IEnumerable<MemberModels> GetSupremeAuthorityUsers()
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = " SELECT * FROM Member WHERE IsSupremeAuthority = 1 ";

                return conn.Query<MemberModels>(sql);
            }
        }


        //public static MemberModels.LoginStatus SysLoginCheck(string LoginID, string PassWord)
        //{
        //    //DENISE 待補

        //    string sql = "isSysOnly != false";


        //    MemberModels member = new MemberModels
        //    {
        //        Id = 510,
        //        isSysOnly = true,
        //        MemName = "Denise",
        //        Email = "denise_wu@kidtech.com.tw"
        //    };

        //    HttpContext.Current.Session[WebInfo.SysMemSkey] = member;
        //    return MemberModels.LoginStatus.Success;

        //}

        #region Login Check
        // 20180516 neil 新增選擇攔位 IsChangedPassword, IsSupremeAuthority
        public static string LoginCheck(string LoginID, string PassWord)
        {
            string sql = "select * from Member where LoginID='{0}'";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
           
            DataTable dt = db.GetDataTable(string.Format(sql, LoginID, PassWord));
            if (dt.Rows.Count > 0 && dt.Rows[0] != null)
            {
                string Pwd = (string)dt.Rows[0]["Password"];
                string Status = dt.Rows[0]["MStatus"].ToString();
                if (Status == "0")
                {
                    if (Pwd == PassWord)
                    {
                        MemberModels member = new MemberModels
                        {
                            Id = (long)dt.Rows[0]["id"],
                            isSysOnly = (bool)dt.Rows[0]["isSysOnly"],
                            LoginID = dt.Rows[0]["LoginID"].ToString(),
                            Name = dt.Rows[0]["Name"].ToString(),
                            Img = dt.Rows[0]["Img"].ToString(),
                            GroupId = (long)dt.Rows[0]["GroupId"],
                            Email = dt.Rows[0]["Email"].ToString(),
                            IsChangedPassword = dt.Rows[0]["IsChangedPassword"] == null ? false : (bool)dt.Rows[0]["IsChangedPassword"],
                            IsSupremeAuthority = dt.Rows[0]["IsSupremeAuthority"] == null ? false : (bool)dt.Rows[0]["IsSupremeAuthority"],
                        };

                        String key = System.Guid.NewGuid().ToString();
                        HttpContext.Current.Session[key] = member;
                        HttpContext.Current.Session[WebInfo.SysMemSkey] = member;
                        return key;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    string isSuspension = "isSuspension";//20190917 Joe 停權判斷
                    return isSuspension;
                }

            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Get Member by Session
        public static MemberModels GetMemberBySession(string sessionId)
        {
            MemberModels member = (MemberModels)HttpContext.Current.Session[sessionId];

            return member;
        }
        #endregion

        #region 後台權限
        public static bool CheckPermission(long PageId, long GroupId)
        {
            if (GroupId == 1 || GroupId == 2)
            {
                return true;
            }
            string sql = "select * from Permission where MenuID={0} and GroupId={1}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = db.GetDataTable(string.Format(sql, PageId, GroupId));
            if (dt.Rows.Count > 0) {
                return true;
            } else {
                return false;
            }
               
        }

        public static bool CheckGroupPermission(long GroupId , string editArea = "")
        {
            bool Permission = false;
            switch (editArea) {

                default:
                    if (GroupId == 1 || GroupId == 2 || GroupId == 7 || GroupId == 8)
                        Permission = true;
                    break;
            }
            return Permission;
        }
        #endregion

        #endregion

        public static MemberModels.LoginStatus SocialLoginCheck(string LoginID, string PassWord)
        {
            //DENISE 待補






            return MemberModels.LoginStatus.Success;

        }
        #endregion

        public static bool IsEmailExist(string email)
        {
            return CommonDA.IsValueExists("Member", "Email", email);
        }

        public static bool IsUserIdExist(string userID)
        {
            return CommonDA.IsValueExists("Member", "LoginID", userID);
        }

        public static IEnumerable<MemberModels> GetManagmentByPageID(long menuID)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try
                {
                    var param = new Dictionary<string, object>();
                    var commandText = $"SELECT m.* FROM [Member] m INNER JOIN [Permission] P ON P.GroupId = m.GroupId WHERE P.MenuID= @MenuID ";
                    param.Add("@MenuID", menuID);
                    return connection.Query<MemberModels>(commandText, param);
                }
                catch { throw; }
            }
        }
        public static bool ChangeMemberShipStatusAndVerifyStatus(long ID, long SiteID, bool? Status, bool? VerifyType, string StatusNote)
        {
            MemberShipRegSetModels regSetModel = MemberShipRegSetDAO.GetItem(SiteID);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DateTime now = DateTime.Now;
            string sql = "Select 1 From MemberShip Where ID = " + ID.ToString();
            string updColumn = "";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                return false;
            }
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);

            if (Status.HasValue)
            {
                updColumn += ",Status=@Status";
                paraList.Add("@Status", Status.Value);
                if (regSetModel.VerifyType != MemberShipVerifyType.Email &&
                    !VerifyType.HasValue)
                {
                    updColumn += ",VerifyTime=@VerifyTime";
                    if (VerifyType.HasValue && VerifyType.Value)
                        paraList.Add("@VerifyTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    else
                        paraList.Add("@VerifyTime", "");
                }
            }
            if (VerifyType.HasValue)
            {
                updColumn += ",VerifyTime=@VerifyTime";
                if (VerifyType.Value)
                    paraList.Add("@VerifyTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    paraList.Add("@VerifyTime", "");
            }
            if (!string.IsNullOrEmpty(StatusNote))
            {
                updColumn += ",StatusNote=@StatusNote";
                paraList.Add("@StatusNote", StatusNote);
            }
            string modifyStr = string.Format("UPDATE MemberShip SET ModifyTime=GETDATE() {0} WHERE ID=@ID ", updColumn);
            int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
            if (exeCount > 0)
            {
                return true;
            }
            return false;
        }

        public static MemberModels GetItemsByLoginID(string loginId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $" SELECT * FROM Member WHERE LoginID = '{loginId}' ";

                return conn.Query<MemberModels>(sql).SingleOrDefault();
            }
        }
    }
}