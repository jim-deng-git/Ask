using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using WorkV3.Common;
using WorkV3.Models.Interface;
using WorkV3.Models.Service;

namespace WorkV3.Models.DataAccess
{
    public class MemberShipDAO
    {
        private static string FavorityType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Favority.ToString();
        public static MemberShipModels GetItem(long id)
        {
            MemberShipModels item = CommonDA.GetItem<MemberShipModels>("MemberShip", id);

            if (item.Photo.StartsWith("http"))
            {
                item.SocialLargePhoto = string.Format("https://graph.facebook.com/{0}/picture?type=large", item.SocialID);
            }
            else
            {
                item.SocialLargePhoto = item.Photo;
            }
            return item;
        }

        public static MemberShipModels GetItem(long siteId, string account)
        {
            string sql = $"Select top 1  * From MemberShip Where SiteID = { siteId } AND Account = N'{ account.Replace("'", "''") }'";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipModels>(sql).SingleOrDefault();
            }
        }
        public static MemberShipModels GetItemByEmail(long siteId, string account)
        {
            string sql = $"Select top 1 * From MemberShip Where SiteID = { siteId } AND Email = N'{ account.Replace("'", "''") }'";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipModels>(sql).SingleOrDefault();
            }
        }
        public static MemberShipModels GetItemByIDCard(long siteId, string account)
        {
            string sql = $"Select top 1 * From MemberShip Where SiteID = { siteId } AND IDCard = N'{ account.Replace("'", "''") }'";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipModels>(sql).SingleOrDefault();
            }
        }
        public static MemberShipModels GetItemByMobile(long siteId, string account)
        {
            string sql = $"Select top 1 * From MemberShip Where SiteID = { siteId } AND Mobile = N'{ account.Replace("'", "''") }'";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipModels>(sql).SingleOrDefault();
            }
        }

        public static void SetItem(MemberShipModels item, bool setPushRecommandCode = false)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");

            tableObj.GetDataFromObject(item);

            DateTime now = DateTime.Now;
            string sql = "Select 1 From MemberShip Where ID = " + item.ID;
            string sql_recommandcode_check = "Select 1 From MemberShip Where RecommandCode ='{0}' ";
            bool isNew = db.GetFirstValue(sql) == null;
            //item.OrderEpaper = (item.OrderEpaper == null ? false : item.OrderEpaper);

            tableObj["Photo"] = string.IsNullOrEmpty(item.Photo) ? "" : item.Photo;
            tableObj["Company"] = string.IsNullOrEmpty(item.Company) ? "" : item.Company;
            tableObj["Regions"] = string.IsNullOrEmpty(item.Regions) ? "" : item.Regions;
            tableObj["Address"] = string.IsNullOrEmpty(item.Address) ? "" : item.Address;
            tableObj["Marriage"] = string.IsNullOrEmpty(item.Marriage) ? "" : item.Marriage;
            tableObj["Education"] = string.IsNullOrEmpty(item.Education) ? "" : item.Education;
            tableObj["Career"] = string.IsNullOrEmpty(item.Career) ? "" : item.Career;
            tableObj["EmergencyName"] = string.IsNullOrEmpty(item.EmergencyName) ? "" : item.EmergencyName;
            tableObj["EmergencyPhone"] = string.IsNullOrEmpty(item.EmergencyPhone) ? "" : item.EmergencyPhone;
            tableObj["EmergencyEmail"] = string.IsNullOrEmpty(item.EmergencyEmail) ? "" : item.EmergencyEmail;
            tableObj["EmergencyMobile"] = string.IsNullOrEmpty(item.EmergencyMobile) ? "" : item.EmergencyMobile;
            tableObj["Password"] = string.IsNullOrEmpty(item.Password) ? "" : item.Password;
            tableObj["PushRecommandCode"] = string.IsNullOrEmpty(item.PushRecommandCode) ? "" : item.PushRecommandCode;
            if (isNew)
            {
                if (item.Creator == 0)
                    tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;

                tableObj["LastResentVerifyMailTime"] = "";
                tableObj["VerifyType"] = (int)MemberVerifyType.Register;
                tableObj["RecommandCode"] = "";
                string RecommandCode = WorkLib.uRandom.GetRandomCode(10);// WorkV3.Common.Member.GenerateRecommandCode();
                bool IsExistRecommandCode = db.GetFirstValue(string.Format(sql_recommandcode_check, RecommandCode)) != null;
                while (IsExistRecommandCode)
                {
                    RecommandCode = WorkLib.uRandom.GetRandomCode(10);// WorkV3.Common.Member.GenerateRecommandCode();
                    IsExistRecommandCode = db.GetFirstValue(string.Format(sql_recommandcode_check, RecommandCode)) != null;
                }

                tableObj["RecommandCode"] = RecommandCode;
                tableObj["Photo"] = string.IsNullOrEmpty(item.Photo) ? "" : item.Photo;
                tableObj["Company"] = string.IsNullOrEmpty(item.Company) ? "" : item.Company;
                tableObj["Regions"] = string.IsNullOrEmpty(item.Regions) ? "" : item.Regions;
                tableObj["Address"] = string.IsNullOrEmpty(item.Address) ? "" : item.Address;
                tableObj["Marriage"] = string.IsNullOrEmpty(item.Marriage) ? "" : item.Marriage;
                tableObj["Education"] = string.IsNullOrEmpty(item.Education) ? "" : item.Education;
                tableObj["Career"] = string.IsNullOrEmpty(item.Career) ? "" : item.Career;
                tableObj["EmergencyName"] = string.IsNullOrEmpty(item.EmergencyName) ? "" : item.EmergencyName;
                tableObj["EmergencyPhone"] = string.IsNullOrEmpty(item.EmergencyPhone) ? "" : item.EmergencyPhone;
                tableObj["EmergencyEmail"] = string.IsNullOrEmpty(item.EmergencyEmail) ? "" : item.EmergencyEmail;
                tableObj["EmergencyMobile"] = string.IsNullOrEmpty(item.EmergencyMobile) ? "" : item.EmergencyMobile;
                tableObj["CreateTime"] = now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                //tableObj.Remove("Status");
                tableObj.Remove("LastLoginTime");
                tableObj.Remove("RecommandCode");
                if(item.Password==null)
                    tableObj.Remove("Password");

                if (!setPushRecommandCode)
                    tableObj.Remove("PushRecommandCode");

                Common.Member curUser = Common.Member.Current;
                if (Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent == null)
                {
                    tableObj["Modifier"] = curUser == null ? 1 : curUser.ID;
                }
                else
                {
                    tableObj["Modifier"] = curUser == null ? Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id : curUser.ID;
                }


                tableObj["ModifyTime"] = now;

                tableObj.Update(item.ID);
            }
        }

        public static void RegisterMemberShip(PageCache pageCache, MemberShipModels item)
        {
            string SitesTitle = SitesDAO.GetInfo(pageCache.SiteID).Title;//網站名稱
            IEmailService emailService = new EmailService();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            tableObj.GetDataFromObject(item);

            DateTime now = DateTime.Now;
            string sql = "Select 1 From MemberShip Where ID = " + item.ID;
            string sql_recommandcode_check = "Select 1 From MemberShip Where RecommandCode ='{0}' ";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                //string RecommandCode = WorkV3.Common.Member.GenerateRecommandCode();
                string RecommandCode = WorkLib.uRandom.GetRandomCode(10);
                bool IsExistRecommandCode = db.GetFirstValue(string.Format(sql_recommandcode_check, RecommandCode)) != null;
                while (IsExistRecommandCode)
                {
                    //RecommandCode = WorkV3.Common.Member.GenerateRecommandCode();
                    RecommandCode = WorkLib.uRandom.GetRandomCode(10);
                    IsExistRecommandCode = db.GetFirstValue(string.Format(sql_recommandcode_check, RecommandCode)) != null;
                }
                if (item.Creator == 0)
                {
                    if (Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent == null)
                    {
                        tableObj["Creator"] = 1;
                    }
                    else
                    {
                        tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                    }
                }
                tableObj["CreateTime"] = now;
                tableObj["VerifyCode"] = item.VerifyCode;
                tableObj["VerifyTime"] = "";
                tableObj["LastResentVerifyMailTime"] = "";
                tableObj["VerifyType"] = (int)MemberVerifyType.Register;
                tableObj["RecommandCode"] = RecommandCode;
                tableObj["PushRecommandCode"] = !string.IsNullOrEmpty(item.PushRecommandCode) ? item.PushRecommandCode : "";
                tableObj["Photo"] = string.IsNullOrEmpty(item.Photo) ? "" : item.Photo;
                tableObj["Company"] = string.IsNullOrEmpty(item.Company) ? "" : item.Company;
                tableObj["Regions"] = string.IsNullOrEmpty(item.Regions) ? "" : item.Regions;
                tableObj["Address"] = string.IsNullOrEmpty(item.Address) ? "" : item.Address;
                tableObj["Marriage"] = string.IsNullOrEmpty(item.Marriage) ? "" : item.Marriage;
                tableObj["Education"] = string.IsNullOrEmpty(item.Education) ? "" : item.Education;
                tableObj["Career"] = string.IsNullOrEmpty(item.Career) ? "" : item.Career;
                tableObj["EmergencyName"] = string.IsNullOrEmpty(item.EmergencyName) ? "" : item.EmergencyName;
                tableObj["EmergencyPhone"] = string.IsNullOrEmpty(item.EmergencyPhone) ? "" : item.EmergencyPhone;
                tableObj["EmergencyEmail"] = string.IsNullOrEmpty(item.EmergencyEmail) ? "" : item.EmergencyEmail;
                tableObj["EmergencyMobile"] = string.IsNullOrEmpty(item.EmergencyMobile) ? "" : item.EmergencyMobile;
                tableObj["ResetPWD"] = "0";
                item.CreateTime = now;

                tableObj.Insert();

                string VerifyUrl = string.Format("{0}://{1}{2}{3}/w/{4}/Verify?M={5}&K={6}", HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.DnsSafeHost,
                    HttpContext.Current.Request.Url.Port == 80 ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(),
                    HttpContext.Current.Request.ApplicationPath == "/" ? "" : "/" + HttpContext.Current.Request.ApplicationPath.Trim('/'),
                    WorkLib.GetItem.appSet("DefaultSiteSN").ToString(),
                    tableObj["ID"].ToString(), item.VerifyCode);
                WorkV3.Areas.Backend.Models.MailTemplateSetModels mailTemplateModel = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMailTemplateItems("Regist");
                if (mailTemplateModel != null)
                {
                    ArrayList attFileList = new ArrayList();
                    if (!string.IsNullOrEmpty(mailTemplateModel.AttFiles))
                    {
                        string UploadVPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(pageCache.SiteID, pageCache.MenuID);
                        List<WorkV3.Areas.Backend.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Areas.Backend.Models.ResourceFilesModels>>(mailTemplateModel.AttFiles);
                        foreach (WorkV3.Areas.Backend.Models.ResourceFilesModels f in fileList)
                        {
                            //System.Net.Mail.Attachment attFile = new System.Net.Mail.Attachment(UploadVPath + "/" + f.FileInfo);
                            attFileList.Add(UploadVPath + f.FileInfo);
                        }
                    }
                    string mailSubject = mailTemplateModel.MailTitle
                        .Replace("[WebsiteName]", SitesTitle)
                        .Replace("[MemberName]", item.Name);//"太報 Tai Sounds 會員註冊驗證信";
                    string mailContent = mailTemplateModel.MailContent.Replace("[MemberName]", item.Name)
                        .Replace("[MemberID]", item.Account)
                        .Replace("[StartLink]", VerifyUrl)
                        .Replace("[WebsiteName]", SitesTitle)
                        .Replace("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));

                    //System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/mailContent/MembershipSignUp.html"))
                    //.Replace("[Name]", item.Name).Replace("[VerifyUrl]", VerifyUrl);

                    //註解-首次註冊驗證信            
                    if (attFileList.Count > 0)
                        emailService.SendMailWithFiles(item.SiteID, item.Email, item.Name, mailSubject, mailContent, attFileList, mailTemplateModel.MailFromAddress, SitesTitle);
                    else
                        emailService.SendEMail(item.SiteID, item.Email, item.Name, mailSubject, mailContent, mailTemplateModel.MailFromAddress, SitesTitle);

                }
            }
        }
        public static bool ModifyMemberShip(long siteId, PageCache pageCache, MemberShipModels item, out bool IsModifyEmail)
        {
            string SitesTitle = SitesDAO.GetInfo(pageCache.SiteID).Title;//網站名稱
            IEmailService emailService = new EmailService();

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            tableObj.GetDataFromObject(item);
            MemberShipModels old_item = GetItem(item.ID);
            IsModifyEmail = false;
            if (item.Email != old_item.Email)
            {
                IsModifyEmail = true;
            }
            DateTime now = DateTime.Now;
            string sql = "Select 1 From MemberShip Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                return false;
            }

            tableObj["Photo"] = string.IsNullOrEmpty(item.Photo) ? "" : item.Photo;
            tableObj["Company"] = string.IsNullOrEmpty(item.Company) ? "" : item.Company;
            tableObj["Regions"] = string.IsNullOrEmpty(item.Regions) ? "" : item.Regions;
            tableObj["Address"] = string.IsNullOrEmpty(item.Address) ? "" : item.Address;
            tableObj["Marriage"] = string.IsNullOrEmpty(item.Marriage) ? "" : item.Marriage;
            tableObj["Education"] = string.IsNullOrEmpty(item.Education) ? "" : item.Education;
            tableObj["Career"] = string.IsNullOrEmpty(item.Career) ? "" : item.Career;
            tableObj["EmergencyName"] = string.IsNullOrEmpty(item.EmergencyName) ? "" : item.EmergencyName;
            tableObj["EmergencyPhone"] = string.IsNullOrEmpty(item.EmergencyPhone) ? "" : item.EmergencyPhone;
            tableObj["EmergencyEmail"] = string.IsNullOrEmpty(item.EmergencyEmail) ? "" : item.EmergencyEmail;
            tableObj["EmergencyMobile"] = string.IsNullOrEmpty(item.EmergencyMobile) ? "" : item.EmergencyMobile;

            tableObj.Remove("ID");
            tableObj.Remove("Photo");
            tableObj.Remove("SiteID");
            tableObj.Remove("Creator");
            tableObj.Remove("Password");
            tableObj.Remove("CreateTime");
            tableObj.Remove("Status");
            tableObj.Remove("LastLoginTime");
            tableObj.Remove("RecommandCode");

            item.VerifyCode = Guid.NewGuid().ToString();
            Common.Member curUser = Common.Member.Current;
            tableObj["Modifier"] = curUser == null ? Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id : curUser.ID;
            tableObj["ModifyTime"] = now;
            tableObj["VerifyType"] = (int)MemberVerifyType.Modify;
            tableObj["VerifyCode"] = item.VerifyCode;
            if (IsModifyEmail)
            {
                tableObj["VerifyTime"] = "";
            }
            else
            {
                tableObj.Remove("VerifyTime");
            }
            tableObj["LastResentVerifyMailTime"] = "";
            tableObj["PushRecommandCode"] = !string.IsNullOrEmpty(item.PushRecommandCode) ? item.PushRecommandCode : "";
            tableObj["ResetPWD"] = "0";
            tableObj.Update(item.ID);

            if (IsModifyEmail)
            {
                string VerifyUrl = string.Format("{0}://{1}{2}{3}/w/{4}/Verify?M={5}&K={6}", HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.DnsSafeHost,
                    HttpContext.Current.Request.Url.Port == 80 ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(),
                    HttpContext.Current.Request.ApplicationPath == "/" ? "" : "/" + HttpContext.Current.Request.ApplicationPath.Trim('/'),
                    WorkLib.GetItem.appSet("DefaultSiteSN").ToString(),
                    item.ID.ToString(), item.VerifyCode);

                //string mailSubject = "太報 Tai Sounds 會員資料修改驗證信";
                //string mailContent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/mailContent/MembershipModify.html"))
                //    .Replace("[Name]", item.Name).Replace("[VerifyUrl]", VerifyUrl);
                WorkV3.Areas.Backend.Models.MailTemplateSetModels mailTemplateModel = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMailTemplateItems("ChangeEmail");
                if (mailTemplateModel != null)
                {
                    ArrayList attFileList = new ArrayList();
                    if (!string.IsNullOrEmpty(mailTemplateModel.AttFiles))
                    {
                        string UploadVPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(pageCache.SiteID, pageCache.MenuID);
                        List<WorkV3.Areas.Backend.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Areas.Backend.Models.ResourceFilesModels>>(mailTemplateModel.AttFiles);
                        foreach (WorkV3.Areas.Backend.Models.ResourceFilesModels f in fileList)
                        {
                            //System.Net.Mail.Attachment attFile = new System.Net.Mail.Attachment(UploadVPath + "/" + f.FileInfo);
                            attFileList.Add(UploadVPath + f.FileInfo);
                        }
                    }
                    string mailSubject = mailTemplateModel
                        .MailTitle.Replace("[WebsiteName]", SitesTitle)
                        .Replace("[MemberName]", item.Name);//"太報 Tai Sounds 會員註冊驗證信";
                    string mailContent = mailTemplateModel.MailContent.Replace("[MemberName]", item.Name)
                        .Replace("[MemberID]", item.Account)
                        .Replace("[ChangeLink]", VerifyUrl)
                        .Replace("[WebsiteName]", SitesTitle)
                        .Replace("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));
                    if (attFileList.Count > 0)
                        emailService.SendMailWithFiles(siteId, item.Email, item.Name, mailSubject, mailContent, attFileList, mailTemplateModel.MailFromAddress, SitesTitle);
                    else
                        emailService.SendEMail(siteId, item.Email, item.Name, mailSubject, mailContent, mailTemplateModel.MailFromAddress, SitesTitle);
                }
            }
            return true;
        }
        public static bool ModifyMemberShipPWD(long siteId, PageCache pageCache, long ID, string PWD)
        {
            string SitesTitle = SitesDAO.GetInfo(pageCache.SiteID).Title;//網站名稱
            IEmailService emailService = new EmailService();

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DateTime now = DateTime.Now;
            string sql = "Select 1 From MemberShip Where ID = " + ID.ToString();
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                return false;
            }
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);
            paraList.Add("@Password", PWD);
            string modifyStr = "UPDATE MemberShip SET Password=@Password, ModifyTime=GETDATE() WHERE ID=@ID ";
            int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
            if (exeCount > 0)
            {
                MemberShipModels member = GetItem(ID);
                string VerifyUrl = string.Format("{0}://{1}{2}{3}/w/{4}/Login", HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.DnsSafeHost,
                    HttpContext.Current.Request.Url.Port == 80 ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(),
                    HttpContext.Current.Request.ApplicationPath == "/" ? "" : "/" + HttpContext.Current.Request.ApplicationPath.Trim('/'),
                    WorkLib.GetItem.appSet("DefaultSiteSN").ToString());

                //string mailSubject = "太報 Tai Sounds 會員資料修改驗證信";
                //string mailContent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/mailContent/MembershipModify.html"))
                //    .Replace("[Name]", item.Name).Replace("[VerifyUrl]", VerifyUrl);
                WorkV3.Areas.Backend.Models.MailTemplateSetModels mailTemplateModel = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMailTemplateItems("ChangePWD");
                if (mailTemplateModel != null)
                {

                    ArrayList attFileList = new ArrayList();
                    if (!string.IsNullOrEmpty(mailTemplateModel.AttFiles))
                    {
                        string UploadVPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(pageCache.SiteID, pageCache.MenuID);
                        List<WorkV3.Areas.Backend.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Areas.Backend.Models.ResourceFilesModels>>(mailTemplateModel.AttFiles);
                        foreach (WorkV3.Areas.Backend.Models.ResourceFilesModels f in fileList)
                        {
                            //System.Net.Mail.Attachment attFile = new System.Net.Mail.Attachment(UploadVPath + "/" + f.FileInfo);
                            attFileList.Add(UploadVPath + f.FileInfo);
                        }
                    }
                    string mailSubject = mailTemplateModel
                        .MailTitle.Replace("[WebsiteName]", SitesTitle);//"太報 Tai Sounds 密碼變更通知信";
                    string mailContent = mailTemplateModel.MailContent.Replace("[MemberName]", member.Name)
                            .Replace("[ChangeLink]", VerifyUrl)
                            .Replace("[WebsiteName]", SitesTitle)
                            .Replace("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));
                    if (attFileList.Count > 0)
                        emailService.SendMailWithFiles(siteId, member.Email, member.Name, mailSubject, mailContent, attFileList, mailTemplateModel.MailFromAddress, SitesTitle);
                    else
                        emailService.SendEMail(siteId, member.Email, member.Name, mailSubject, mailContent, mailTemplateModel.MailFromAddress, SitesTitle);
                }
                return true;
            }
            return false;
        }
        public static bool ModifyMemberShipRecommandCode(long ID, string RecommandCode)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DateTime now = DateTime.Now;
            string sql = "Select 1 From MemberShip Where ID = " + ID.ToString();
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                return false;
            }
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);
            paraList.Add("@PushRecommandCode", RecommandCode);
            string modifyStr = "UPDATE MemberShip SET PushRecommandCode=@PushRecommandCode, ModifyTime=GETDATE() WHERE ID=@ID ";
            int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
            if (exeCount > 0)
            {
                return true;
            }
            return false;
        }
        public static bool ResentMemberVerifyMail(long siteId, PageCache pageCache, MemberShipModels item)
        {
            string SitesTitle = SitesDAO.GetInfo(siteId).Title;//網站名稱
            IEmailService emailService = new EmailService();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            tableObj.GetDataFromObject(item);

            DateTime now = DateTime.Now;
            string verifyCode = Guid.NewGuid().ToString();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", item.ID);
            paraList.Add("@VerifyCode", verifyCode);
            paraList.Add("@LastResentVerifyMailTime", now.ToString("yyyy-MM-dd HH:mm:ss"));
            string modifyStr = "UPDATE MemberShip SET LastResentVerifyMailTime=@LastResentVerifyMailTime, VerifyCode=@VerifyCode WHERE ID=@ID ";

            int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
            if (exeCount > 0)
            {
                string VerifyUrl = string.Format("{0}://{1}{2}{3}/w/{4}/Verify?M={5}&K={6}", HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.DnsSafeHost,
                    HttpContext.Current.Request.Url.Port == 80 ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(),
                    HttpContext.Current.Request.ApplicationPath == "/" ? "" : "/" + HttpContext.Current.Request.ApplicationPath.Trim('/'),
                    WorkLib.GetItem.appSet("DefaultSiteSN").ToString(),
                    item.ID.ToString(), verifyCode);

                string mailSubject = "";
                string mailContent = "";
                if (item.VerifyType == MemberVerifyType.Modify)
                {
                    mailSubject = "太報 Tai Sounds 會員資料修改驗證信";
                    mailContent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/mailContent/MembershipModify.html"))
                      .Replace("[Name]", item.Name).Replace("[StartLink]", VerifyUrl);

                    WorkV3.Areas.Backend.Models.MailTemplateSetModels mailTemplateModel = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMailTemplateItems("Regist");
                    if (mailTemplateModel != null)
                    {
                        ArrayList attFileList = new ArrayList();
                        if (!string.IsNullOrEmpty(mailTemplateModel.AttFiles))
                        {
                            string UploadVPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(pageCache.SiteID, pageCache.MenuID);
                            List<WorkV3.Areas.Backend.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Areas.Backend.Models.ResourceFilesModels>>(mailTemplateModel.AttFiles);
                            foreach (WorkV3.Areas.Backend.Models.ResourceFilesModels f in fileList)
                            {
                                //System.Net.Mail.Attachment attFile = new System.Net.Mail.Attachment(UploadVPath + "/" + f.FileInfo);
                                attFileList.Add(UploadVPath + f.FileInfo);
                            }
                        }
                        mailSubject = mailTemplateModel
                            .MailTitle.Replace("[WebsiteName]", pageCache.SiteName)
                            .Replace("[MemberName]", item.Name);
                            //"太報 Tai Sounds 會員註冊驗證信";
                        mailContent = mailTemplateModel.MailContent.Replace("[MemberName]", item.Name)
                            .Replace("[MemberID]", item.Account)
                            .Replace("[ChangeLink]", VerifyUrl)
                            .Replace("[StartLink]", VerifyUrl)
                            .Replace("[WebsiteName]", pageCache.SiteName)
                            .Replace("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));

                        if (attFileList.Count > 0)
                            emailService.SendMailWithFiles(siteId, item.Email, item.Name, mailSubject, mailContent, attFileList, mailTemplateModel.MailFromAddress, SitesTitle);
                        else
                            emailService.SendEMail(siteId, item.Email, item.Name, mailSubject, mailContent, mailTemplateModel.MailFromAddress, SitesTitle);

                    }
                }
                else
                {
                    //mailSubject = "太報 Tai Sounds 會員註冊驗證信";
                    //mailContent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/mailContent/MembershipSignUp.html"))
                    //  .Replace("[Name]", item.Name).Replace("[VerifyUrl]", VerifyUrl);

                    WorkV3.Areas.Backend.Models.MailTemplateSetModels mailTemplateModel = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMailTemplateItems("Regist");
                    if (mailTemplateModel != null)
                    {
                        ArrayList attFileList = new ArrayList();
                        if (!string.IsNullOrEmpty(mailTemplateModel.AttFiles))
                        {
                            string UploadVPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(pageCache.SiteID, pageCache.MenuID);
                            List<WorkV3.Areas.Backend.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Areas.Backend.Models.ResourceFilesModels>>(mailTemplateModel.AttFiles);
                            foreach (WorkV3.Areas.Backend.Models.ResourceFilesModels f in fileList)
                            {
                                //System.Net.Mail.Attachment attFile = new System.Net.Mail.Attachment(UploadVPath + "/" + f.FileInfo);
                                attFileList.Add(UploadVPath + f.FileInfo);
                            }
                        }
                        mailSubject = mailTemplateModel.MailTitle
                            .Replace("[WebsiteName]", pageCache.SiteName)
                            .Replace("[MemberName]", item.Name);//"太報 Tai Sounds 會員註冊驗證信";
                        mailContent = mailTemplateModel.MailContent.Replace("[MemberName]", item.Name)
                            .Replace("[MemberID]", item.Account)
                            .Replace("[ChangeLink]", VerifyUrl)
                            .Replace("[StartLink]", VerifyUrl)
                            .Replace("[WebsiteName]", pageCache.SiteName)
                            .Replace("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));

                        if (attFileList.Count > 0)
                            emailService.SendMailWithFiles(siteId, item.Email, item.Name, mailSubject, mailContent, attFileList, mailTemplateModel.MailFromAddress, SitesTitle);
                        else
                            emailService.SendEMail(siteId, item.Email, item.Name, mailSubject, mailContent, mailTemplateModel.MailFromAddress, SitesTitle);
                    }
                }
                emailService.SendEMail(siteId, item.Email, item.Name, mailSubject, mailContent);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static VerifyResult VerifyMemberShip(long ID, string verifyCode)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);
            paraList.Add("@VerifyCode", verifyCode);

            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            string sql = "Select top 1 * From MemberShip Where ID=@ID AND VerifyCode=@VerifyCode  ";
            DataTable memTable = db.GetDataTable(sql, paraList);
            if (memTable == null || memTable.Rows.Count <= 0)
                return VerifyResult.DataError;

            MemberVerifyType verifyType = (MemberVerifyType)int.Parse(memTable.Rows[0]["VerifyType"].ToString());
            if (!string.IsNullOrEmpty(memTable.Rows[0]["VerifyTime"].ToString()))
                return VerifyResult.IsVerified;
            paraList.Add("@VerifyTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string modifyStr = "UPDATE MemberShip SET VerifyTime=@VerifyTime, VerifyType=0 WHERE ID=@ID AND VerifyCode=@VerifyCode ";
            int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
            if (exeCount > 0)
            {
                if (verifyType == MemberVerifyType.Modify)
                    return VerifyResult.ModifyVerifiedSuccess;
                else
                    return VerifyResult.Success;
            }
            return VerifyResult.DataError;
        }
        public static VerifyResult ForgetMemberShip(long siteId, PageCache pageCache, string Account, string Email)
        {
            string SitesTitle = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteId).Title;//網站名稱
            IEmailService emailService = new EmailService();
            string MemberName = "";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@Account", Account);
            paraList.Add("@Email", Email);

            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            string sql = "Select top 1 * From MemberShip Where Account=@Account AND Email=@Email  ";
            DataTable memTable = db.GetDataTable(sql, paraList);
            if (memTable == null || memTable.Rows.Count <= 0)
                return VerifyResult.DataError;

            if (string.IsNullOrEmpty(memTable.Rows[0]["VerifyTime"].ToString()))
                return VerifyResult.NotVerified;
            MemberName = memTable.Rows[0]["Name"].ToString();
            paraList.Clear();
            paraList.Add("@ID", long.Parse(memTable.Rows[0]["ID"].ToString()));
            string modifyStr = "UPDATE MemberShip SET ResetPWD=1 WHERE ID=@ID ";
            int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
            if (exeCount > 0)
            {
                string ForgetUrl = string.Format("{0}://{1}{2}{3}/w/{4}/ResetPWD?M={5}", HttpContext.Current.Request.Url.Scheme,
                  HttpContext.Current.Request.Url.DnsSafeHost,
                  HttpContext.Current.Request.Url.Port == 80 ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(),
                  HttpContext.Current.Request.ApplicationPath == "/" ? "" : "/" + HttpContext.Current.Request.ApplicationPath.Trim('/'),
                  pageCache.SiteSN,
                  memTable.Rows[0]["ID"].ToString());
                WorkV3.Areas.Backend.Models.MailTemplateSetModels mailTemplateModel = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetMailTemplateItems("Forget");
                if (mailTemplateModel != null)
                {
                    ArrayList attFileList = new ArrayList();
                    if (!string.IsNullOrEmpty(mailTemplateModel.AttFiles))
                    {
                        string UploadVPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(pageCache.SiteID, pageCache.MenuID);
                        List<WorkV3.Areas.Backend.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Areas.Backend.Models.ResourceFilesModels>>(mailTemplateModel.AttFiles);
                        foreach (WorkV3.Areas.Backend.Models.ResourceFilesModels f in fileList)
                        {
                            //System.Net.Mail.Attachment attFile = new System.Net.Mail.Attachment(UploadVPath + "/" + f.FileInfo);
                            attFileList.Add(UploadVPath + f.FileInfo);
                        }
                    }
                    string mailSubject = mailTemplateModel.MailTitle
                        .Replace("[WebsiteName]", SitesTitle)
                        .Replace("[MemberName]", MemberName);//"太報 Tai Sounds 會員註冊驗證信";
                    string mailContent = mailTemplateModel.MailContent.Replace("[MemberName]", MemberName)
                        .Replace("[ChangeLink]", ForgetUrl)
                        .Replace("[WebsiteName]", SitesTitle)
                        .Replace("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));

                    if (attFileList.Count > 0)
                        emailService.SendMailWithFiles(siteId, Email, MemberName, mailSubject, mailContent, attFileList, mailTemplateModel.MailFromAddress, SitesTitle);
                    else
                        emailService.SendEMail(siteId, Email, MemberName, mailSubject, mailContent, mailTemplateModel.MailFromAddress, SitesTitle);
                }
                //string mailSubject = "太報 Tai Sounds 會員忘記密碼驗證信";
                //string mailContent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/mailContent/MembershipForget.html"))
                //    .Replace("[Name]", MemberName).Replace("[ForgetUrl]", ForgetUrl);
                return VerifyResult.Success;
            }
            return VerifyResult.DataError;
        }
        public static VerifyResult LoadResetMemberShip(long ID, out MemberShipModels item)
        {
            item = null;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);

            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            string sql = "Select top 1 * From MemberShip Where ID=@ID  ";

            DataTable memTable = db.GetDataTable(sql, paraList);
            if (memTable == null || memTable.Rows.Count <= 0)
                return VerifyResult.DataError;

            if (string.IsNullOrEmpty(memTable.Rows[0]["VerifyTime"].ToString()))
                return VerifyResult.NotVerified;

            if (Convert.ToBoolean(memTable.Rows[0]["ResetPWD"].ToString()))
                return VerifyResult.NotNeedReset;

            item = GetItem(ID);
            return VerifyResult.Success;
        }
        public static VerifyResult ResetPWD(long ID, string password)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);

            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            string sql = "Select top 1 * From MemberShip Where ID=@ID  ";

            DataTable memTable = db.GetDataTable(sql, paraList);
            if (memTable == null || memTable.Rows.Count <= 0)
                return VerifyResult.DataError;

            if (string.IsNullOrEmpty(memTable.Rows[0]["VerifyTime"].ToString()))
                return VerifyResult.NotVerified;

            if (!Convert.ToBoolean(memTable.Rows[0]["ResetPWD"].ToString()))
                return VerifyResult.NotNeedReset;

            paraList.Clear();
            paraList.Add("@ID", long.Parse(memTable.Rows[0]["ID"].ToString()));
            paraList.Add("@Password", password);

            string modifyStr = "UPDATE MemberShip SET Password=@Password, ResetPWD=0 WHERE ID=@ID ";
            int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
            if (exeCount > 0)
                return VerifyResult.Success;
            return VerifyResult.DataError;
        }
        public static int Delete(long id)
        {
            long[] ids = { id };
            return Delete(ids);
        }

        public static int Delete(IEnumerable<long> ids)
        {
            if (ids == null || ids.Count() == 0)
                return 0;

            string sql =
                    "Delete MemberShip Where ID In ({0})\n";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }

        public static IEnumerable<MemberShipModels> GetItems(MemberShipSearchModels search, int pageSize, int pageIndex, out int recordCount)
        {
            List<MemberShipModels> items = new List<MemberShipModels>();
            if (search == null)
            {
                recordCount = 0;
                return items;
            }

            string sql =
                "Select ID, Account, Name, Photo, Email, Mobile, Status, CreateTime, LastLoginTime, MemberType, VerifyTime, LastResentVerifyMailTime " +
                "From MemberShip Where {0} Order By {1}";

            List<string> where = new List<string>();
            where.Add("SiteID = " + search.SiteID);

            if (!string.IsNullOrWhiteSpace(search.Key))
            {
                string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                where.Add(string.Format("(Account {0} OR Name {0} OR Email {0} OR Mobile {0} OR Phone {0} OR IDCard {0})", key));
            }

            if (search.StartRegTime != null)
                where.Add(string.Format("CreateTime >= '{0:yyyy/MM/dd HH:mm}'", search.StartRegTime));

            if (search.EndRegTime != null)
                where.Add(string.Format("CreateTime <= '{0:yyyy/MM/dd HH:mm}'", search.EndRegTime));

            if (search.StartLoginTime != null)
                where.Add(string.Format("LastLoginTime >= '{0:yyyy/MM/dd HH:mm}'", search.StartLoginTime));

            if (search.EndLoginTime != null)
                where.Add(string.Format("LastLoginTime >= '{0:yyyy/MM/dd HH:mm}'", search.EndLoginTime));
            if (search.IdenityList != null)
            {
                string cateIDs = string.Join(",", search.IdenityList);
                where.Add(string.Format(" ID IN (SELECT MemberShipID FROM MemberShipSetting WHERE CategoryID IN ({0}))", cateIDs));

            }
            if (search.FavorityList != null)
            {
                string cateIDs = string.Join(",", search.FavorityList);
                where.Add(string.Format(" ID IN (SELECT MemberShipID FROM MemberShipSetting WHERE CategoryID IN ({0}))", cateIDs));

            }
            if (string.IsNullOrWhiteSpace(search.Sort))
            {
                search.Sort = "CreateTime Desc";
            }
            else
            {
                string[] allowSorts = { "CreateTime Desc", "CreateTime", "LastLoginTime Desc", "LastLoginTime", "Status" };
                if (!allowSorts.Contains(search.Sort))
                    search.Sort = "CreateTime Desc";
            }


            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(string.Format(sql, string.Join(" And ", where), search.Sort), pageSize, pageIndex, out recordCount);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new MemberShipModels
                    {
                        ID = (long)dr["ID"],
                        Account = dr["Account"].ToString().Trim(),
                        MemberType = (MemberType)((int)dr["MemberType"]),
                        Name = dr["Name"].ToString().Trim(),
                        Email = dr["Email"].ToString().Trim(),
                        Mobile = dr["Mobile"].ToString().Trim(),
                        Status = (bool)dr["Status"],
                        VerifyTime = dr["VerifyTime"].ToString().Trim(),
                        Photo = dr["Photo"].ToString().Trim(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        LastLoginTime = dr["LastLoginTime"] as DateTime?,
                        LastResentVerifyMailTime = dr["LastResentVerifyMailTime"].ToString().Trim()
                    });
                }
            }

            return items;
        }

        public static List<string> GetItemsAll(MemberShipSearchModels search)
        {
            List<string> items = new List<string>();
            string sql =
                "Select ID " +
                "From MemberShip Where {0} Order By {1}";

            List<string> where = new List<string>();
            where.Add("SiteID = " + search.SiteID);

            if (!string.IsNullOrWhiteSpace(search.Key))
            {
                string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                where.Add(string.Format("(Account {0} OR Name {0} OR Email {0} OR Mobile {0} OR Phone {0} OR IDCard {0})", key));
            }

            if (search.StartRegTime != null)
                where.Add(string.Format("CreateTime >= '{0:yyyy/MM/dd HH:mm}'", search.StartRegTime));

            if (search.EndRegTime != null)
                where.Add(string.Format("CreateTime <= '{0:yyyy/MM/dd HH:mm}'", search.EndRegTime));

            if (search.StartLoginTime != null)
                where.Add(string.Format("LastLoginTime >= '{0:yyyy/MM/dd HH:mm}'", search.StartLoginTime));

            if (search.EndLoginTime != null)
                where.Add(string.Format("LastLoginTime >= '{0:yyyy/MM/dd HH:mm}'", search.EndLoginTime));
            if (search.IdenityList != null)
            {
                string cateIDs = string.Join(",", search.IdenityList);
                where.Add(string.Format(" ID IN (SELECT MemberShipID FROM MemberShipSetting WHERE CategoryID IN ({0}))", cateIDs));

            }
            if (search.FavorityList != null)
            {
                string cateIDs = string.Join(",", search.FavorityList);
                where.Add(string.Format(" ID IN (SELECT MemberShipID FROM MemberShipSetting WHERE CategoryID IN ({0}))", cateIDs));

            }
            if (string.IsNullOrWhiteSpace(search.Sort))
            {
                search.Sort = "CreateTime Desc";
            }
            else
            {
                string[] allowSorts = { "CreateTime Desc", "CreateTime", "LastLoginTime Desc", "LastLoginTime", "Status" };
                if (!allowSorts.Contains(search.Sort))
                    search.Sort = "CreateTime Desc";
            }


            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(string.Format(sql, string.Join(" And ", where), search.Sort));
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(dr["ID"].ToString());
                }
            }

            return items;
        }
        public static DataTable GetExportItems(MemberShipSearchModels search)
        {
            //if (search == null)
            //{
            //    return items;
            //}

            string sql =
                @"Select * From MemberShip Where {0} Order By {1}";

            List<string> where = new List<string>();
            where.Add("1 =1 ");

            if (!string.IsNullOrWhiteSpace(search.Key))
            {
                string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                where.Add(string.Format("(Account {0} OR Name {0} OR Email {0} OR Mobile {0} OR Phone {0} OR IDCard {0})", key));
            }

            if (search.SiteID != 0)
            {
                where.Add($"SiteID = {search.SiteID}");
            }

            if (search.StartRegTime != null)
                where.Add(string.Format("CreateTime >= '{0:yyyy/MM/dd HH:mm}'", search.StartRegTime));

            if (search.EndRegTime != null)
                where.Add(string.Format("CreateTime <= '{0:yyyy/MM/dd HH:mm}'", search.EndRegTime));

            if (search.StartLoginTime != null)
                where.Add(string.Format("LastLoginTime >= '{0:yyyy/MM/dd HH:mm}'", search.StartLoginTime));

            if (search.EndLoginTime != null)
                where.Add(string.Format("LastLoginTime >= '{0:yyyy/MM/dd HH:mm}'", search.EndLoginTime));

            search.Sort = "CreateTime Desc";
            if (!string.IsNullOrWhiteSpace(search.Sort))
            {
                string[] allowSorts = { "CreateTime Desc", "CreateTime", "LastLoginTime Desc", "LastLoginTime", "Status" };
                if (!allowSorts.Contains(search.Sort))
                    search.Sort = "CreateTime Desc";
            }


            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(string.Format(sql, string.Join(" And ", where), search.Sort));

            return datas;
        }

        public static DataTable GetExportItemsByID(IEnumerable<long> ids)
        {
            var where = ids.ToList();
            string sql = @"Select * From MemberShip Where ID IN ({0}) ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(string.Format(sql, string.Join(",", where)));

            return datas;
        }

        public static void UpdateLoginTime(long memberId)
        {
            string sql = "Update MemberShip Set LastLoginTime = GetDate() Where ID = " + memberId;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static MemberShipModels CheckPushRecommandCode(long siteId, string type, string PushRecommandCode)
        {
            if (string.IsNullOrWhiteSpace(PushRecommandCode))
                return null;

            if (string.IsNullOrWhiteSpace(type))
                return null;

            string sql = $"Select top 1 * From MemberShip Where SiteID = { siteId } And {type} = N'{ PushRecommandCode.Replace("'", "''") }'";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipModels>(sql).SingleOrDefault();
            }
            //SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            //return db.GetFirstValue(sql) != null;
        }

        public static bool CheckAccount(long siteId, string account, long? id)
        {
            if (string.IsNullOrWhiteSpace(account))
                return false;

            string sql = $"Select 1 From MemberShip Where SiteID = { siteId } And Account = N'{ account.Replace("'", "''") }'";
            if (id != null)
                sql += " AND ID <> " + id;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return db.GetFirstValue(sql) == null;
        }
        public static bool CheckEmail(long siteId, string email, long? id)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string sql = $"Select 1 From MemberShip Where Email = N'{ email.Replace("'", "''") }' AND SiteID = '{siteId}'";
            if (id != null)
                sql += " AND ID <> " + id;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return db.GetFirstValue(sql) == null;
        }
        public static bool CheckIDCard(long siteId, string IDCard, long? id)
        {
            if (string.IsNullOrWhiteSpace(IDCard))
                return false;

            string sql = $"Select 1 From MemberShip Where IDCard = N'{ IDCard.Replace("'", "''") }'";
            if (id != null)
                sql += " AND ID <> " + id;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return db.GetFirstValue(sql) == null;
        }
        public static bool CheckPushRecommandCodeExist(string RecommandCode)
        {
            if (string.IsNullOrWhiteSpace(RecommandCode))
                return true;

            string sql = $"Select 1 From MemberShip Where RecommandCode = N'{ RecommandCode.Replace("'", "''") }'";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return db.GetFirstValue(sql) != null;
        }
        public static int GetPageCollectionCount(long pageID)
        {
            if (pageID <= 0)
                return 0;
            string check_sql = $"Select SUM(1) From MemberShipCollections Where PageID={pageID}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            var sumCount = db.GetFirstValue(check_sql);
            if (sumCount != null)
            {
                if (sumCount == DBNull.Value)
                    return 0;

                int collectionCount = (int)sumCount;

                return collectionCount;
            }
            return 0;
        }
        public static CollectionResult CheckMemberCollectionExist(long pageID)
        {
            WorkV3.Common.Member curUser = WorkV3.Common.Member.Current;
            if (curUser == null)
                return CollectionResult.NotLogin;
            if (pageID <= 0)
                return CollectionResult.PageErrror;

            string sql = $"Select 1 From MemberShipCollections Where MemberShipID = {curUser.ID} AND PageID={pageID}";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            if (db.GetFirstValue(sql) != null)
                return CollectionResult.Collectioned;
            return CollectionResult.NoCollection;
        }
        public static int MemberCollection(long pageID, string pageTitle)
        {
            WorkV3.Common.Member curUser = WorkV3.Common.Member.Current;
            if (curUser == null)
                return 0;
            if (pageID <= 0)
                return 0;
            string check_sql = $"Select 1 From MemberShipCollections Where MemberShipID = {curUser.ID} AND PageID={pageID}";
            string sql = $"INSERT INTO MemberShipCollections (ID, MemberShipID, PageID, PageTitle, CreateTime, ModifyTime ) VALUES (@ID, @MemberShipID, @PageID, @PageTitle, GETDATE(), GETDATE())";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            if (db.GetFirstValue(check_sql) != null)
            {
                CancelMemberCollection(pageID);
                return 1;
            }
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", WorkLib.GetItem.NewSN());
            paraList.Add("@MemberShipID", curUser.ID);
            paraList.Add("@PageID", pageID);
            paraList.Add("@PageTitle", pageTitle);
            return db.ExecuteNonQuery(sql, paraList);
        }
        public static int CancelMemberCollection(long pageID)
        {
            WorkV3.Common.Member curUser = WorkV3.Common.Member.Current;
            if (curUser == null)
                return 0;
            if (pageID <= 0)
                return 0;
            string check_sql = $"Select 1 From MemberShipCollections Where MemberShipID = {curUser.ID} AND PageID={pageID}";
            string sql = $" DELETE MemberShipCollections WHERE MemberShipID=@MemberShipID AND PageID=@PageID ";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            if (db.GetFirstValue(check_sql) == null)
                return 0;
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@MemberShipID", curUser.ID);
            paraList.Add("@PageID", pageID);
            return db.ExecuteNonQuery(sql, paraList);
        }
        #region 社群登入

        public static MemberShipModels GetItemBySocialID(long siteId, string socialID)
        {
            string sql = $"Select * From MemberShip Where SiteID = { siteId } AND SocialID = N'{ socialID.Replace("'", "''") }'";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<MemberShipModels>(sql).SingleOrDefault();
            }
        }
        public static bool BindSoicalMember(WorkV3.Models.MemberType memberType, long SiteID, ViewModels.FBMember socialMember)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@MemberType", (int)memberType);
            paraList.Add("@SocialID", socialMember.id);
            //20190923 Nina Add FB登入分站台
            paraList.Add("@SiteID", SiteID);

            SQLData.TableObject tableObj = db.GetTableObject("MemberShip");
            string sql = "Select top 1 * From MemberShip Where SocialID=@SocialID  AND MemberType=@MemberType AND SiteID=@SiteID ";
            string sql_recommandcode_check = "Select 1 From MemberShip Where RecommandCode ='{0}' ";

            DataTable memTable = db.GetDataTable(sql, paraList);
            if (memTable == null || memTable.Rows.Count <= 0)
            {
                string RecommandCode = WorkLib.uRandom.GetRandomCode(10);// //WorkV3.Common.Member.GenerateRecommandCode();
                bool IsExistRecommandCode = db.GetFirstValue(string.Format(sql_recommandcode_check, RecommandCode)) != null;
                while (IsExistRecommandCode)
                {
                    RecommandCode = WorkLib.uRandom.GetRandomCode(10);// WorkV3.Common.Member.GenerateRecommandCode();
                    IsExistRecommandCode = db.GetFirstValue(string.Format(sql_recommandcode_check, RecommandCode)) != null;
                }

                MemberShipModels item = new MemberShipModels();
                item.ID = WorkLib.GetItem.NewSN();
                item.SiteID = SiteID;
                item.Account = socialMember.email;
                item.Email = socialMember.email;
                if (!string.IsNullOrWhiteSpace(socialMember.birthday))
                    item.Birthday = DateTime.Parse(socialMember.birthday);
                if (!string.IsNullOrWhiteSpace(socialMember.gender))
                    item.Sex = socialMember.gender == "male" ? "男" : "女";
                item.Photo = "";
                if (socialMember.picture != null && socialMember.picture.data != null && !string.IsNullOrEmpty(socialMember.picture.data.url))
                {
                    item.Photo = socialMember.picture.data.url;
                }
                item.Password = "";
                item.Name = socialMember.name;
                item.Status = true;
                item.Creator = 0;
                item.VerifyCode = Guid.NewGuid().ToString();
                item.VerifyTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                item.CreateTime = DateTime.Now;
                item.MemberType = MemberType.FB;
                item.SocialID = socialMember.id;
                item.LastLoginTime = DateTime.Now;
                item.LastResentVerifyMailTime = "";
                item.VerifyType = MemberVerifyType.None;
                item.RecommandCode = RecommandCode;
                item.PushRecommandCode = "";
                item.Regions = "";
                item.Address = "";
                item.Marriage = "";
                item.Education = "";
                item.Career = "";
                item.Company = "";
                item.EmergencyName = "";
                item.EmergencyMobile = "";
                item.EmergencyPhone = "";
                item.EmergencyEmail = "";
                item.OrderEpaper = false;
                tableObj.GetDataFromObject(item);
                tableObj.Insert();
                return true;
            }
            else
            {
                paraList.Clear();
                string updateFileds = "Name=@Name,Email=@Email,Account=@Email";
                if (!string.IsNullOrWhiteSpace(socialMember.birthday))
                {
                    updateFileds += ",Birthday=@Birthday";
                    paraList.Add("@Birthday", socialMember.birthday);
                }
                if (!string.IsNullOrWhiteSpace(socialMember.gender))
                {
                    updateFileds += ",Sex=@Sex";
                    paraList.Add("@Sex", socialMember.gender == "male" ? "男" : "女");
                }
                bool IsNeedUpgradePhoto = false;
                if (socialMember.picture != null && !string.IsNullOrEmpty(socialMember.picture.data.url))
                {

                    if (memTable.Rows[0]["Photo"].ToString().StartsWith("http"))
                    {
                        IsNeedUpgradePhoto = true;
                    }
                }
                // 因應太報上線需求, 加入若建立日期為 5/14 之前的且空白的, 就重抓圖片
                if (string.IsNullOrEmpty(memTable.Rows[0]["ModifyTime"].ToString()) ||
                    DateTime.Parse(memTable.Rows[0]["ModifyTime"].ToString()) <= DateTime.Parse("2018/05/14 14:00"))
                {
                    if (string.IsNullOrEmpty(memTable.Rows[0]["Photo"].ToString()))
                    {
                        if (socialMember.picture != null && !string.IsNullOrEmpty(socialMember.picture.data.url))
                        {
                            IsNeedUpgradePhoto = true;
                        }
                    }
                }
                if (IsNeedUpgradePhoto)
                {
                    updateFileds += ",Photo=@Photo";
                    paraList.Add("@Photo", socialMember.picture.data.url);
                }
                string modifyStr = string.Format(@" UPDATE MemberShip SET {0}, ModifyTime=GETDATE(), LastLoginTime=GETDATE() WHERE ID=@ID ",
                    updateFileds);
                paraList.Add("@Name", socialMember.name);
                paraList.Add("@Email", socialMember.email);
                paraList.Add("@ID", long.Parse(memTable.Rows[0]["ID"].ToString()));
                int exeCount = db.ExecuteNonQuery(modifyStr, paraList);
                if (exeCount > 0)
                    return true;

            }
            return false;
        }
        #endregion


        public static List<ViewModels.MemberCollectionViewModel> GetMemberCollections(long memberID)
        {
            List<ViewModels.MemberCollectionViewModel> collectionList = new List<ViewModels.MemberCollectionViewModel>();
            //            string sql = @"Select Pages.*, MemberShipCollections.CreateTime AS CollectionTime From Pages 
            //                                LEFT JOIN MemberShipCollections ON MemberShipCollections.PageID=Pages.No " +
            //                                    $" AND MemberShipID = {memberID} " +
            //                                    $"WHERE No IN (SELECT PageID FROM MemberShipCollections  Where MemberShipID = {memberID}) " +
            //                                    @" AND No IN (
            //                                        Select [Pages].No from [Article]
            //                                        JOIN [Cards] ON [Cards].No=[Article].CardNo
            //                                        JOIN [Zones] ON [Zones].No=[Cards].ZoneNo
            //                                        JOIN [Pages] ON [Pages].No=[Zones].PageNo
            //                                        JOIN Menus ON Menus.ID=Pages.MenuID
            //                                        WHERE [Article].IsIssue=1 AND Menus.ShowStatus=1
            //UNION 

            //                                        Select [Pages].No from [Events]
            //                                        JOIN [Cards] ON [Cards].No=[Events].CardNo
            //                                        JOIN [Zones] ON [Zones].No=[Cards].ZoneNo
            //                                        JOIN [Pages] ON [Pages].No=[Zones].PageNo
            //                                        JOIN Menus ON Menus.ID=Pages.MenuID
            //                                        WHERE [Events].IsIssue=1 AND Menus.ShowStatus=1
            //                                                ) " +
            //                                    " Order By Pages.ModifyTime, CollectionTime DESC ";
            string sql = @"Select Pages.*, MemberShipCollections.CreateTime AS CollectionTime From Pages 
                                LEFT JOIN MemberShipCollections ON MemberShipCollections.PageID=Pages.No " +
                                   $" AND MemberShipID = {memberID} " +
                                   $"WHERE No IN (SELECT PageID FROM MemberShipCollections  Where MemberShipID = {memberID}) " +
                                   @" AND No IN (
                                        Select [Pages].No from [Article]
                                        JOIN [Cards] ON [Cards].No=[Article].CardNo
                                        JOIN [Zones] ON [Zones].No=[Cards].ZoneNo
                                        JOIN [Pages] ON [Pages].No=[Zones].PageNo
                                        JOIN Menus ON Menus.ID=Pages.MenuID
                                        WHERE [Article].IsIssue=1 AND Menus.ShowStatus=1 ) ";
                                   
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable collectionTable = db.GetDataTable(sql);
            if (collectionTable == null)
                return collectionList;

            foreach (DataRow row in collectionTable.Rows)
            {
                collectionList.Add(new ViewModels.MemberCollectionViewModel()
                {
                    No = long.Parse(row["No"].ToString()),
                    MenuID = long.Parse(row["MenuID"].ToString()),
                    Title = row["Title"].ToString(),
                    SN = row["SN"].ToString(),
                    ImgSrc = "",
                    ImgAlt = "",
                    LinkUrl = "",
                    Summary = ""
                });
            }
            return collectionList;
        }
        public static List<ViewModels.MemberCollectionMenuViewModel> GetMenuCollections(long siteID, long memberID)
        {
            List<ViewModels.MemberCollectionMenuViewModel> collectionList = new List<ViewModels.MemberCollectionMenuViewModel>();
            string sql = $"SELECT * FROM Menus where SiteID={siteID} and ShowStatus=1  " +
                        @" AND ID IN (SELECT MenuID FROM Pages WHERE No IN (
                                SELECT PageID FROM MemberShipCollections " +
                                $"WHERE MemberShipID={memberID})) ORDER BY sort ASC ";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable collectionTable = db.GetDataTable(sql);
            if (collectionTable == null)
                return collectionList;

            foreach (DataRow row in collectionTable.Rows)
            {
                collectionList.Add(new ViewModels.MemberCollectionMenuViewModel()
                {
                    MenuID = long.Parse(row["ID"].ToString()),
                    Title = row["Title"].ToString(),
                    SN = row["SN"].ToString()
                });
            }
            return collectionList;
        }
        public static FormItem GetMemberApplyedRecord(long eventID, long memberID)
        {
            FormItem record = null;
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@MemberID", memberID);
            paraList.Add("@EventID", eventID);
            string sql = @" SELECT * FROM formitem where FormID in 
                            (
	                            SELECT ID FROM [Form] WHERE SourceID=@EventID
                            )
                            AND Email IN 
                            (
	                            SELECT Email FROM MemberShip WHERE ID=@MemberID
                            )";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable recordTable = db.GetDataTable(sql, paraList);
            if (recordTable == null || recordTable.Rows.Count <= 0)
                return record;
            DataRow dr = recordTable.Rows[0];

            record = new FormItem()
            {
                ID = (long)dr["ID"],
                FormID = (long)dr["FormID"],
                Name = dr["Name"].ToString().Trim(),
                Sex = dr["Sex"].ToString().Trim(),
                Email = dr["Email"].ToString().Trim(),
                CheckStatus = (byte)dr["CheckStatus"]
                //IssueStart = dr["IssueStart"] as DateTime?,
                //IssueEnd = dr["IssueEnd"] as DateTime?,
                //IssueDate = dr["IssueDate"] as DateTime?,
                //CustomIcon = (bool)dr["CustomIcon"],
                //DateKind = dr["DateKind"].ToString().Trim(),
                //DateStart = dr["DateStart"] as DateTime?,
                //DateEnd = dr["DateEnd"] as DateTime?,
                //Icon = dr["Icon"].ToString().Trim(),
                //Summary = dr["Summary"].ToString(),
                //Creator = (long)dr["Creator"],
                //PageSN = dr["PageSN"].ToString()
            };

            return record;
        }
        public static List<ViewModels.MemberCollectionViewModel> GetMemberFavitoryPages(long memberID)
        {
            string SearchMenuRange = "AND [Pages].MenuID IN (SELECT [ID] FROM Menus WHERE DataType IN ('Article','Event'))";
            int maxRowCount = 20;
            int addRowCount = 0;
            List<ViewModels.MemberCollectionViewModel> collectionList = new List<ViewModels.MemberCollectionViewModel>();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@MemberID", memberID);
            paraList.Add("@CategoryType", FavorityType);
            //string sql = string.Format(@" SELECT Pages.*, 0 AS ViewSort FROM Pages 
            //                    JOIN Zones ON Zones.PageNo = Pages.No
            //                    JOIN Cards ON Cards.ZoneNo = Zones.No
            //                    JOIN[Article] ON[Article].CardNo = Cards.No
            //                    JOIN[ArticleToCategory] ON[ArticleToCategory].ArticleID =[Article].ID
            //                    JOIN[MemberShipSetting] ON[MemberShipSetting].CategoryID =[ArticleToCategory].CategoryID
            //                WHERE [MemberShipSetting].MemberShipID = @MemberID and[MemberShipSetting].Type = @CategoryType AND [Pages].ShowStatus=1  {0}
            //                UNION
            //                SELECT Pages.*, 0 AS ViewSort FROM Pages
            //                    JOIN Zones ON Zones.PageNo = Pages.No
            //                    JOIN Cards ON Cards.ZoneNo = Zones.No
            //                    JOIN[Events] ON[Events].CardNo = Cards.No
            //                    JOIN[EventToCategory] ON[EventToCategory].EventID =[Events].ID
            //                    JOIN[MemberShipSetting] ON[MemberShipSetting].CategoryID =[EventToCategory].CategoryID
            //                WHERE [MemberShipSetting].MemberShipID = @MemberID and[MemberShipSetting].Type = @CategoryType AND [Pages].ShowStatus=1 {0}", SearchMenuRange);
            string sql = string.Format(@" SELECT Pages.*, 0 AS ViewSort FROM Pages 
                                JOIN Zones ON Zones.PageNo = Pages.No
                                JOIN Cards ON Cards.ZoneNo = Zones.No
                                JOIN[Article] ON[Article].CardNo = Cards.No
                                JOIN[ArticleToCategory] ON[ArticleToCategory].ArticleID =[Article].ID
                                JOIN[MemberShipSetting] ON[MemberShipSetting].CategoryID =[ArticleToCategory].CategoryID
                            WHERE [MemberShipSetting].MemberShipID = @MemberID and[MemberShipSetting].Type = @CategoryType AND [Pages].ShowStatus=1  {0}
                            UNION
                            SELECT Pages.*, 0 AS ViewSort FROM Pages
                                JOIN Zones ON Zones.PageNo = Pages.No
                                JOIN Cards ON Cards.ZoneNo = Zones.No                               
                                JOIN[MemberShipSetting] ON[MemberShipSetting].CategoryID =[EventToCategory].CategoryID
                            WHERE [MemberShipSetting].MemberShipID = @MemberID and[MemberShipSetting].Type = @CategoryType AND [Pages].ShowStatus=1 {0}", SearchMenuRange);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable firstTable = db.GetDataTable(sql, paraList);
            if (firstTable != null)
            {
                foreach (DataRow row in firstTable.Rows)
                {
                    addRowCount++;
                    collectionList.Add(new ViewModels.MemberCollectionViewModel()
                    {
                        No = long.Parse(row["No"].ToString()),
                        MenuID = long.Parse(row["MenuID"].ToString()),
                        Title = row["Title"].ToString(),
                        SN = row["SN"].ToString(),
                        ImgSrc = "",
                        ImgAlt = "",
                        LinkUrl = "",
                        Summary = ""
                    });
                    if (addRowCount >= maxRowCount)
                        break;
                }
            }
            if (collectionList.Count < maxRowCount)
            {
                List<Areas.Backend.Models.CategoryModels> favCategories = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(memberID, FavorityType);
                paraList.Clear();
                if (favCategories != null && favCategories.Count > 0)
                {
                    sql = "";

                    foreach (Areas.Backend.Models.CategoryModels favCategory in favCategories)
                    {
                        if (!string.IsNullOrEmpty(sql)) sql += " UNION ";
                        //sql += string.Format(@"SELECT [Pages].* FROM [Pages] 
                        //                        JOIN Zones ON Zones.PageNo = [Pages].No
                        //                        JOIN Cards ON Cards.ZoneNo = Zones.No
                        //                        WHERE Cards.No in 
                        //                        (

                        //                            SELECT CardNo FROM[Article] WHERE[Article].ID IN
                        //                              (SELECT SourceNo FROM SEO WHERE Keywords LIKE '%'+@Keyword_{0}+'%')
                        //                                UNION
                        //                            SELECT CardNo FROM[Events] WHERE[Events].ID IN
                        //                              (SELECT SourceNo FROM SEO WHERE Keywords LIKE '%'+@Keyword_{0}+'%')
                        //                        ) AND [Pages].ShowStatus=1 {1}", favCategory.ID, SearchMenuRange);
                        sql += string.Format(@"SELECT [Pages].* FROM [Pages] 
                                                JOIN Zones ON Zones.PageNo = [Pages].No
                                                JOIN Cards ON Cards.ZoneNo = Zones.No
                                                WHERE Cards.No in 
                                                (

                                                    SELECT CardNo FROM[Article] WHERE[Article].ID IN
                                                      (SELECT SourceNo FROM SEO WHERE Keywords LIKE '%'+@Keyword_{0}+'%')
                                                        AND [Pages].ShowStatus=1 {1}", favCategory.ID, SearchMenuRange);
                        paraList.Add(string.Format("@Keyword_{0}", favCategory.ID), favCategory.Title);
                    }
                    if (!string.IsNullOrEmpty(sql))
                    {
                        DataTable secondTable = db.GetDataTable(sql, paraList);
                        if (secondTable != null)
                        {
                            foreach (DataRow row in secondTable.Rows)
                            {
                                addRowCount++;
                                collectionList.Add(new ViewModels.MemberCollectionViewModel()
                                {
                                    No = long.Parse(row["No"].ToString()),
                                    MenuID = long.Parse(row["MenuID"].ToString()),
                                    Title = row["Title"].ToString(),
                                    SN = row["SN"].ToString(),
                                    ImgSrc = "",
                                    ImgAlt = "",
                                    LinkUrl = "",
                                    Summary = ""
                                });
                                if (addRowCount >= maxRowCount)
                                    break;
                            }
                        }
                    }
                }
            }
            if (collectionList.Count < maxRowCount)
            {
                sql = string.Format(@" SELECT top {0} Pages.* FROM Pages WHERE [Pages].ShowStatus=1 {1} ORDER BY NEWID() ", maxRowCount.ToString(), SearchMenuRange);
                DataTable thirdTable = db.GetDataTable(sql);
                if (thirdTable != null)
                {
                    foreach (DataRow row in thirdTable.Rows)
                    {
                        addRowCount++;
                        collectionList.Add(new ViewModels.MemberCollectionViewModel()
                        {
                            No = long.Parse(row["No"].ToString()),
                            MenuID = long.Parse(row["MenuID"].ToString()),
                            Title = row["Title"].ToString(),
                            SN = row["SN"].ToString(),
                            ImgSrc = "",
                            ImgAlt = "",
                            LinkUrl = "",
                            Summary = ""
                        });
                        if (addRowCount >= maxRowCount)
                            break;
                    }
                }
            }
            return collectionList;

        }

        public static void sendMobileVerifyCode(long MemberID, string captcha)
        {
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $@"UPDATE MemberShip SET MobileVerifyCode='{captcha}', VerifyTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE ID='{MemberID}' ";
                conn.Execute(sql);
            }
        }

        public static VerifyResult MobileVerify(MemberShipModels member, string captcha)
        {
            if (member.MobileVerifyCode == captcha)
            {
                TimeSpan ts = DateTime.Now - DateTime.Parse(member.VerifyTime);
                var TimeDiff = ts.TotalSeconds;
                if (TimeDiff > 60)
                {
                    return VerifyResult.VerifyExpired;
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
                    {
                        string sql = $@"UPDATE MemberShip SET MobileVerifyCode='000000', VerifyTime = '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' WHERE ID='{member.ID}' ";
                        conn.Execute(sql);
                    }
                    return VerifyResult.Success;
                }
            }
            return VerifyResult.DataError;
        }
        public static void MobileVerifyLog(long MemberID, string Mobile, string VerifyCode, string Status)
        {
            string TimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $@"INSERT INTO MobileVerifyLogs (MemberID, Mobile, VerifyTime, VerifyCode, Status) 
                                VALUES('{MemberID}' ,'{Mobile}' ,'{TimeNow}' ,'{VerifyCode}' ,'{Status}' ) ";
                conn.Execute(sql);
            }
        }
    }
    public enum VerifyResult
    {
        Success = 0,
        DataError = 1,
        IsVerified = 2,
        NotVerified = 3,
        NotNeedReset = 4,
        ModifyVerifiedSuccess = 5,
        VerifyExpired = 6
    }
    public enum CollectionResult
    {
        NotLogin = 0,
        PageErrror = 99,
        NoCollection = 1,
        Collectioned = 2
    }
}