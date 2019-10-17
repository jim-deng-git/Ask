using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WorkV3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region  前台

            routes.MapRoute(
              name: "FrontSitePage",
              url: "w/{SiteSN}/{PageSN}",
              defaults: new { controller = "Home", action = "Index", PageSN = UrlParameter.Optional },
              namespaces: new string[] { "WorkV3.Controllers" }
            );

            routes.MapRoute(
              name: "FrontSitePageCont",
              url: "w/{SiteSN}/{PageSN}/{id}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
              namespaces: new string[] { "WorkV3.Controllers" }
            );

            routes.MapRoute(
              name: "FrontSiteSinglePageCont",
              url: "sp/{SiteSN}/{PageSN}/{type}",
              defaults: new { controller = "SinglePage", action = "Index" },
              namespaces: new string[] { "WorkV3.Controllers" }
            );

            routes.MapRoute(
             name: "RewardCollect",
             url: "spc/{SiteSN}/{PageSN}/",
             defaults: new { controller = "Home", action = "IndexWithoutLayout", PageSN = UrlParameter.Optional,lay= "~/Views/Shared/_MasterLayout.cshtml" },
             namespaces: new string[] { "WorkV3.Controllers" }

           );

            routes.MapRoute(
             name: "RewardCollectCont",
             url: "spc/{SiteSN}/{PageSN}/{id}",
             defaults: new { controller = "Home", action = "IndexWithoutLayout", id = UrlParameter.Optional, lay = "~/Views/Shared/_MasterLayout.cshtml" },
             namespaces: new string[] { "WorkV3.Controllers" }
              );

            routes.MapRoute(
             name: "EpaperContent",
             url: "edm/{SiteSN}/{PageSN}/{EpaperID}",
             defaults: new { controller = "Epaper", action = "EpaperContent", SiteSN = UrlParameter.Optional, PageSN = UrlParameter.Optional, EpaperID = UrlParameter.Optional },
             namespaces: new string[] { "WorkV3.Controllers" }

           );

            routes.MapRoute(
             name: "EpaperCont",
             url: "edm/{SiteSN}/{PageSN}/{id}",
             defaults: new { controller = "Home", action = "IndexWithoutLayout", id = UrlParameter.Optional, lay = "" },
             namespaces: new string[] { "WorkV3.Controllers" }
              );
            #endregion
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "WorkV3.Controllers" }
            );


            //後台by站台功能
            //routes.MapRoute(
            //  name: "Backend",
            //  url: "Backend/{SiteSN}/{action}/{id}",
            //  defaults: new { controller = "Backend", action = "Login", id = UrlParameter.Optional }
            //);

            //前台

            //routes.MapRoute(
            //    name: "MenuSample",
            //    url: "{controller}/{action}/{id}/{MenuTest}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, MenuTest = UrlParameter.Optional }
            //);
        }
    }
}
