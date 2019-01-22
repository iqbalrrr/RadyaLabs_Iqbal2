using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace RadyaLabs.Components.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            RouteValueDictionary route = new RouteValueDictionary();
            route["language"] = context.RouteData.Values["language"];
            route["returnUrl"] = context.HttpContext.Request.RawUrl;
            route["controller"] = "Auth";
            route["action"] = "Login";
            route["area"] = "";

            context.Result = new RedirectToRouteResult(route);
        }
    }
}
