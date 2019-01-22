using RadyaLabs.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public class RouteConfigTests
    {
        private RouteConfig config;

        public RouteConfigTests()
        {
            config = new RouteConfig();
        }

        #region RegisterRoutes(RouteCollection routes)

        [Fact]
        public void RegisterRoutes_ForLowercaseUrls()
        {
            RouteCollection routes = new RouteCollection();

            config.RegisterRoutes(routes);

            Assert.True(routes.LowercaseUrls);
        }

        [Fact]
        public void RegisterRoutes_IgnoresAxd()
        {
            RouteCollection routes = new RouteCollection();
            config.RegisterRoutes(routes);

            Route expected = new Route("{resource}.axd/{*pathInfo}", new StopRoutingHandler());
            Route actual = routes.First() as Route;

            Assert.Equal(expected.RouteHandler.GetType(), actual.RouteHandler.GetType());
            Assert.Equal(expected.Url, actual.Url);
        }

        [Fact]
        public void RegisterRoutes_DefaultMultilingual()
        {
            RouteCollection routes = new RouteCollection();
            config.RegisterRoutes(routes);

            Route actual = routes["DefaultMultilingual"] as Route;

            Assert.Equal(new[] { "RadyaLabs.Controllers" }, actual.DataTokens["Namespaces"] as String[]);
            Assert.Equal("{language}/{controller}/{action}/{id}", actual.Url);
            Assert.False((Boolean?)actual.DataTokens["UseNamespaceFallback"]);
            Assert.Equal(UrlParameter.Optional, actual.Defaults["id"]);
            Assert.Equal("Home", actual.Defaults["controller"]);
            Assert.Equal("lt", actual.Constraints["language"]);
            Assert.Equal("[0-9]*", actual.Constraints["id"]);
            Assert.Equal("Index", actual.Defaults["action"]);
            Assert.Equal(2, actual.Constraints.Count);
            Assert.Equal(2, actual.DataTokens.Count);
            Assert.Equal(3, actual.Defaults.Count);
        }

        [Fact]
        public void RegisterRoutes_Default()
        {
            RouteCollection routes = new RouteCollection();
            config.RegisterRoutes(routes);

            Route actual = routes["Default"] as Route;

            Assert.Equal(new[] { "RadyaLabs.Controllers" }, actual.DataTokens["Namespaces"] as String[]);
            Assert.False((Boolean?)actual.DataTokens["UseNamespaceFallback"]);
            Assert.Equal(UrlParameter.Optional, actual.Defaults["id"]);
            Assert.Equal("{controller}/{action}/{id}", actual.Url);
            Assert.Equal("Home", actual.Defaults["controller"]);
            Assert.Equal("en", actual.Constraints["language"]);
            Assert.Equal("[0-9]*", actual.Constraints["id"]);
            Assert.Equal("Index", actual.Defaults["action"]);
            Assert.Equal("en", actual.Defaults["language"]);
            Assert.Equal(2, actual.Constraints.Count);
            Assert.Equal(2, actual.DataTokens.Count);
            Assert.Equal(4, actual.Defaults.Count);
        }

        #endregion
    }
}
