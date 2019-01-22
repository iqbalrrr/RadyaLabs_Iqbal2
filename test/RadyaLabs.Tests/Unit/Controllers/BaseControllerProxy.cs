using RadyaLabs.Controllers;
using System;
using System.Web.Mvc;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public class BaseControllerProxy : BaseController
    {
        public RedirectToRouteResult BaseRedirectToAction(String action)
        {
            return RedirectToAction(action);
        }
        public RedirectToRouteResult BaseRedirectToAction(String action, Object route)
        {
            return RedirectToAction(action, route);
        }
        public RedirectToRouteResult BaseRedirectToAction(String action, String controller)
        {
            return RedirectToAction(action, controller);
        }
        public RedirectToRouteResult BaseRedirectToAction(String action, String controller, Object route)
        {
            return RedirectToAction(action, controller, route);
        }
    }
}
