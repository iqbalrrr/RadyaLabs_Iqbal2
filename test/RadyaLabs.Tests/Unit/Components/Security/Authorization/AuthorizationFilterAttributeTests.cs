using RadyaLabs.Components.Security;
using NSubstitute;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Security
{
    public class AuthorizationFilterAttributeTests
    {
        #region OnAuthorization(AuthorizationContext filterContext)

        [Fact]
        public void OnAuthorization_NotAuthenticated_RedirectsToLogin()
        {
            AuthorizationContext action = new AuthorizationContext();
            action.ActionDescriptor = Substitute.For<ActionDescriptor>();
            HttpContextBase context = HttpContextFactory.CreateHttpContextBase();
            AuthorizationFilterAttribute attribute = new AuthorizationFilterAttribute { Users = "None" };

            action.RouteData = context.Request.RequestContext.RouteData;
            action.RouteData.Values["test"] = "Test";
            action.HttpContext = context;

            attribute.OnAuthorization(action);

            RouteValueDictionary actual = (action.Result as RedirectToRouteResult).RouteValues;

            Assert.Equal(action.RouteData.Values["language"], actual["language"]);
            Assert.Equal(action.HttpContext.Request.RawUrl, actual["returnUrl"]);
            Assert.Equal("Auth", actual["controller"]);
            Assert.Equal("Login", actual["action"]);
            Assert.Equal("", actual["area"]);
            Assert.Equal(5, actual.Count);
        }

        #endregion
    }
}
