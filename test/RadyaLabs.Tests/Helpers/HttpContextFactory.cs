using NSubstitute;
using System.Collections;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RadyaLabs.Tests
{
    public static class HttpContextFactory
    {
        public static HttpContext CreateHttpContext()
        {
            HttpRequest request = new HttpRequest("", "http://localhost:19175/domain/", "p=1&n&=k");
            HttpBrowserCapabilities browser = new HttpBrowserCapabilities();
            browser.Capabilities = new Hashtable { { "cookies", "true" } };
            HttpResponse response = new HttpResponse(new StringWriter());
            HttpContext context = new HttpContext(request, response);
            request.Browser = browser;

            RouteValueDictionary route = request.RequestContext.RouteData.Values;
            route["area"] = "administration";
            route["controller"] = "accounts";
            route["action"] = "details";
            route["language"] = "en";
            MapRoutes();

            IIdentity identity = Substitute.For<IIdentity>();
            identity.IsAuthenticated.Returns(true);
            identity.Name.Returns("1");

            context.User = Substitute.For<IPrincipal>();
            context.User.Identity.Returns(identity);

            return context;
        }
        public static HttpContextBase CreateHttpContextBase()
        {
            HttpContext context = CreateHttpContext();

            HttpRequestBase request = Substitute.ForPartsOf<HttpRequestWrapper>(context.Request);
            HttpContextBase contextBase = Substitute.ForPartsOf<HttpContextWrapper>(context);
            contextBase.Response.Returns(new HttpResponseWrapper(context.Response));
            contextBase.Server.Returns(Substitute.For<HttpServerUtilityBase>());
            request.ApplicationPath.Returns("/domain");
            contextBase.Request.Returns(request);

            return contextBase;
        }

        private static void MapRoutes()
        {
            RouteTable.Routes.Clear();

            RouteValueDictionary tokens = RouteTable.Routes
                .MapRoute(
                    "AdministrationMultilingual",
                    "{language}/Administration/{controller}/{action}/{id}",
                    new { area = "Administration", action = "Index", id = UrlParameter.Optional },
                    new { language = "lt", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers.Administration" })
                .DataTokens;

            tokens["UseNamespaceFallback"] = false;
            tokens["area"] = "Administration";

            tokens = RouteTable.Routes
                .MapRoute(
                    "Administration",
                    "Administration/{controller}/{action}/{id}",
                    new { language = "en", area = "Administration", action = "Index", id = UrlParameter.Optional },
                    new { language = "en", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers.Administration" })
                .DataTokens;

            tokens["UseNamespaceFallback"] = false;
            tokens["area"] = "Administration";

            RouteTable.Routes
                .MapRoute(
                    "DefaultMultilingual",
                    "{language}/{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "lt", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers" })
                .DataTokens["UseNamespaceFallback"] = false;

            RouteTable.Routes
                .MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { language = "en", controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "en", id = "[0-9]*" },
                    new[] { "RadyaLabs.Controllers" })
                .DataTokens["UseNamespaceFallback"] = false;

            RouteTable.Routes.LowercaseUrls = true;
        }
    }
}
