using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WorkV3.ViewModels.API;
using Jose;

namespace WorkV3.Filters
{
    public class JwtAuthActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string APISecret = WorkLib.GetItem.appSet("APISecret").ToString();
            if (actionContext.Request.Headers.Authorization == null || actionContext.Request.Headers.Authorization.Scheme != "Bearer")
            {
                setErrorResponse(actionContext, "驗證錯誤");
            }
            else
            {
                try
                {
                    var jwtObject = Jose.JWT.Decode<ApiToken>(
                        actionContext.Request.Headers.Authorization.Parameter,
                        System.Text.Encoding.UTF8.GetBytes(APISecret),
                        JwsAlgorithm.HS256);
                    if (Convert.ToDateTime(jwtObject.expire) < DateTime.UtcNow.AddHours(08))
                        actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Token已經過期。");
                }
                catch (Exception ex)
                {
                    setErrorResponse(actionContext, ex.Message);
                }
            }

            base.OnActionExecuting(actionContext);
        }

        private static void setErrorResponse(HttpActionContext actionContext, string message)
        {
            var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
            actionContext.Response = response;
        }
    }
}