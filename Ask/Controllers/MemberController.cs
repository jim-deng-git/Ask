using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Common;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WorkV3.Models.Repository;

namespace WorkV3.Controllers
{
    public class MemberController : Controller
    {
        const string LoginDefaultPage = "Desktop";
        string IdentityType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Identity.ToString();
        string FavorityType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Favority.ToString();
        string CareerType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Career.ToString();
        string EducationType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Education.ToString();
        string MarriageType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.Marriage.ToString();
        string ChildrenStatusType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.ChildrenStatus.ToString();
        string MilitaryServiceType = WorkV3.Areas.Backend.Models.DataAccess.CategoryType.MilitaryService.ToString();
        const string NotVerifiedMessage = "您的會員資料尚未完成電子郵件驗證，請先完成電子郵件驗證流程!";
        const string AccountOrEmailErrorMessage = "帳號或E-Mail錯誤!";
        const string ParameterErrorMessage = "參數錯誤!";
        const string VerifiedErrorMessage = "您已驗證過，無需重複驗證!";
        const string NotNeedResetErrorMessage = "您尚未申請重設密碼!";
        const string ForgetSuccessMessage = "請至您的註冊信箱收取忘記密碼驗證信，並依流程重設密碼，即可重新登入網站會員，謝謝!";
        const string ForgetVerifySuccessMessage = "您已驗證通過，成功註冊為我們的會員，謝謝!";
        const string ModifyVerifySuccessMessage = "您已驗證完成，謝謝!";
        const string ResetPwdSuccessMessage = "重設密碼成功，謝謝!";
        const string VerifiedMailSuccessMessage = "驗證信已寄出，請至您的信箱收信!";
        const string MobileVerifyErrorMessage = "驗證碼錯誤!";
        const string MobileVerifyExpiredMessage = "驗證碼已過期!請重新發送，謝謝!";

        private MemberShipRepository memberShipRepo = new MemberShipRepository();

        #region Login
        [HttpGet]
        public ActionResult Login(CardsModels model)
        {
            Member curUser = Member.Current;
            if (curUser != null)
            {
                curUser.Logout();
                Response.Redirect ("Login");
                return View($"Style{ model.StylesID }-login");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["prev"]))
            {
                ViewBag.PrevUrl = HttpUtility.UrlDecode(Request.QueryString["prev"]);
                TempData["Prev"] = HttpUtility.UrlDecode(Request.QueryString["prev"]);
            }
            SitePage page = CardsDAO.GetPage(model.No);            
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.RememberMe = false;
            // add fb oauth url
            ViewBag.FBUrl = GetFBOauthUrl(page.SiteID, page.SiteSN);
            ViewBag.SocialSet = Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetSocialItems(page.SiteID);
            LoadMemberShipSet(model.SiteID);

            HttpCookie accountCookie = Request.Cookies["cc"];            
            if (accountCookie != null && !String.IsNullOrEmpty(accountCookie.Value)) {                
                try {
                    Dictionary<string, string> accountItem = JsonConvert.DeserializeObject<Dictionary<string, string>>(accountCookie.Value.Decrypt());
                    ViewBag.Account = accountItem["Account"];
                    ViewBag.Password = accountItem["Password"];
                    ViewBag.RememberMe = true;
                } catch {

                }
            }

            return View($"Style{ model.StylesID }-login");
        }

