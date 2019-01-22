using RadyaLabs.Components.Mvc;
using RadyaLabs.Objects;
using RadyaLabs.Services;
using RadyaLabs.Validators;
using System;
using System.Web.Mvc;

namespace RadyaLabs.Controllers.Administration
{
    [Area("Administration")]
    public class RolesController : ValidatedController<IRoleValidator, IRoleService>
    {
        public RolesController(IRoleValidator validator, IRoleService service)
            : base(validator, service)
        {
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View(Service.GetViews());
        }

        [HttpGet]
        public ViewResult Create()
        {
            RoleView role = new RoleView();
            Service.SeedPermissions(role);

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] RoleView role)
        {
            if (!Validator.CanCreate(role))
            {
                Service.SeedPermissions(role);

                return View(role);
            }

            Service.Create(role);

            return RedirectIfAuthorized("Index");
        }

        [HttpGet]
        public ActionResult Details(Int32 id)
        {
            return NotEmptyView(Service.GetView(id));
        }

        [HttpGet]
        public ActionResult Edit(Int32 id)
        {
            return NotEmptyView(Service.GetView(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoleView role)
        {
            if (!Validator.CanEdit(role))
            {
                Service.SeedPermissions(role);

                return View(role);
            }

            Service.Edit(role);

            AuthorizationProvider?.Refresh();

            return RedirectIfAuthorized("Index");
        }

        [HttpGet]
        public ActionResult Delete(Int32 id)
        {
            return NotEmptyView(Service.GetView(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult DeleteConfirmed(Int32 id)
        {
            Service.Delete(id);

            AuthorizationProvider?.Refresh();

            return RedirectIfAuthorized("Index");
        }
    }
}
