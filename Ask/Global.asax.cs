using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WorkV3.App_Start;

namespace WorkV3
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static bool IsExeTimer = false;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Models.DataAccess.SitesDAO.CreateSiteManagers();
            AutofacConfig.Bootstrapper();
        }
        public void ExeTimer()
        {
            //20180613 自動產製sitemap charlie_shan
            Models.DataAccess.SitemapRecreateSchedule sitemap = new Models.DataAccess.SitemapRecreateSchedule();
            sitemap.StartTimer();

            //20180914 自動截取各網設截圖 charlie_shan
            Models.DataAccess.SiteCoverRecreateSchedule siteCover = new Models.DataAccess.SiteCoverRecreateSchedule(Request.ApplicationPath.Trim('/'));
            //siteCover.StartTimer();
        }
        public void Application_BeginRequest(object sender, EventArgs e)
        {
            string logFileName = System.Web.Hosting.HostingEnvironment.MapPath("~/SitemapCreateLog.txt");
            if (!IsExeTimer)
            {
                ExeTimer();
                IsExeTimer = true;
                System.IO.File.AppendAllText(logFileName, DateTime.Now.ToString() + " IsExeTimer " + IsExeTimer.ToString() + System.Environment.NewLine);
            }
            if (HttpContext.Current != null)
            {
                if (WorkV3.Models.DataAccess.SitesDAO.SiteUrl == null)
                {
                    string host = HttpContext.Current.Request.Url.Host;
                    string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                    string[] exc = new string[] { "webdemo.kidtech.com.tw" }; //測試用
                    //因為測試站網址有加上目錄 (ex: http://127.0.0.1/目錄) 和 正式站不同 (http://127.0.0.1)
                    //這會造成電子報發送在進行Request取得電子報內容時會失敗
                    if (exc.Any(m => m == host))
                        root += "/" + HttpContext.Current.Request.Url.Segments[1].Replace("/", "");

                    WorkV3.Models.DataAccess.SitesDAO.SiteUrl = root;
                    System.IO.File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/MailSendLog.txt"), "EpaperRenderUrlRoot：" + WorkV3.Models.DataAccess.SitesDAO.SiteUrl + " Log At " + DateTime.Now + Environment.NewLine);
                }
            }
        }
        public void Application_EndRequest(object sender, EventArgs e) {
            WorkV3.BundleConfig.IsLoadJWPlayer = false;
            WorkV3.BundleConfig.IsLoadColorbox = false;
            WorkV3.BundleConfig.IsLoadSwiper = false;
        }
        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
    }
}
