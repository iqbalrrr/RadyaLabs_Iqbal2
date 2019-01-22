using RadyaLabs.Components.Extensions;
using RadyaLabs.Components.Security;
using NSubstitute;
using System;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Extensions
{
    public class AuthorizationExtensionsTests : IDisposable
    {
        private HtmlHelper html;

        public AuthorizationExtensionsTests()
        {
            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            html = HtmlHelperFactory.CreateHtmlHelper();
        }
        public void Dispose()
        {
            Authorization.Provider = null;
        }

        #region IsAuthorizedFor(this HtmlHelper html, String action)

        [Fact]
        public void IsAuthorizedFor_Action_NullProvider_ReturnsTrue()
        {
            Authorization.Provider = null;

            Assert.True(html.IsAuthorizedFor("Action"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsAuthorizedFor_Action_ReturnsAuthorizationResult(Boolean isAuthorized)
        {
            Int32? accountId = html.ViewContext.HttpContext.User.Id();
            String area = html.ViewContext.RouteData.Values["area"] as String;
            String controller = html.ViewContext.RouteData.Values["controller"] as String;

            Authorization.Provider.IsAuthorizedFor(accountId, area, controller, "Action").Returns(isAuthorized);

            Boolean actual = html.IsAuthorizedFor("Action");
            Boolean expected = isAuthorized;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region IsAuthorizedFor(this HtmlHelper html, String action, String controller)

        [Fact]
        public void IsAuthorizedFor_Controller_NullProvider_ReturnsTrue()
        {
            Authorization.Provider = null;

            Assert.True(html.IsAuthorizedFor("Action", "Controller"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsAuthorizedFor_Controller_ReturnsProviderResult(Boolean isAuthorized)
        {
            Int32? accountId = html.ViewContext.HttpContext.User.Id();
            String area = html.ViewContext.RouteData.Values["area"] as String;

            Authorization.Provider.IsAuthorizedFor(accountId, area, "Controller", "Action").Returns(isAuthorized);

            Boolean actual = html.IsAuthorizedFor("Action", "Controller");
            Boolean expected = isAuthorized;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region IsAuthorizedFor(this HtmlHelper html, String action, String controller, String area)

        [Fact]
        public void IsAuthorizedFor_Area_NullProvider_ReturnsTrue()
        {
            Authorization.Provider = null;

            Assert.True(html.IsAuthorizedFor("Action", "Controller", "Area"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsAuthorizedFor_Area_ReturnsProviderResult(Boolean isAuthorized)
        {
            Int32? accountId = html.ViewContext.HttpContext.User.Id();

            Authorization.Provider.IsAuthorizedFor(accountId, "Area", "Controller", "Action").Returns(isAuthorized);

            Boolean actual = html.IsAuthorizedFor("Action", "Controller", "Area");
            Boolean expected = isAuthorized;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
