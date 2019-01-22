using RadyaLabs.Controllers;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public abstract class ControllerTests
    {
        protected void ReturnCurrentAccountId(BaseController controller, Int32 id)
        {
            controller.When(sub => { Int32 get = sub.CurrentAccountId; }).DoNotCallBase();
            controller.CurrentAccountId.Returns(id);
        }

        protected void ProtectsFromOverposting(Controller controller, String postMethod, String properties)
        {
            MethodInfo methodInfo = controller
                .GetType()
                .GetMethods()
                .First(method =>
                    method.Name == postMethod &&
                    method.IsDefined(typeof(HttpPostAttribute), false));

            Assert.True(methodInfo.GetParameters()[0].IsDefined(typeof(BindAttribute), false));
        }

        protected RedirectToRouteResult NotEmptyView(BaseController controller, Object model)
        {
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary());
            controller.When(sub => sub.NotEmptyView(model)).DoNotCallBase();
            controller.NotEmptyView(model).Returns(result);

            return result;
        }

        protected RedirectToRouteResult RedirectToDefault(BaseController controller)
        {
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary());
            controller.When(sub => sub.RedirectToDefault()).DoNotCallBase();
            controller.RedirectToDefault().Returns(result);

            return result;
        }
        protected RedirectToRouteResult RedirectToNotFound(BaseController controller)
        {
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary());
            controller.When(sub => sub.RedirectToNotFound()).DoNotCallBase();
            controller.RedirectToNotFound().Returns(result);

            return result;
        }
        protected RedirectToRouteResult RedirectIfAuthorized(BaseController controller, String action)
        {
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary());
            controller.When(sub => sub.RedirectIfAuthorized(action)).DoNotCallBase();
            controller.RedirectIfAuthorized(action).Returns(result);

            return result;
        }
        protected RedirectToRouteResult RedirectIfAuthorized(BaseController baseController, String action, String controller)
        {
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary());
            baseController.When(sub => sub.RedirectIfAuthorized(action, controller)).DoNotCallBase();
            baseController.RedirectIfAuthorized(action, controller).Returns(result);

            return result;
        }
    }
}
