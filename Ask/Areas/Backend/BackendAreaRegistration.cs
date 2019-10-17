using System.Web.Mvc;

namespace WorkV3.Areas.Backend
{
    public class BackendAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Backend";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               "Backend_Login",
               "Backend/login",
               new { controller = "Home", action = "Login" }
            );

            context.MapRoute(
                "Backend_Site",
                "Backend/w/{SiteSN}/{action}/{id}",
                new { controller = "Home", action = "Login", id = UrlParameter.Optional }
             );

            context.MapRoute(
               "Backend_default",
               "Backend/{controller}/{action}/{id}",
               new { controller = "Home", action = "Index", id = UrlParameter.Optional }           
            );

        }
    }
}