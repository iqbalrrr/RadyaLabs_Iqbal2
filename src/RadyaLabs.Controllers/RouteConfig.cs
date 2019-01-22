using System.Web.Mvc;
using System.Web.Routing;

namespace RadyaLabs.Controllers
{
    public class RouteConfig : IRouteConfig
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes
                .MapRoute(
                    "DefaultMultilingual",
                    "{language}/{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "lt", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers" })
                .DataTokens["UseNamespaceFallback"] = false;

            routes
                .MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { language = "en", controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "en", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers" })
                .DataTokens["UseNamespaceFallback"] = false;
        }
    }
}
