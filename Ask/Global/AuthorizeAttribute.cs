using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Global
{
    public class MemberShipAuthorizeAttribute: AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Common.Member.Current != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            context.Response.Redirect($"~/w/Channel/Login");
        }
    }
    public class MemberShipAjaxAuthorizeAttribute: AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return WorkV3.Common.Member.Current != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            context.Response.Redirect($"~/Cart/ReturnFalse");
        }
    }
}