using RadyaLabs.Components.Security;
using System;
using System.Web.Mvc;

namespace RadyaLabs.Components.Extensions
{
    public static class AuthorizationExtensions
    {
        public static Boolean IsAuthorizedFor(this HtmlHelper html, String action)
        {
            String controller = html.ViewContext.RouteData.Values["controller"] as String;

            return html.IsAuthorizedFor(action, controller);
        }
        public static Boolean IsAuthorizedFor(this HtmlHelper html, String action, String controller)
        {
            String area = html.ViewContext.RouteData.Values["area"] as String;

            return html.IsAuthorizedFor(action, controller, area);
        }
        public static Boolean IsAuthorizedFor(this HtmlHelper html, String action, String controller, String area)
        {
            Int32? accountId = html.ViewContext.HttpContext.User.Id();

            return Authorization.Provider?.IsAuthorizedFor(accountId, area, controller, action) != false;
        }
    }
}