        public void Logout(CardsModels model)
        {
            Member curUser = Member.Current;
            if (curUser != null)
            {
                curUser.Logout();
            }
            Response.Redirect("Login");
           
        }
        [HttpPost]
        public ActionResult Login(CardsModels model, Captcha captcha, string account, string password, bool rememberMe) {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.RememberMe = rememberMe;

            ViewBag.Account = account;
            ViewBag.Password = password;
            LoadMemberShipSet(model.SiteID);
            // add fb oauth url
            ViewBag.FBUrl = GetFBOauthUrl(page.SiteID, page.SiteSN);

            if (!captcha.Validate()) {
                ViewBag.Script = $"Component.showElementError('#captcha', '* 驗證碼填寫錯誤');";
                return View($"Style{ model.StylesID }-login");
            }

            HttpCookie accountCookie = new HttpCookie("cc");
            DateTime now = DateTime.Now;
            if (rememberMe) {
                var accountItem = new { Account = account, Password = password };                
                accountCookie.Value = JsonConvert.SerializeObject(accountItem).Encrypt();
                accountCookie.Expires = now.AddYears(5);
            } else
                accountCookie.Expires = now.AddDays(-1);
            Response.Cookies.Add(accountCookie);
            if (!string.IsNullOrEmpty(Request.QueryString["prev"]))
            {
                ViewBag.PrevUrl = HttpUtility.UrlDecode(Request.QueryString["prev"]);
                TempData["Prev"] = HttpUtility.UrlDecode(Request.QueryString["prev"]);
            }
            else {
                if (TempData["Prev"] != null)
                {
                    TempData["Prev"] = TempData["Prev"];
                    ViewBag.PrevUrl = TempData["Prev"];
                }
            }
            LoginStatus loginStatus = Member.Login(model.SiteID, account, password);
            string prevUrl = Request.QueryString["prev"];
            if (ViewBag.PrevUrl != null)
                prevUrl = ViewBag.PrevUrl ;
            if (loginStatus == LoginStatus.成功)
            {
                MemberShipLoginLogsDAO.AddLoginLogs(new MemberShipLoginLogModel() {
                    MemberShipID = Member.Current.ID,
                    Browser = Request.Browser.Browser,
                    UserAgent = Request.UserAgent,
                    IP = WorkLib.GetItem.IPAddr(),
                    AddTime = DateTime.Now,
                    IPNum = (long)WorkLib.GetItem.GetIPNum()
                });
                if (!String.IsNullOrEmpty(prevUrl))
                    Response.Redirect(prevUrl);
                else
                {
                    //Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN, PageSN = "CollectionList" }));
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN, PageSN = LoginDefaultPage }));
                }
            }
            else
            {
                if (loginStatus == LoginStatus.EMail未驗證)
                {
                    if (String.IsNullOrEmpty(prevUrl))
                        prevUrl = Url.Action("Index", "Home", new { SiteSN = page.SiteSN, PageSN = LoginDefaultPage });

                    ViewBag.Script = @"ShowConfirmMessage('" + prevUrl + @"');";
                    //if (!String.IsNullOrEmpty(prevUrl))
                    //    Response.Redirect(prevUrl);
                    //else
                    //{
                    //    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN, PageSN = "Index" }));
                    //}
                }
                else if(loginStatus == LoginStatus.手機未驗證)
                {
                    if (String.IsNullOrEmpty(prevUrl))
                        prevUrl = @Url.Action("Index", "Home", new { SiteSN = page.SiteSN, PageSN = "MobileVerify", MemberID = Member.Current.ID });

                    ViewBag.Script = @"ShowMobileConfirmMessage('" + prevUrl + @"');";
                }
                else
                {
                    ViewBag.Script = $"Component.showElementError('#account', '* 用戶名不存在或密碼錯誤');";
                }
            }
            
            return View($"Style{ model.StylesID }-login");
        }
        private Areas.Backend.Models.MemberShipRegSetModels  LoadMemberShipSet(long SiteID)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            string loginText = "";
            Areas.Backend.Models.MemberShipRegSetModels MemberSetModel = Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetItem(SiteID);
            if (MemberSetModel == null || !MemberSetModel.IsOpenReg)
            {
                //var url = Url.Action("Index", "Home", new { SiteSN = pageCache.SiteSN });
                var url = Url.RouteUrl("FrontSitePage", new { SiteSN = pageCache.SiteSN, PageSn = "Index" });
                Response.Redirect(url);
            }
            else
            {
                if (MemberSetModel.LoginTypeList.Count <= 0)
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = pageCache.SiteSN }));
                else
                {
                    foreach (Areas.Backend.Models.MemberShipLoginType loginType in MemberSetModel.LoginTypeList)
                    {
                        switch (loginType)
                        {
                            case Areas.Backend.Models.MemberShipLoginType.Email:
                                loginText += "Email/";
                                break;
                            case Areas.Backend.Models.MemberShipLoginType.ID:
                                loginText += "身分證字號/";
                                break;
                            case Areas.Backend.Models.MemberShipLoginType.InputAccount:
                                loginText += "帳號/";
                                break;
                            case Areas.Backend.Models.MemberShipLoginType.Mobile:
                                loginText += "手機/";
                                break;
                        }
                    }
                    ViewBag.MemberSetModel = MemberSetModel;
                    loginText = loginText.Trim('/');
                }
            }
            ViewBag.LoginPlaceHolder = loginText;

            return MemberSetModel;
        }
        #endregion
        #region 忘記密碼
        [HttpGet]
        public ActionResult Forget(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;

            MemberShipModels item = new MemberShipModels();

            return View($"Style{ model.StylesID }-forget", item);
        }
        [HttpPost]
        public ActionResult Forget(long siteId,CardsModels model, Captcha captcha, MemberShipModels item)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;

            if (!captcha.Validate())
            {
                ViewBag.Script = $"Component.showElementError('#captcha', '* 驗證碼填寫錯誤');";
                return View($"Style{ model.StylesID }-register", item);
            }

            item.ID = WorkLib.GetItem.NewSN();
            item.SiteID = page.SiteID;
            item.VerifyCode = Guid.NewGuid().ToString();
            item.VerifyTime = "";
            string vMessage = "";
            VerifyResult vResult = MemberShipDAO.ForgetMemberShip(siteId,pageCache, item.Account, item.Email);
            switch (vResult)
            {
                case VerifyResult.DataError:
                    vMessage = AccountOrEmailErrorMessage;
                    break;
                case VerifyResult.NotVerified:
                    vMessage = NotVerifiedMessage;
                    break;
                case VerifyResult.Success:
                    vMessage = ForgetSuccessMessage;
                    break;
            }
            ViewBag.Message = vMessage;
            ViewBag.Exit = true;
            return View($"Style{ model.StylesID }-forget", item);
        }
        #endregion
        #region 會員E-Mail驗證
        [HttpGet]
        public ActionResult Verify(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;

            string verifyMessage = "";
            if (string.IsNullOrEmpty(Request.QueryString["M"]) ||
                string.IsNullOrEmpty(Request.QueryString["K"]))
            {
                verifyMessage = ParameterErrorMessage;
            }
            else
            {
                try
                {
                    long memID = long.Parse(Request.QueryString["M"]);
                    string verifyCode =　new Guid( Request.QueryString["K"]).ToString();
                    VerifyResult verifyResult = MemberShipDAO.VerifyMemberShip(memID, verifyCode);
                    switch (verifyResult)
                    {
                        case VerifyResult.DataError:
                            verifyMessage = ParameterErrorMessage;
                            break;
                        case VerifyResult.IsVerified:
                            verifyMessage = VerifiedErrorMessage;
                            break;
                        case VerifyResult.Success:
                            verifyMessage = ForgetVerifySuccessMessage;
                            break;
                        case VerifyResult.ModifyVerifiedSuccess:
                            verifyMessage = ModifyVerifySuccessMessage;
                            break;
                    }
                }
                catch
                {
                    verifyMessage = ParameterErrorMessage;
                }
            }
            ViewBag.VerifyMessage = verifyMessage;
            return View($"Style{ model.StylesID }-verify");
        }
        #endregion
        #region 會員重發驗證信
        [HttpGet]
        public ActionResult ResentMail(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;

            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }

            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            return View($"Style{ model.StylesID }-resentmail", item);
        }
        [HttpPost]
        public ActionResult ResentMail(long siteId,CardsModels model, string ID)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;

            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }

            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            bool IsExit = false;
            string vMessage = "";
            IsExit = MemberShipDAO.ResentMemberVerifyMail(siteId,pageCache, item);
            if (!IsExit)
            {
                vMessage = "驗證信發送失敗!";
            }
            else
            {
                vMessage = VerifiedMailSuccessMessage;
            }
            ViewBag.Message = vMessage;
            ViewBag.Exit = IsExit;
            return View($"Style{ model.StylesID }-resentmail", item);
        }
        #endregion
        #region 會員重設密碼
        [HttpGet]
        public ActionResult ResetPWD(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            MemberShipModels item = null;
            string vMessage = "";
            if (string.IsNullOrEmpty(Request.QueryString["M"]))
            {
                vMessage = ParameterErrorMessage;
            }
            else
            {
                try
                {
                    long memID = long.Parse(Request.QueryString["M"]);
                    ViewBag.MemberID = memID.ToString();
                    VerifyResult verifyResult = MemberShipDAO.LoadResetMemberShip(memID, out item);
                    switch (verifyResult)
                    {
                        case VerifyResult.DataError:
                            vMessage = ParameterErrorMessage;
                            break;
                        case VerifyResult.NotNeedReset:
                            vMessage =  NotNeedResetErrorMessage;
                            break;
                        case VerifyResult.NotVerified:
                            vMessage = NotVerifiedMessage;
                            break;
                    }
                }
                catch
                {
                    vMessage = ParameterErrorMessage;
                }
            }
            ViewBag.Message = vMessage;
            return View($"Style{ model.StylesID }-resetpwd");
        }
        [HttpPost]
        public ActionResult ResetPWD(CardsModels model, Captcha captcha, string ID, string password, string againpassword)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.Password = password;
            ViewBag.AgainPassword = againpassword;
            ViewBag.MemberID = ID;

            if (password != againpassword)
            {
                ViewBag.Script = $"Component.showElementError('#password', '* 二次密碼輸入不相符');";
                return View($"Style{ model.StylesID }-resetpwd");
            }
            if (!captcha.Validate())
            {
                ViewBag.Script = $"Component.showElementError('#captcha', '* 驗證碼填寫錯誤');";
                return View($"Style{ model.StylesID }-resetpwd");
            }

            bool IsExit = false;
            string vMessage = "";
            VerifyResult verifyResult = MemberShipDAO.ResetPWD(long.Parse(ID), password);
            switch (verifyResult)
            {
                case VerifyResult.DataError:
                    vMessage = ParameterErrorMessage;
                    break;
                case VerifyResult.NotNeedReset:
                    vMessage = NotNeedResetErrorMessage;
                    break;
                case VerifyResult.NotVerified:
                    vMessage = NotVerifiedMessage;
                    break;
                case VerifyResult.Success:
                    IsExit = true;
                    vMessage = ResetPwdSuccessMessage;
                    break;
            }
            ViewBag.Message = vMessage;
            ViewBag.Exit = IsExit;
            return View($"Style{ model.StylesID }-resetpwd");
        }
        #endregion
        #region 會員變更密碼
        [HttpGet]
        public ActionResult ChangePWD(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }

            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            return View($"Style{ model.StylesID }-changepwd", item);
        }
        [HttpPost]
        public ActionResult ChangePWD(long siteId,CardsModels model, string old_password, string password, string againpassword)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.Password = password;
            ViewBag.AgainPassword = againpassword;
            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }
            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            if (old_password != item.Password)
            {
                ViewBag.Script = $"Component.showElementError('#old_password', '* 舊密碼輸入不正確');";
                return View($"Style{ model.StylesID }-changepwd");
            }
            if (password != againpassword)
            {
                ViewBag.Script = $"Component.showElementError('#password', '* 二次密碼輸入不相符');";
                return View($"Style{ model.StylesID }-changepwd");
            }

            bool IsExit = false;
            string vMessage = "";
            bool vResult = MemberShipDAO.ModifyMemberShipPWD(siteId,pageCache, curUser.ID, password);
            if (!vResult)
            {
                vMessage = "密碼更新失敗";
            }
            else
            {
                vMessage = "密碼修改成功";
                IsExit = true;
            }
            ViewBag.Message = vMessage;
            ViewBag.Exit = IsExit;
            return View($"Style{ model.StylesID }-changepwd");
        }
        #endregion
        #region FB 登入
        private string GetFBOauthUrl(long siteId, string siteSN)
        {
            string app_id = System.Configuration.ConfigurationManager.AppSettings["FBAppID"].ToString();//WorkLib.GetItem.appSet("FBAppID").ToString();
            string scope = WorkLib.GetItem.appSet("AppScope").ToString();

            //20190923 Nina Add FB登入取後台設定值
            Areas.Backend.Models.MemberShipRegSocialSetModels socialSetItem = Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetSocialItem(siteId, MemberType.FB);
            if (socialSetItem != null)
                app_id = socialSetItem.AppID;

            string redirect_uri = string.Format("{0}://{1}{2}/w/{3}/FB", Request.Url.Scheme,
                    Request.Url.DnsSafeHost,
                    //Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString(),
                    Request.ApplicationPath == "/" ? "" : "/" + Request.ApplicationPath.Trim('/'),
                    siteSN);
            string tokenUrl = string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&scope={1}&auth_type=rerequest&redirect_uri={2}", app_id, scope, redirect_uri);
            return tokenUrl;
        }
        [HttpGet]
        public ActionResult FB(CardsModels model)
        {

            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.FBUrl = GetFBOauthUrl(page.SiteID, page.SiteSN);
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;

            string loginUrl = "~/w/" + page.SiteSN + "/Login";
            if (!string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                string code = Request.QueryString["code"];
                string app_id = System.Configuration.ConfigurationManager.AppSettings["FBAppID"].ToString();//WorkLib.GetItem.appSet("FBAppID").ToString();
                string secret = WorkLib.GetItem.appSet("FBSecret").ToString();

                //20190923 Nina Add FB登入取後台設定值
                Areas.Backend.Models.MemberShipRegSocialSetModels socialSetItem = Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetSocialItem(page.SiteID, MemberType.FB);
                if (socialSetItem != null)
                {
                    app_id = socialSetItem.AppID;
                    secret = socialSetItem.SecretKey;
                }

                string redirect_uri = string.Format("{0}://{1}{2}/w/{3}/FB", Request.Url.Scheme,
                        Request.Url.DnsSafeHost,
                        //Request.Url.Port == 80 ? "" : ":" + Request.Url.Port.ToString(),
                        Request.ApplicationPath == "/" ? "" : "/" + Request.ApplicationPath.Trim('/'),
                        page.SiteSN);

                string getTokenUrl = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}"
                    , app_id, redirect_uri, secret, code);
                HttpWebRequest request = HttpWebRequest.Create(getTokenUrl) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 30000;

                string result = "";
                // 取得回應資料
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                JObject fbRequestTokenResult = JObject.Parse(result);
                JToken errorToken = fbRequestTokenResult.SelectToken("error");
                if (errorToken != null)
                {
                    //ViewBag.Message = "您尚未授權的資料，請重新授權。";
                    //return View($"Style{ model.StylesID }-FB");
                    TempData["Message"] = "您尚未授權的資料，請重新授權。";
                    Response.Redirect(loginUrl);
                }
                ViewModels.FBToken token = JsonConvert.DeserializeObject<ViewModels.FBToken>(result);
                //cover,
                string getProfileUrl = string.Format("https://graph.facebook.com/me?access_token={0}&fields=id,name,birthday,email,gender,picture", token.access_token);

                request = HttpWebRequest.Create(getProfileUrl) as HttpWebRequest;
                result = "";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }

                ViewModels.FBMember member = JsonConvert.DeserializeObject<ViewModels.FBMember>(result);
                if (string.IsNullOrEmpty(member.email))
                {
                    //ViewBag.Message = "您未同意取得您的電子郵件信箱，請使用別種方式登入。";
                    //return View($"Style{ model.StylesID }-FB");

                    TempData["Message"] = "您未同意取得您的電子郵件信箱，請使用別種方式登入。";
                    Response.Redirect(loginUrl);
                }

                MemberShipDAO.BindSoicalMember(MemberType.FB, page.SiteID, member);
                LoginStatus loginStatus = Member.Login(model.SiteID, member.id);
                if (loginStatus == LoginStatus.成功)
                {
                    MemberShipLoginLogsDAO.AddLoginLogs(new MemberShipLoginLogModel()
                    {
                        MemberShipID = Member.Current.ID,
                        Browser = Request.Browser.Browser,
                        UserAgent = Request.UserAgent,
                        IP = WorkLib.GetItem.IPAddr(),
                        AddTime = DateTime.Now,
                        IPNum = (long)WorkLib.GetItem.GetIPNum()
                    });
                    string prevUrl = Request.QueryString["prev"];
                    if(TempData["Prev"]!= null )
                        prevUrl = TempData["Prev"].ToString();
                    if (!String.IsNullOrEmpty(prevUrl))
                        Response.Redirect(prevUrl);
                    else
                    {
                        Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN, PageSN = "Desktop" }));
                    }
                }
                else
                {
                    ViewBag.Script = $"Component.alert('* 認證失敗');";
                }
                ViewBag.MemberName = member.name;
            }
            else
            {
                //ViewBag.Message = "您尚未授權的資料，請重新授權。";
                TempData["Message"] = "您尚未授權的資料，請重新授權。";
                Response.Redirect(loginUrl);
            }
            TempData["Message"] = "您尚未授權的資料，請重新授權。";
            Response.Redirect(loginUrl);
            return View($"Style{ model.StylesID }-Login");

        }
        [HttpGet]
        public ActionResult Register(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.PageID = page.PageNo;
            ViewBag.MenuID = page.MenuID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);
            ViewBag.IdentityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewBag.ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            ViewBag.MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);
            WorkV3.Areas.Backend.Models.AgreeStatementSetModels agreeSetting = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetStatementItems();
            ViewBag.AgreeSetting = agreeSetting;
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(model.SiteID);
            if (regModel != null)
            {
                if (regModel.RegType == Areas.Backend.Models.MemberShipRegType.Social)
                {
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN }));
                }
            }
            MemberShipModels item = new MemberShipModels();

            return View($"Style{ model.StylesID }-register", item);
        }
        #endregion
        #region 會員註冊說明
        [HttpGet]
        public ActionResult SecurityDesc(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            return View($"Style{ model.StylesID }-SecurityDesc");
        }
        #endregion
        [HttpPost]
        public ActionResult Register(CardsModels model, Captcha captcha, MemberShipModels item, string[] IdentityList, string[] FavorityList, string OrderEpaperTypeList)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.PageID = page.PageNo;
            ViewBag.MenuID = page.MenuID;
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(model.SiteID);
            if (regModel != null)
            {
                if (regModel.RegType == Areas.Backend.Models.MemberShipRegType.Social)
                {
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN }));
                }
            }
            var IdentityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            var FavorityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            var CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            var MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            var EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            var ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            var MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);
            ViewBag.IdentityCategories = IdentityCategories;
            ViewBag.FavorityCategories = FavorityCategories;
            ViewBag.CareerCategories = CareerCategories;
            ViewBag.MarriageCategories = MarriageCategories;
            ViewBag.EducationCategories = EducationCategories;
            WorkV3.Areas.Backend.Models.AgreeStatementSetModels agreeSetting = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetStatementItems();
            ViewBag.AgreeSetting = agreeSetting;
            if (IdentityList != null)
            {
                List<Areas.Backend.Models.CategoryModels> MemberSelectIdentityList = new List<Areas.Backend.Models.CategoryModels>();
                foreach (string Identity in IdentityList)
                {
                    MemberSelectIdentityList.Add(new Areas.Backend.Models.CategoryModels() {
                        ID = long.Parse(Identity)
                    });
                }
                item.IdenityTypeList = MemberSelectIdentityList;
            }
            if (FavorityList != null)
            {
                List<Areas.Backend.Models.CategoryModels> MemberSelectFavorityList = new List<Areas.Backend.Models.CategoryModels>();
                foreach (string Favority in FavorityList)
                {
                    MemberSelectFavorityList.Add(new Areas.Backend.Models.CategoryModels()
                    {
                        ID = long.Parse(Favority)
                    });
                }
                item.FavorityList = MemberSelectFavorityList;
            }
            if (regModel.IsNeedVerifyCode)
            {
                if (!captcha.Validate())
                {
                    ViewBag.Script = $"Component.showElementError('#captcha', '* 驗證碼填寫錯誤');";
                    return View($"Style{ model.StylesID }-register", item);
                }
            }
            if (!MemberShipDAO.CheckEmail(model.SiteID, item.Email, null))
            {
                ViewBag.Script = $"Component.showElementError('#Email', '* Email已存在');";
                return View($"Style{ model.StylesID }-register", item);
            }
            if (agreeSetting!= null && agreeSetting.IsNeedAgree && !item.AgreeDesc)
            {
                ViewBag.Script = $"Component.showElementError('#Agree', '* 請同意宣告事項');";
                return View($"Style{ model.StylesID }-register", item);
            }

            bool Address_IsNeedValue = regModel.RegColumnSets.Where(x => x.ColumnName == "Address").First().IsNeedValue;
            if(Address_IsNeedValue)
            {
                if(item.Regions != null)
                {
                    var RegionsCodes = item.Regions.Replace('[', ' ').Replace(']', ' ').Split(',');
                    if(RegionsCodes.Count() == 1 && Int32.Parse(RegionsCodes[0]) == 1)
                    {
                        ViewBag.Script = $"Component.showElementError('#div_Address', '* 此欄位為必填');";
                        return View($"Style{ model.StylesID }-register", item);
                    }
                }
                else
                {
                    ViewBag.Script = $"Component.showElementError('#div_Address', '* 此欄位為必填');";
                    return View($"Style{ model.StylesID }-register", item);
                }

                if(item.Address == null)
                {
                    ViewBag.Script = $"Component.showElementError('#div_Address', '* 此欄位為必填');";
                    return View($"Style{ model.StylesID }-register", item);
                }
            }

            if (item.sameAddress)
            {
                item.PermanentRegions = item.Regions;
                item.PermanentAddress = item.Address;
            }

            bool PermanentAddress_IsNeedValue = regModel.RegColumnSets.Where(x => x.ColumnName == "PermanentAddress").First().IsNeedValue;
            if (PermanentAddress_IsNeedValue && !item.sameAddress)
            {
                if (item.PermanentRegions != null)
                {
                    var RegionsCodes = item.PermanentRegions.Replace('[', ' ').Replace(']', ' ').Split(',');
                    if (RegionsCodes.Count() == 1 && Int32.Parse(RegionsCodes[0]) == 1)
                    {
                        ViewBag.Script = $"Component.showElementError('#div_PermanentAddress', '* 此欄位為必填');";
                        return View($"Style{ model.StylesID }-register", item);
                    }
                }
                else
                {
                    ViewBag.Script = $"Component.showElementError('#div_PermanentAddress', '* 此欄位為必填');";
                    return View($"Style{ model.StylesID }-register", item);
                }

                if (item.PermanentAddress == null)
                {
                    ViewBag.Script = $"Component.showElementError('#div_PermanentAddress', '* 此欄位為必填');";
                    return View($"Style{ model.StylesID }-register", item);
                }
            }

            if (item.MilitaryService == "505")
            {
                item.MilitaryService = "505," + item.MilitaryService_Y + ',' + item.MilitaryService_M;
            }

            if (!string.IsNullOrWhiteSpace(item.BankBook))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.BankBook);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fBankBook"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.BankBook = string.Empty;
                    }
                    else
                    {
                        item.BankBook = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, page.SiteID, "Member");
                    }
                }
                else
                {
                    item.BankBook = imgModel.Img;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.IDcardPhoto_Front))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.IDcardPhoto_Front);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fIDcardPhoto_Front"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.IDcardPhoto_Front = string.Empty;
                    }
                    else
                    {
                        item.IDcardPhoto_Front = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, page.SiteID, "Member");
                    }
                }
                else
                {
                    item.IDcardPhoto_Front = imgModel.Img;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.IDcardPhoto_Back))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.IDcardPhoto_Back);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fIDcardPhoto_Back"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.IDcardPhoto_Back = string.Empty;
                    }
                    else
                    {
                        item.IDcardPhoto_Back = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, page.SiteID, "Member");
                    }
                }
                else
                {
                    item.IDcardPhoto_Back = imgModel.Img;
                }
            }
            if(item.IDcardPhoto_Front == null && item.IDcardPhoto_Back == null)
            {
                item.IDcardPhoto = null;
            }
            else
            {
                item.IDcardPhoto = item.IDcardPhoto_Front + ',' + item.IDcardPhoto_Back;
            }
            if(item.sameAddress)
            {
                item.PermanentRegions = item.Regions;
                item.PermanentAddress = item.Address;
            }

            item.ID = WorkLib.GetItem.NewSN();
            item.SiteID = page.SiteID;
            item.VerifyCode = Guid.NewGuid().ToString();
            item.VerifyTime = "";
            item.Phone = "";

            ViewBag.AgreeSetting = agreeSetting;
            MemberShipDAO.RegisterMemberShip(pageCache, item);

            if (IdentityList != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(item.ID, IdentityType, IdentityList.ToList());
            }
            if (FavorityList != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(item.ID, FavorityType, FavorityList.ToList());
            }
            ViewBag.Exit = true;
            ViewBag.VerifyType = regModel.VerifyType;

            return View($"Style{ model.StylesID }-register", item);
        }

        [HttpGet]
        public ActionResult MyInfo(CardsModels model) {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.MenuID = page.MenuID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);
            ViewBag.IdentityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewBag.ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            ViewBag.MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);
            WorkV3.Areas.Backend.Models.AgreeStatementSetModels agreeSetting = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetStatementItems();
            ViewBag.AgreeSetting = agreeSetting;
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(model.SiteID);
            if (regModel != null)
            {
                if (regModel.RegType == Areas.Backend.Models.MemberShipRegType.Social)
                {
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN }));
                }
            }

            if (!string.IsNullOrEmpty(Request.QueryString["Resent"]))
            {
                if (Request.QueryString["Resent"] == "1")
                    ViewBag.ShowResent = "1";
            }
            Member curUser = Member.Current;
            if (curUser == null) {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }
            
            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            item.IdenityTypeList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(curUser.ID, IdentityType);
            item.FavorityList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(curUser.ID, FavorityType);
            //讀生活照
            if (!string.IsNullOrEmpty(item.DailyPhoto))
            {
                var DailyPhotoName = item.DailyPhoto.Split(',');
                string imgJson = "";
                for (int i = 0; i < DailyPhotoName.Length; i++)
                {
                    imgJson += JsonConvert.SerializeObject(new { ID = i, Img = DailyPhotoName[i] });
                    if (i != DailyPhotoName.Length - 1)
                        imgJson += ",";
                }

                ViewBag.Imgs = "[" + imgJson + "]";
            }

            return View($"Style{ model.StylesID }-myInfo", item);
        }

        [HttpPost]
        public ActionResult MyInfo(CardsModels model, MemberShipModels item, string[] IdentityList, string[] FavorityList, string OrderEpaperTypeList)
        {
            ViewBag.CheckFail = false;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.SiteID = page.SiteID;
            ViewBag.MenuID = page.MenuID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);
            ViewBag.IdentityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewBag.ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            ViewBag.MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);
            WorkV3.Areas.Backend.Models.AgreeStatementSetModels agreeSetting = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetStatementItems();
            ViewBag.AgreeSetting = agreeSetting;
            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(model.SiteID);
            if (regModel != null)
            {
                if (regModel.RegType == Areas.Backend.Models.MemberShipRegType.Social)
                {
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN }));
                }
            }
            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }
            MemberShipModels member_item = MemberShipDAO.GetItem(curUser.ID);
            if (!string.IsNullOrWhiteSpace(item.PushRecommandCode))
            {
                if(member_item.RecommandCode == item.PushRecommandCode)
                {
                    ViewBag.Script = $"Component.showElementError('#PushRecommandCode', '* 推薦人代碼不可為本人');";
                    ViewBag.CheckFail = true;
                    return View($"Style{ model.StylesID }-myInfo", item);
                }
                bool recommandCodeExist = MemberShipDAO.CheckPushRecommandCodeExist(member_item.PushRecommandCode);
                if (!recommandCodeExist)
                {
                    ViewBag.Script = $"Component.showElementError('#PushRecommandCode', '* 推薦人代碼不存在');";
                    ViewBag.CheckFail = true;
                    return View($"Style{ model.StylesID }-myInfo", item);
                }
            }

            //if (!captcha.Validate()) {
            //    ViewBag.Script = $"Component.showElementError('#captcha', '* 驗證碼填寫錯誤');";
            //    return View($"Style{ model.StylesID }-myInfo", item);
            //}

            item.ID = curUser.ID;
            item.SiteID = page.SiteID;
            item.Account = curUser.Account;
            item.LastResentVerifyMailTime = "";
            bool IsModifyEmail = false;
            if (!MemberShipDAO.ModifyMemberShip(item.SiteID,pageCache, item, out IsModifyEmail))
            {
                ViewBag.Script = $"Component.showElementError('#captcha', '* 會員資料有誤, 請重新填寫');";
                return View($"Style{ model.StylesID }-myInfo", item);
            }
            
            if (IdentityList != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(item.ID, IdentityType, IdentityList.ToList());
            }
            if (FavorityList != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(item.ID, FavorityType, FavorityList.ToList());
            }
            ViewBag.IsModifyEmail = IsModifyEmail;
            ViewBag.Exit = true;

            member_item.IdenityTypeList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(member_item.ID, IdentityType);
            member_item.FavorityList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(member_item.ID, FavorityType);

            //圖片
            if (!string.IsNullOrWhiteSpace(item.BankBook))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.BankBook);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fBankBook"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.BankBook = string.Empty;
                    }
                    else
                    {
                        item.BankBook = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, page.SiteID, "Member");
                    }
                }
                else
                {
                    item.BankBook = imgModel.Img;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.IDcardPhoto_Front))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.IDcardPhoto_Front);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fIDcardPhoto_Front"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.IDcardPhoto_Front = string.Empty;
                    }
                    else
                    {
                        item.IDcardPhoto_Front = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, page.SiteID, "Member");
                    }
                }
                else
                {
                    item.IDcardPhoto_Front = imgModel.Img;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.IDcardPhoto_Back))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.IDcardPhoto_Back);
                if (imgModel.ID == 0)
                {
                    HttpPostedFileBase postedFile = Request.Files["fIDcardPhoto_Back"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.IDcardPhoto_Back = string.Empty;
                    }
                    else
                    {
                        item.IDcardPhoto_Back = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, page.SiteID, "Member");
                    }
                }
                else
                {
                    item.IDcardPhoto_Back = imgModel.Img;
                }
            }
            if (item.IDcardPhoto_Front == null && item.IDcardPhoto_Back == null)
            {
                item.IDcardPhoto = null;
            }
            else
            {
                item.IDcardPhoto = item.IDcardPhoto_Front + ',' + item.IDcardPhoto_Back;
            }
            MemberShipDAO.SetItem(item, false);

            if (item.DailyPhoto != null)
            {
                var DailyPhotoName = item.DailyPhoto.Split(',');
                string imgJson = "";
                for (int i = 0; i < DailyPhotoName.Length; i++)
                {
                    imgJson += JsonConvert.SerializeObject(new { ID = i, Img = DailyPhotoName[i] });
                    if (i != DailyPhotoName.Length - 1)
                        imgJson += ",";
                }

                ViewBag.Imgs = "[" + imgJson + "]";
            }

            Member.RefreshMemberSession();
            return View($"Style{ model.StylesID }-myInfo", member_item);
        }
        [HttpGet]
        public ActionResult MySocialInfo(CardsModels model) {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);
            ViewBag.IdentityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewBag.ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            ViewBag.MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);
            Member curUser = Member.Current;
            if (curUser == null) {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }

            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(model.SiteID);
            if (regModel != null)
            {
                if (regModel.RegType == Areas.Backend.Models.MemberShipRegType.WebAccount)
                {
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN }));
                }
            }
            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            item.IdenityTypeList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(item.ID, IdentityType);
            item.FavorityList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(item.ID, FavorityType);

            return View($"Style{ model.StylesID }-MySocialInfo", item);
        }
        [HttpPost]
        public ActionResult MySocialInfo(CardsModels model, MemberShipModels item, string[] IdentityList, string[] FavorityList, string OrderEpaperTypeList)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);
            ViewBag.IdentityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            ViewBag.ChildrenStatusCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(ChildrenStatusType);
            ViewBag.MilitaryServiceCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MilitaryServiceType);

            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Index" }));
            }

            WorkV3.Areas.Backend.Models.MemberShipRegSetModels regModel = LoadMemberShipSet(model.SiteID);
            if (regModel != null)
            {
                if (regModel.RegType == Areas.Backend.Models.MemberShipRegType.WebAccount)
                {
                    Response.Redirect(Url.Action("Index", "Home", new { SiteSN = page.SiteSN }));
                }
            }

            MemberShipModels member_item = MemberShipDAO.GetItem(curUser.ID);
            if (!string.IsNullOrWhiteSpace(item.PushRecommandCode))
            {
                if (member_item.RecommandCode == item.PushRecommandCode)
                {
                    ViewBag.Script = $"Component.showElementError('#PushRecommandCode', '* 推薦人代碼不可為本人');";
                    ViewBag.CheckFail = true;
                    return View($"Style{ model.StylesID }-MySocialInfo", item);
                }
                bool recommandCodeExist = MemberShipDAO.CheckPushRecommandCodeExist(item.PushRecommandCode);
                if (!recommandCodeExist)
                {
                    ViewBag.Script = $"Component.showElementError('#PushRecommandCode', '* 推薦人代碼不存在');";
                    ViewBag.CheckFail = true;
                    return View($"Style{ model.StylesID }-MySocialInfo", item);
                }
                if (!MemberShipDAO.ModifyMemberShipRecommandCode(member_item.ID, item.PushRecommandCode))
                {
                    ViewBag.Script = $"Component.showElementError('#PushRecommandCode', '* 推薦人代碼儲存錯誤');";
                    return View($"Style{ model.StylesID }-MySocialInfo", item);
                }
            }

            if (IdentityList != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(member_item.ID, IdentityType, IdentityList.ToList());
            }
            if (FavorityList != null)
            {
                WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.SetSelectedItems(member_item.ID, FavorityType, FavorityList.ToList());
            }
            List<string> orderTypeList = new List<string>();
            if (!string.IsNullOrEmpty(OrderEpaperTypeList))
            {
                orderTypeList = OrderEpaperTypeList.Split(';').ToList();
            }


            bool IsModifyEmail = false;

            item.ID = curUser.ID;
            item.SiteID = page.SiteID;
            item.Account = curUser.Account;
            item.LastResentVerifyMailTime = "";
            if (!MemberShipDAO.ModifyMemberShip(item.SiteID,pageCache, item, out IsModifyEmail))
            {
                ViewBag.Script = $"Component.showElementError('#captcha', '* 會員資料有誤, 請重新填寫');";
                return View($"Style{ model.StylesID }-MySocialInfo", item);
            }
            ViewBag.Exit = true;

            member_item.IdenityTypeList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(member_item.ID, IdentityType);
            member_item.FavorityList = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetSelectedItems(member_item.ID, FavorityType);

            member_item = MemberShipDAO.GetItem(curUser.ID);
            ViewBag.Exit = true;
            return View($"Style{ model.StylesID }-MySocialInfo", member_item);
        }

        [HttpGet]
        public ActionResult Point(CardsModels model, int? index)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.MenuID = page.MenuID;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.PageID = page.PageNo;
            ViewBag.MenuID = page.MenuID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);

            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Login" }));
            }

            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };
            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);


            ViewModels.PointsRecordViewModel PointsRecord = new ViewModels.PointsRecordViewModel();

            int totalRecord;
            IEnumerable<PointsModel> points = PointsDAO.GetItems(page.SiteID, curUser.ID, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;

            PointsRecord.Points = points;
            PointsRecord.Total = PointsDAO.GetPointsTotal(page.SiteID, curUser.ID);

            item.PointsRecord = PointsRecord;

            ViewBag.Pagination = pagination;

            return View($"Style{ model.StylesID }-Point", item);
        }

        [HttpGet]
        public int CheckAccount(long siteId, string account) {
            return MemberShipDAO.CheckAccount(siteId, account, null) ? 1 : 0;
        }
        [HttpGet]
        public int CheckEmail(long siteId, string email)
        {
            return MemberShipDAO.CheckEmail(siteId, email, null) ? 1 : 0;
        }
        [HttpGet]
        public int CheckIDCard(long siteId, string IDCard)
        {
            return MemberShipDAO.CheckIDCard(siteId, IDCard, null) ? 1 : 0;
        }
        [HttpGet]
        public int CheckPushRecommandCode(string PushRecommandCode)
        {
            return MemberShipDAO.CheckPushRecommandCodeExist(PushRecommandCode) ? 1 : 0;
        }


        [HttpGet]
        public int Collection(long PageID)
        {
            string PageTitle = "";
            int exeCount =  MemberShipDAO.MemberCollection(PageID, PageTitle);
            return exeCount;
        }

        [HttpGet]
        public ActionResult Desktop(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.PageID = page.PageNo;
            ViewBag.MenuID = page.MenuID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);

            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Login" }));
            }

            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            List<ViewModels.MemberCollectionViewModel> collectionList= WorkV3.Models.DataAccess.MemberShipDAO.GetMemberCollections(curUser.ID);

            if (collectionList != null && collectionList.Count > 0)
            {
                foreach (ViewModels.MemberCollectionViewModel vmodel in collectionList)
                {
                    vmodel.LinkUrl = Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = vmodel.SN });
                    vmodel.ImgSrc = PagesDAO.GetContentImage(page.SiteID, vmodel.MenuID, vmodel.No);
                    vmodel.ImgAlt = vmodel.Title;
                    vmodel.Summary = PagesDAO.GetContentDesc(page.SiteID, page.MenuID, vmodel.No);
                }
            }
            item.CollectionList = collectionList;
            item.CollectionList = collectionList;

            return View($"Style{ model.StylesID }-Desktop", item);
        }

        [HttpGet]
        public ActionResult CollectionList(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.PageID = page.PageNo;
            ViewBag.MenuID = page.MenuID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);

            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Login" }));
            }

            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            List<ViewModels.MemberCollectionViewModel> collectionList = WorkV3.Models.DataAccess.MemberShipDAO.GetMemberCollections(curUser.ID);
            if (collectionList != null && collectionList.Count > 0)
            {
                foreach (ViewModels.MemberCollectionViewModel vmodel in collectionList)
                {
                    vmodel.LinkUrl = Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = vmodel.SN });
                    vmodel.ImgSrc = PagesDAO.GetContentImage(page.SiteID, vmodel.MenuID, vmodel.No);
                    vmodel.ImgAlt = vmodel.Title;
                    vmodel.Summary = PagesDAO.GetContentDesc(page.SiteID, page.MenuID, vmodel.No);
                }
            }
            item.CollectionList = collectionList;
            List<ViewModels.MemberCollectionMenuViewModel> collectionMenuList = WorkV3.Models.DataAccess.MemberShipDAO.GetMenuCollections(page.SiteID, curUser.ID);
            
            item.CollectionMenuList = collectionMenuList;

            return View($"Style{ model.StylesID }-CollectionList", item);
        }
        [HttpGet]
        public ActionResult LikeEventCalendar(CardsModels model)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.MenuID = page.MenuID;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            ViewBag.PageID = page.PageNo;
            ViewBag.MenuID = page.MenuID;
            ViewBag.DateTimeFmt = WebInfo.DateTimeNotS;
            ViewBag.UploadVPath = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID);

            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.Redirect(Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = "Login" }));
            }

            MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
            //List<ViewModels.MemberCollectionViewModel> collectionList = WorkV3.Models.DataAccess.MemberShipDAO.GetMemberCollections(curUser.ID);
            //if (collectionList != null && collectionList.Count > 0)
            //{
            //    foreach (ViewModels.MemberCollectionViewModel vmodel in collectionList)
            //    {
            //        vmodel.LinkUrl = Url.Action("Index", "Home", new { siteSn = page.SiteSN, pageSn = vmodel.SN });
            //        vmodel.ImgSrc = PagesDAO.GetContentImage(page.SiteID, vmodel.MenuID, vmodel.No);
            //        vmodel.ImgAlt = vmodel.Title;
            //        vmodel.Summary = PagesDAO.GetContentDesc(page.SiteID, page.MenuID, vmodel.No);
            //    }
            //}
            //item.CollectionList = collectionList;
            //List<ViewModels.MemberCollectionMenuViewModel> collectionMenuList = WorkV3.Models.DataAccess.MemberShipDAO.GetMenuCollections(page.SiteID, curUser.ID);

            //item.CollectionMenuList = collectionMenuList;

            return View($"Style{ model.StylesID }-LikeEventCalendar", item);
        }

        //[HttpGet]
        //public JsonResult MemberEvnets(long menuId, string YY, string MM)
        //{
        //    Member curUser = Member.Current;
        //    if (curUser == null)
        //    {
        //        Response.StatusCode = 401;
        //        return Json("");
        //    }
        //    MemberShipModels mem = MemberShipDAO.GetItem(curUser.ID);
        //    int eventYY = int.Parse(YY);
        //    int eventMM = int.Parse(MM);
        //    DateTime startDate = new DateTime(eventYY, eventMM, 1, 0, 0, 0);
        //    DateTime endDate = new DateTime(eventYY, eventMM, DateTime.DaysInMonth(eventYY, eventMM), 23, 59, 59);
        //    List<EventModels> onlineEventList = WorkV3.Models.DataAccess.MemberShipDAO.GetMemberOnlineEventList(curUser.ID);
        //    List<EventModels> offlineEventList = WorkV3.Models.DataAccess.MemberShipDAO.GetMemberOfflineEventList(curUser.ID);
        //    List<EventModels> canRegisterEventList = WorkV3.Models.DataAccess.MemberShipDAO.GetMemberOnlineNoApplyedEventList(curUser.ID);
        //    List<ViewModels.MemberEventViewModel> evnetModels = new List<ViewModels.MemberEventViewModel>();
        //    EventSettingModels setting = EventSettingDAO.GetItem(menuId);
        //    if (onlineEventList != null && onlineEventList.Count > 0)
        //    {
        //        var sEventList = onlineEventList.FindAll(f => f.DateStart >= startDate && f.DateStart <= endDate);
        //        if (sEventList != null && sEventList.Count > 0)
        //        {
        //            foreach (EventModels eModel in sEventList)
        //            {
        //                string cancelUrl = "", Location = "", Organizer = "";

        //                FormModel fModel = FormDAO.GetItemFromSourceID(eModel.ID);
        //                var applyRecords = FormItemDAO.GetItems(fModel.ID);
        //                var applyRecord = applyRecords.Where(p => p.Email == mem.Email);

        //                if (applyRecord != null && applyRecord.Count() > 0)
        //                {
        //                    cancelUrl = Url.FullActionUrl("Index", "Home", new { siteSn = PageCache.SiteSN, pageSn = eModel.PageSN, step = 5, formId = applyRecord.First().FormID, email = applyRecord.First().Email, token = (applyRecord.First().FormID.ToString() + applyRecord.First().Email).MD5() });
        //                }
        //                foreach (EventPlaceModels eventplace in eModel.GetPlaces())
        //                {
        //                    Location += eventplace.Address + "\n";
        //                }
        //                foreach (EventOrganizerModels eventorg in eModel.GetOrganizers())
        //                {
        //                    Organizer += eventorg.Name + "\n";
        //                }
        //                try
        //                {
        //                    ViewModels.MemberEventViewModel vModel = new ViewModels.MemberEventViewModel()
        //                    {
        //                        No = eModel.ID.ToString(),
        //                        title = eModel.Title,
        //                        start = eModel.DateStart.Value.ToString("yyyy-MM-dd"),
        //                        end = eModel.DateEnd.HasValue ? eModel.DateEnd.Value.ToString("yyyy-MM-dd") : eModel.DateStart.Value.ToString("yyyy-MM-dd"),
        //                        images =  eModel.Icon,
        //                        summary = eModel.Summary,
        //                        status = "已報名",
        //                        cancellink = cancelUrl,
        //                        location = Location,
        //                        organizer = Organizer
        //                    };

        //                    //if (!string.IsNullOrEmpty(vModel.images) && !vModel.images.Contains("/"))
        //                    //{
        //                    //    vModel.images = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(eModel.SiteID, eModel.MenuID) + vModel.images;
        //                    //}
        //                    evnetModels.Add(vModel);
        //                }
        //                catch { continue; }
        //            }
        //        }
        //    }
        //    if (offlineEventList != null && offlineEventList.Count > 0)
        //    {
        //        var sEventList = offlineEventList.FindAll(f => f.DateStart >= startDate && f.DateStart <= endDate);
        //        if (sEventList != null && sEventList.Count > 0)
        //        {
        //            foreach (EventModels eModel in sEventList)
        //            {
        //                string cancelUrl = "", Location = "", Organizer = "";
        //                FormModel fModel = FormDAO.GetItemFromSourceID(eModel.ID);
        //                var applyRecords = FormItemDAO.GetItems(fModel.ID);
        //                var applyRecord = applyRecords.Where(p => p.Email == mem.Email);

        //                if (applyRecord != null && applyRecord.Count() > 0)
        //                {
        //                    cancelUrl = Url.FullActionUrl("Index", "Home", new { siteSn = PageCache.SiteSN, pageSn = eModel.PageSN, step = 5, formId = applyRecord.First().FormID, email = applyRecord.First().Email, token = (applyRecord.First().FormID.ToString() + applyRecord.First().Email).MD5() });
        //                }
        //                foreach (EventPlaceModels eventplace in eModel.GetPlaces())
        //                {
        //                    Location += eventplace.Address + "\n";
        //                }
        //                foreach (EventOrganizerModels eventorg in eModel.GetOrganizers())
        //                {
        //                    Organizer += eventorg.Name + "\n";
        //                }
        //                ViewModels.MemberEventViewModel vModel = new ViewModels.MemberEventViewModel()
        //                {
        //                    No = eModel.ID.ToString(),
        //                    title = eModel.Title,
        //                    start = eModel.DateStart.Value.ToString("yyyy/MM/dd"),
        //                    end = eModel.DateEnd.HasValue ? eModel.DateEnd.Value.ToString("yyyy-MM-dd") : eModel.DateStart.Value.ToString("yyyy/MM/dd"),
        //                    images = eModel.Icon,
        //                    summary = eModel.Summary,
        //                    status = "已報名",
        //                    cancellink = cancelUrl,
        //                    location = Location,
        //                    organizer = Organizer
        //                };
        //                //if (!string.IsNullOrEmpty(vModel.images) && !vModel.images.Contains("/"))
        //                //{
        //                //    vModel.images = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(eModel.SiteID, eModel.MenuID)+vModel.images;
        //                //}
        //                evnetModels.Add(vModel);
        //            }
        //        }
        //    }
        //    if (canRegisterEventList != null && canRegisterEventList.Count > 0)
        //    {
        //        var sEventList = canRegisterEventList.FindAll(f => f.DateStart >= startDate && f.DateStart <= endDate);
        //        if (sEventList != null && sEventList.Count > 0)
        //        {
        //            foreach (EventModels eModel in sEventList)
        //            {
        //                string cancelUrl = "", Location = "", Organizer = "";
        //                FormModel fModel = FormDAO.GetItemFromSourceID(eModel.ID);
        //                var applyRecords = FormItemDAO.GetItems(fModel.ID);
        //                var applyRecord = applyRecords.Where(p => p.Email == mem.Email);

        //                if (applyRecord != null && applyRecord.Count() > 0)
        //                {
        //                    cancelUrl = Url.FullActionUrl("Index", "Home", new { siteSn = PageCache.SiteSN, pageSn = eModel.PageSN, step = 5, formId = applyRecord.First().FormID, email = applyRecord.First().Email, token = (applyRecord.First().FormID.ToString() + applyRecord.First().Email).MD5() });
        //                }
        //                foreach (EventPlaceModels eventplace in eModel.GetPlaces())
        //                {
        //                    Location += eventplace.Address + "\n";
        //                }
        //                foreach (EventOrganizerModels eventorg in eModel.GetOrganizers())
        //                {
        //                    Organizer += eventorg.Name + "\n";
        //                }
        //                ViewModels.MemberEventViewModel vModel = new ViewModels.MemberEventViewModel()
        //                {
        //                    No = eModel.ID.ToString(),
        //                    title = eModel.Title,
        //                    start = eModel.DateStart.Value.ToString("yyyy/MM/dd"),
        //                    end = eModel.DateEnd.HasValue ? eModel.DateEnd.Value.ToString("yyyy-MM-dd") : eModel.DateStart.Value.ToString("yyyy/MM/dd"),
        //                    images = eModel.Icon,
        //                    summary = eModel.Summary,
        //                    status = "未報名",
        //                    cancellink = cancelUrl,
        //                    location = Location,
        //                    organizer = Organizer
        //                };

        //                //if (!string.IsNullOrEmpty(vModel.images) && !vModel.images.Contains("/"))
        //                //{
        //                //    vModel.images = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(eModel.SiteID, eModel.MenuID) + vModel.images;
        //                //}
        //                evnetModels.Add(vModel);
        //            }
        //        }
        //    }
        //    return Json(evnetModels, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult AgreeDesc(CardsModels model)
        {
            WorkV3.Areas.Backend.Models.AgreeStatementSetModels item = WorkV3.Areas.Backend.Models.DataAccess.MemberShipSettingDAO.GetStatementItems();
            return View($"Style{ model.StylesID }-MemberAgreeDesc", item);
        }
        #region 頭像上傳
        //[HttpGet]
        //public ActionResult UploadPhoto(long SiteID, long MenuID)
        //{
        //    Member curUser = Member.Current;
        //    if (curUser == null)
        //    {
        //        Response.StatusCode = 401;
        //        return Json("");
        //    }
        //    MemberShipModels mem = MemberShipDAO.GetItem(curUser.ID);
        //    string photoImage = mem.Photo;
        //    if (string.IsNullOrEmpty(photoImage))
        //    {
        //        return "";
        //    }
        //    IEnumerable<ResourceImagesModels> ImgsModel = ResourceImagesDAO.GetItems(sourceNo, "Match");
        //    ResourceImagesModels ImgModel = null;
        //    if (ImgsModel != null && ImgsModel.Any())
        //    {
        //        ImgModel = ImgsModel.FirstOrDefault();
        //    }

        //    ViewBag.SiteID = SiteID;
        //    ViewBag.MenuID = MenuID;
        //    ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
        //    ViewBag.ImgModelJson = ImgModel != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ImgModel) : string.Empty;
        //    return View();
        //}
        [HttpPost]
        public string UploadPhoto(long siteId, long menuId)
        {
            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.StatusCode = 401;
                return "no member";
            }
            if (Request.Files == null || Request.Files.Count == 0)
                return "no file";

            HttpPostedFileBase postedFile = Request.Files[0];
            if (postedFile.ContentLength == 0)
                return null;

            MemberShipModels mem = MemberShipDAO.GetItem(curUser.ID);
           
            string fileName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId);
            mem.Photo = fileName;
            if (string.IsNullOrEmpty(fileName))
            {
                return " uplodate file fail ";
            }
            MemberShipDAO.SetItem(mem);
            return fileName;
        }
        [HttpPost]
        public void DeletePhoto(long siteId, long menuId)
        {
            Member curUser = Member.Current;
            if (curUser == null)
            {
                Response.StatusCode = 401;
                return;
            }
            MemberShipModels mem = MemberShipDAO.GetItem(curUser.ID);
            if(string.IsNullOrEmpty(mem.Photo))
                return;
            string uPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(siteId, menuId);
            if (System.IO.File.Exists(uPath + mem.Photo))
                System.IO.File.Delete(uPath + mem.Photo);
            mem.Photo = "";
            MemberShipDAO.SetItem(mem);
        }
        #endregion

        public ActionResult MobileVerify (CardsModels model, long MemberID)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            MemberShipModels item = MemberShipDAO.GetItem(MemberID);

            string SendStatus = SnedMobileVerify(MemberID, item.Mobile);

            return View($"Style1-MobileVerify", item);
        }

        [HttpPost]
        public ActionResult MobileVerify(CardsModels model, long SiteID, long MemberID, string captcha)
        {
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.PageSN = page.PageSN;
            ViewBag.SiteID = page.SiteID;
            MemberShipModels item = MemberShipDAO.GetItem(MemberID);

            VerifyResult verifyResult = MemberShipDAO.MobileVerify(item, captcha);

            bool IsExit = false;
            string vMessage = "";
            switch (verifyResult)
            {
                case VerifyResult.DataError:
                    ViewBag.Script = $"Component.showElementError('#captcha', '* {MobileVerifyErrorMessage}');";
                    break;
                case VerifyResult.VerifyExpired:
                    ViewBag.Script = $"Component.alert('{MobileVerifyExpiredMessage}');";
                    break;
                case VerifyResult.Success:
                    IsExit = true;
                    vMessage = ForgetVerifySuccessMessage;
                    break;
            }

            ViewBag.Message = vMessage;
            ViewBag.Exit = IsExit;

            return View($"Style1-MobileVerify", item);
        }

        [HttpPost]
        public ActionResult ReSnedMobileVerify(long MemberID, string Mobile)
        {
            string SendStatus = SnedMobileVerify(MemberID, Mobile);

            return Json(SendStatus);
        }

        public string SnedMobileVerify(long MemberID, string Mobile)
        {
            bool CaptchaJustNum = true;
            string captcha = Captcha.Create(6, CaptchaJustNum).Value;

            MemberShipDAO.sendMobileVerifyCode(MemberID, captcha);

            #region 發送手機簡訊
            string SendStatus = "";
            try
            {
                string ApiServer = "https://api.kotsms.com.tw/kotsmsapi-1.php";
                var srcEncoding = Encoding.UTF8;
                var dstEncoding = Encoding.GetEncoding(950); // big5
                var source = srcEncoding.GetBytes($"您的驗證碼為 {captcha} ! 請於60秒內輸入驗證碼。");
                var converted = Encoding.Convert(srcEncoding, dstEncoding, source);
                var encryptedContent = HttpUtility.UrlEncode(converted);
                string UriFormattedString = "{0}?username={1}&password={2}&dstaddr={3}&smbody={4}";
                string url = string.Format(UriFormattedString, ApiServer, "BR", "2012168", Mobile, encryptedContent);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var responseString = string.Empty;
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = "Get";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        responseString = new StreamReader(stream).ReadToEnd();
                        string replacement = Regex.Replace(responseString, @"\t|\n|\r|kmsgid=", "");
                        long responseCode = Int64.Parse(replacement);
                        if (responseCode < 0)
                        {
                            SendStatus = "fail";
                        }
                        else
                        {
                            SendStatus = "success";
                        }
                    }
                }
            }
            catch
            {
                SendStatus = "fail";
            }
            MemberShipDAO.MobileVerifyLog(MemberID, Mobile, captcha, SendStatus);
            #endregion

            return SendStatus;
        }

        [HttpPost]
        public ActionResult getVerifyTime(long MemberID)
        {
            MemberShipModels item = MemberShipDAO.GetItem(MemberID);
            return Json(item.VerifyTime);
        }
    }
}