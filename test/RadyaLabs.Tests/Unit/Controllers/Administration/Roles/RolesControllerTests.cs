using RadyaLabs.Components.Security;
using RadyaLabs.Controllers.Administration;
using RadyaLabs.Objects;
using RadyaLabs.Services;
using RadyaLabs.Validators;
using NSubstitute;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers.Administration
{
    public class RolesControllerTests : ControllerTests, IDisposable
    {
        private RolesController controller;
        private IRoleValidator validator;
        private IRoleService service;
        private RoleView role;

        public RolesControllerTests()
        {
            validator = Substitute.For<IRoleValidator>();
            service = Substitute.For<IRoleService>();

            role = ObjectFactory.CreateRoleView();

            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            controller = Substitute.ForPartsOf<RolesController>(validator, service);
            controller.ControllerContext = new ControllerContext { RouteData = new RouteData() };
        }
        public void Dispose()
        {
            Authorization.Provider = null;
        }

        #region Index()

        [Fact]
        public void Index_ReturnsRoleViews()
        {
            service.GetViews().Returns(new RoleView[0].AsQueryable());

            Object actual = controller.Index().Model;
            Object expected = service.GetViews();

            Assert.Same(expected, actual);
        }

        #endregion

        #region Create()

        [Fact]
        public void Create_ReturnsNewRoleView()
        {
            RoleView actual = controller.Create().Model as RoleView;

            Assert.NotNull(actual.Permissions);
            Assert.Null(actual.Title);
        }

        [Fact]
        public void Create_SeedsPermissions()
        {
            RoleView view = controller.Create().Model as RoleView;

            service.Received().SeedPermissions(view);
        }

        #endregion

        #region Create(RoleView role)

        [Fact]
        public void Create_ProtectsFromOverpostingId()
        {
            ProtectsFromOverposting(controller, "Create", "Id");
        }

        [Fact]
        public void Create_CanNotCreate_SeedsPermissions()
        {
            validator.CanCreate(role).Returns(false);

            controller.Create(role);

            service.Received().SeedPermissions(role);
        }

        [Fact]
        public void Create_CanNotCreate_ReturnsSameView()
        {
            validator.CanCreate(role).Returns(false);

            Object actual = (controller.Create(role) as ViewResult).Model;
            Object expected = role;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_Role()
        {
            validator.CanCreate(role).Returns(true);

            controller.Create(role);

            service.Received().Create(role);
        }

        [Fact]
        public void Create_RedirectsToIndex()
        {
            validator.CanCreate(role).Returns(true);

            Object expected = RedirectIfAuthorized(controller, "Index");
            Object actual = controller.Create(role);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Details(Int32 id)

        [Fact]
        public void Details_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);

            Object expected = NotEmptyView(controller, role);
            Object actual = controller.Details(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Edit(Int32 id)

        [Fact]
        public void Edit_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);

            Object expected = NotEmptyView(controller, role);
            Object actual = controller.Edit(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Edit(RoleView role)

        [Fact]
        public void Edit_CanNotEdit_SeedsPermissions()
        {
            validator.CanEdit(role).Returns(false);

            controller.Edit(role);

            service.Received().SeedPermissions(role);
        }

        [Fact]
        public void Edit_CanNotEdit_ReturnsSameView()
        {
            validator.CanEdit(role).Returns(false);

            Object actual = (controller.Edit(role) as ViewResult).Model;
            Object expected = role;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_Role()
        {
            validator.CanEdit(role).Returns(true);

            controller.Edit(role);

            service.Received().Edit(role);
        }

        [Fact]
        public void Edit_RefreshesAuthorization()
        {
            validator.CanEdit(role).Returns(true);

            controller.Edit(role);

            controller.AuthorizationProvider.Received().Refresh();
        }

        [Fact]
        public void Edit_RedirectsToIndex()
        {
            validator.CanEdit(role).Returns(true);

            Object expected = RedirectIfAuthorized(controller, "Index");
            Object actual = controller.Edit(role);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Delete(Int32 id)

        [Fact]
        public void Delete_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);

            Object expected = NotEmptyView(controller, role);
            Object actual = controller.Delete(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region DeleteConfirmed(Int32 id)

        [Fact]
        public void DeleteConfirmed_DeletesRole()
        {
            controller.DeleteConfirmed(role.Id);

            service.Received().Delete(role.Id);
        }

        [Fact]
        public void DeleteConfirmed_RefreshesAuthorization()
        {
            controller.DeleteConfirmed(role.Id);

            controller.AuthorizationProvider.Received().Refresh();
        }

        [Fact]
        public void DeleteConfirmed_RedirectsToIndex()
        {
            Object expected = RedirectIfAuthorized(controller, "Index");
            Object actual = controller.DeleteConfirmed(role.Id);

            Assert.Same(expected, actual);
        }

        #endregion
    }
}
