using RadyaLabs.Controllers;
using RadyaLabs.Services;
using RadyaLabs.Validators;
using NSubstitute;
using System;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public class ValidatedControllerTests : ControllerTests
    {
        private ValidatedController<IValidator, IService> controller;
        private IValidator validator;
        private IService service;

        public ValidatedControllerTests()
        {
            service = Substitute.For<IService>();
            validator = Substitute.For<IValidator>();
            controller = Substitute.ForPartsOf<ValidatedController<IValidator, IService>>(validator, service);
        }

        #region ValidatedController(TService service, TValidator validator)

        [Fact]
        public void ValidatedController_SetsValidator()
        {
            Object actual = controller.Validator;
            Object expected = validator;

            Assert.Same(expected, actual);
        }

        #endregion

        #region OnActionExecuting(ActionExecutingContext context)

        [Fact]
        public void OnActionExecuting_SetsServiceCurrentAccountId()
        {
            ReturnCurrentAccountId(controller, 1);

            ((IActionFilter)controller).OnActionExecuting(null);

            Int32 expected = controller.CurrentAccountId;
            Int32 actual = service.CurrentAccountId;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsValidatorCurrentAccountId()
        {
            ReturnCurrentAccountId(controller, 1);

            ((IActionFilter)controller).OnActionExecuting(null);

            Int32 expected = controller.CurrentAccountId;
            Int32 actual = validator.CurrentAccountId;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsValidatorAlerts()
        {
            ((IActionFilter)controller).OnActionExecuting(null);

            Object expected = controller.Alerts;
            Object actual = validator.Alerts;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsModelState()
        {
            ((IActionFilter)controller).OnActionExecuting(null);

            Object expected = controller.ModelState;
            Object actual = validator.ModelState;

            Assert.Same(expected, actual);
        }

        #endregion

        #region Dispose()

        [Fact]
        public void Dispose_Service()
        {
            controller.Dispose();

            service.Received().Dispose();
        }

        [Fact]
        public void Dispose_Validator()
        {
            controller.Dispose();

            validator.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            controller.Dispose();
            controller.Dispose();
        }

        #endregion
    }
}
