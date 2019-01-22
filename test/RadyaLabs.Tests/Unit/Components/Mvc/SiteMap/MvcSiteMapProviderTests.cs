using RadyaLabs.Components.Extensions;
using RadyaLabs.Components.Mvc;
using RadyaLabs.Components.Security;
using NSubstitute;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class MvcSiteMapProviderTests : IDisposable
    {
        private RouteValueDictionary route;
        private MvcSiteMapProvider siteMap;
        private ViewContext context;

        static MvcSiteMapProviderTests()
        {
            CreateSiteMap("Mvc.sitemap");
        }
        public MvcSiteMapProviderTests()
        {
            siteMap = new MvcSiteMapProvider("Mvc.sitemap", new MvcSiteMapParser());
            context = HtmlHelperFactory.CreateHtmlHelper().ViewContext;
            route = context.RouteData.Values;
        }
        public void Dispose()
        {
            Authorization.Provider = null;
        }

        #region GetSiteMap(ViewContext context)

        [Fact]
        public void GetSiteMap_NullAuthorization_ReturnsAllNodes()
        {
            Authorization.Provider = null;

            MvcSiteMapNode[] actual = siteMap.GetSiteMap(context).ToArray();

            Assert.Single(actual);

            Assert.Null(actual[0].Action);
            Assert.Null(actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-cogs", actual[0].IconClass);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);

            Assert.Empty(actual[0].Children);

            Assert.Equal("Index", actual[0].Action);
            Assert.Equal("Accounts", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-user", actual[0].IconClass);

            Assert.Null(actual[1].Action);
            Assert.Equal("Roles", actual[1].Controller);
            Assert.Equal("Administration", actual[1].Area);
            Assert.Equal("fa fa-users", actual[1].IconClass);

            actual = actual[1].Children.ToArray();

            Assert.Single(actual);
            Assert.Empty(actual[0].Children);

            Assert.Equal("Create", actual[0].Action);
            Assert.Equal("Roles", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("far fa-file", actual[0].IconClass);
        }

        [Fact]
        public void GetSiteMap_ReturnsAuthorizedNodes()
        {
            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            Authorization.Provider.IsAuthorizedFor(context.HttpContext.User.Id(), "Administration", "Accounts", "Index").Returns(true);

            MvcSiteMapNode[] actual = siteMap.GetSiteMap(context).ToArray();

            Assert.Single(actual);

            Assert.Null(actual[0].Action);
            Assert.Null(actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-cogs", actual[0].IconClass);

            actual = actual[0].Children.ToArray();

            Assert.Single(actual);

            Assert.Empty(actual[0].Children);

            Assert.Equal("Index", actual[0].Action);
            Assert.Equal("Accounts", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-user", actual[0].IconClass);
        }

        [Fact]
        public void GetSiteMap_SetsActiveMenu()
        {
            Authorization.Provider = null;
            route["action"] = "Create";
            route["controller"] = "Roles";
            route["area"] = "Administration";

            MvcSiteMapNode[] actual = siteMap.GetSiteMap(context).ToArray();

            Assert.Single(actual);
            Assert.False(actual[0].IsActive);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);
            Assert.False(actual[0].IsActive);
            Assert.False(actual[1].IsActive);
            Assert.Empty(actual[0].Children);

            actual = actual[1].Children.ToArray();

            Assert.Empty(actual[0].Children);
            Assert.True(actual[0].IsActive);
            Assert.Single(actual);
        }

        [Fact]
        public void GetSiteMap_NonMenuChildrenNodeIsActive_SetsActiveMenu()
        {
            Authorization.Provider = null;
            route["action"] = "Edit";
            route["controller"] = "Accounts";
            route["area"] = "Administration";

            MvcSiteMapNode[] actual = siteMap.GetSiteMap(context).ToArray();

            Assert.Single(actual);
            Assert.False(actual[0].IsActive);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);
            Assert.True(actual[0].IsActive);
            Assert.False(actual[1].IsActive);
            Assert.Empty(actual[0].Children);

            actual = actual[1].Children.ToArray();

            Assert.Single(actual);
            Assert.False(actual[0].IsActive);
            Assert.Empty(actual[0].Children);
        }

        [Fact]
        public void GetSiteMap_ActiveMenuParents_SetsHasActiveChildren()
        {
            Authorization.Provider = null;
            route["action"] = "Create";
            route["controller"] = "Roles";
            route["area"] = "Administration";

            MvcSiteMapNode[] actual = siteMap.GetSiteMap(context).ToArray();

            Assert.Single(actual);
            Assert.True(actual[0].HasActiveChildren);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);
            Assert.Empty(actual[0].Children);
            Assert.True(actual[1].HasActiveChildren);
            Assert.False(actual[0].HasActiveChildren);

            actual = actual[1].Children.ToArray();

            Assert.Single(actual);
            Assert.Empty(actual[0].Children);
            Assert.False(actual[0].HasActiveChildren);
        }

        [Fact]
        public void GetSiteMap_RemovesEmptyNodes()
        {
            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            Authorization.Provider.IsAuthorizedFor(Arg.Any<Int32?>(), Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>()).Returns(true);
            Authorization.Provider.IsAuthorizedFor(context.HttpContext.User.Id(), "Administration", "Roles", "Create").Returns(false);

            MvcSiteMapNode[] actual = siteMap.GetSiteMap(context).ToArray();

            Assert.Single(actual);

            Assert.Null(actual[0].Action);
            Assert.Null(actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-cogs", actual[0].IconClass);

            actual = actual[0].Children.ToArray();

            Assert.Single(actual);

            Assert.Empty(actual[0].Children);

            Assert.Equal("Index", actual[0].Action);
            Assert.Equal("Accounts", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-user", actual[0].IconClass);
        }

        #endregion

        #region GetBreadcrumb(ViewContext context)

        [Fact]
        public void GetBreadcrumb_IsCaseInsensitive()
        {
            route["controller"] = "profile";
            route["action"] = "edit";
            route["area"] = null;

            MvcSiteMapNode[] actual = siteMap.GetBreadcrumb(context).ToArray();

            Assert.Equal(3, actual.Length);

            Assert.Equal("fa fa-home", actual[0].IconClass);
            Assert.Equal("Home", actual[0].Controller);
            Assert.Equal("Index", actual[0].Action);
            Assert.Null(actual[0].Area);

            Assert.Equal("fa fa-user", actual[1].IconClass);
            Assert.Equal("Profile", actual[1].Controller);
            Assert.Null(actual[1].Action);
            Assert.Null(actual[1].Area);

            Assert.Equal("fa fa-pencil-alt", actual[2].IconClass);
            Assert.Equal("Profile", actual[2].Controller);
            Assert.Equal("Edit", actual[2].Action);
            Assert.Null(actual[2].Area);
        }

        [Fact]
        public void GetBreadcrumb_NoAction_ReturnsEmpty()
        {
            route["controller"] = "profile";
            route["action"] = "edit";
            route["area"] = "area";

            Assert.Empty(siteMap.GetBreadcrumb(context));
        }

        #endregion

        #region Test helpers

        private static void CreateSiteMap(String path)
        {
            XElement
                .Parse(
                    @"<siteMap>
                        <siteMapNode icon=""fa fa-home"" controller=""Home"" action=""Index"">
                            <siteMapNode icon=""fa fa-user"" controller=""Profile"">
                                <siteMapNode icon=""fa fa-pencil-alt"" controller=""Profile"" action=""Edit"" />
                            </siteMapNode>
                            <siteMapNode menu=""true"" icon=""fa fa-cogs"" area=""Administration"">
                                <siteMapNode menu=""true"" icon=""fa fa-user"" area=""Administration"" controller=""Accounts"" action=""Index"">
                                    <siteMapNode icon=""fa fa-info"" area=""Administration"" controller=""Accounts"" action=""Details"">
                                        <siteMapNode icon=""fa fa-pencil-alt"" area=""Administration"" controller=""Accounts"" action=""Edit"" />
                                    </siteMapNode>
                                </siteMapNode>
                                <siteMapNode menu=""true"" icon=""fa fa-users"" area=""Administration"" controller=""Roles"">
                                    <siteMapNode menu=""true"" icon=""far fa-file"" area=""Administration"" controller=""Roles"" action=""Create"" />
                                    <siteMapNode icon=""fa fa-pencil-alt"" area=""Administration"" controller=""Roles"" action=""Edit"" />
                                </siteMapNode>
                            </siteMapNode>
                        </siteMapNode>
                    </siteMap>")
                .Save(path);
        }

        #endregion
    }
}
