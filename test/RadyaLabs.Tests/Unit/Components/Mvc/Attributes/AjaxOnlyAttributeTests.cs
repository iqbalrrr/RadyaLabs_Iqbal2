using RadyaLabs.Components.Mvc;
using NSubstitute;
using System;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class AjaxOnlyAttributeTests
    {
        #region IsValidForRequest(ControllerContext context, MethodInfo method)

        [Theory]
        [InlineData("", false)]
        [InlineData("XMLHttpRequest", true)]
        public void IsValidForRequest_Ajax(String header, Boolean isValid)
        {
            ControllerContext context = new ControllerContext();
            context.HttpContext = HttpContextFactory.CreateHttpContextBase();
            context.HttpContext.Request["X-Requested-With"].Returns(header);

            Boolean actual = new AjaxOnlyAttribute().IsValidForRequest(context, null);
            Boolean expected = isValid;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
