using RadyaLabs.Controllers;
using RadyaLabs.Services;
using NSubstitute;
using System;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public class HomeControllerTests : ControllerTests
    {
        private HomeController controller;
        private IAccountService service;

        public HomeControllerTests()
        {
            service = Substitute.For<IAccountService>();
            controller = Substitute.ForPartsOf<HomeController>(service);

            ReturnCurrentAccountId(controller, 1);
        }

        #region Index()

        [Fact]
        public void Index_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectIfAuthorized(controller, "Logout", "Auth");
            Object actual = controller.Index();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Index_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            ViewResult actual = controller.Index() as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion

        #region Error()

        [Fact]
        public void Error_ReturnsEmptyView()
        {
            ViewResult actual = controller.Error();

            Assert.Null(actual.Model);
        }

        #endregion

        #region NotFound()

        [Fact]
        public void NotFound_ReturnsEmptyView()
        {
            ViewResult actual = controller.NotFound();

            Assert.Null(actual.Model);
        }

        #endregion

        #region Unauthorized()

        [Fact]
        public void Unauthorized_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectIfAuthorized(controller, "Logout", "Auth");
            Object actual = controller.Unauthorized();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Unauthorized_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            ViewResult actual = controller.Unauthorized() as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion
    }
}
