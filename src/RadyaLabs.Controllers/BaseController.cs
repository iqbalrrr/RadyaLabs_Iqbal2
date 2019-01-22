using RadyaLabs.Components.Alerts;
using RadyaLabs.Components.Extensions;
using RadyaLabs.Components.Mvc;
using RadyaLabs.Components.Security;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace RadyaLabs.Controllers
{
    [AuthorizationFilter]
    public abstract class BaseController : Controller
    {
        public IAuthorizationProvider AuthorizationProvider { get; protected set; }
        public virtual Int32 CurrentAccountId { get; protected set; }
        public AlertsContainer Alerts { get; protected set; }

        protected BaseController()
        {
            AuthorizationProvider = Authorization.Provider;
            Alerts = new AlertsContainer();
        }

        public virtual ActionResult NotEmptyView(Object model)
        {
            if (model == null)
                return RedirectToNotFound();

            return View(model);
        }
        public virtual ActionResult RedirectToLocal(String url)
        {
            if (!Url.IsLocalUrl(url))
                return RedirectToDefault();

            return Redirect(url);
        }
        public virtual RedirectToRouteResult RedirectToDefault()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public virtual RedirectToRouteResult RedirectToNotFound()
        {
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }
        public virtual RedirectToRouteResult RedirectToUnauthorized()
        {
            return RedirectToAction("Unauthorized", "Home", new { area = "" });
        }
        public virtual RedirectToRouteResult RedirectIfAuthorized(String action)
        {
            return RedirectIfAuthorized(action, null, null);
        }
        public virtual RedirectToRouteResult RedirectIfAuthorized(String action, Object route)
        {
            return RedirectIfAuthorized(action, null, route);
        }
        public virtual RedirectToRouteResult RedirectIfAuthorized(String action, String controller)
        {
            return RedirectIfAuthorized(action, controller, null);
        }
        public virtual RedirectToRouteResult RedirectIfAuthorized(String action, String controller, Object route)
        {
            RouteValueDictionary values = new RouteValueDictionary(route);
            String area = (values["area"] ?? RouteData.Values["area"]) as String;
            controller = (controller ?? values["controller"] ?? RouteData.Values["controller"]) as String;

            if (!IsAuthorizedFor(action, controller, area))
                return RedirectToDefault();

            return RedirectToAction(action, controller, values);
        }

        public virtual Boolean IsAuthorizedFor(String action, String controller, String area)
        {
            return AuthorizationProvider?.IsAuthorizedFor(CurrentAccountId, area, controller, action) != false;
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, Object state)
        {
            CurrentAccountId = User.Id() ?? 0;

            String abbreviation = RouteData.Values["language"].ToString();
            GlobalizationManager.Languages.Current = GlobalizationManager.Languages[abbreviation];

            return base.BeginExecuteCore(callback, state);
        }
        protected override void OnAuthorization(AuthorizationContext context)
        {
            String area = context.RouteData.Values["area"] as String;
            String action = context.RouteData.Values["action"] as String;
            String controller = context.RouteData.Values["controller"] as String;

            if (User.Identity.IsAuthenticated && !IsAuthorizedFor(action, controller, area))
                context.Result = RedirectToUnauthorized();
        }
        protected override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Result is JsonResult))
            {
                AlertsContainer current = TempData["Alerts"] as AlertsContainer;
                if (current == null)
                    TempData["Alerts"] = Alerts;
                else
                    current.Merge(Alerts);
            }
        }
    }
}
