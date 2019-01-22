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
    public class AccountsControllerTests : ControllerTests, IDisposable
    {
        private AccountCreateView accountCreate;
        private AccountsController controller;
        private IAccountValidator validator;
        private AccountEditView accountEdit;
        private IAccountService service;
        private AccountView account;

        public AccountsControllerTests()
        {
            validator = Substitute.For<IAccountValidator>();
            service = Substitute.For<IAccountService>();

            accountCreate = ObjectFactory.CreateAccountCreateView();
            accountEdit = ObjectFactory.CreateAccountEditView();
            account = ObjectFactory.CreateAccountView();

            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            controller = Substitute.ForPartsOf<AccountsController>(validator, service);
            controller.ControllerContext = new ControllerContext { RouteData = new RouteData() };
        }
        public void Dispose()
        {
            Authorization.Provider = null;
        }

        #region Index()

        [Fact]
        public void Index_ReturnsAccountViews()
        {
            service.GetViews().Returns(new AccountView[0].AsQueryable());

            Object actual = controller.Index().Model;
            Object expected = service.GetViews();

            Assert.Same(expected, actual);
        }

        #endregion

        #region Create()

        [Fact]
        public void Create_ReturnsEmptyView()
        {
            ViewResult actual = controller.Create();

            Assert.Null(actual.Model);
        }

        #endregion

        #region Create(AccountCreateView account)

        [Fact]
        public void Create_ProtectsFromOverpostingId()
        {
            ProtectsFromOverposting(controller, "Create", "Id");
        }

        [Fact]
        public void Create_CanNotCreate_ReturnsSameView()
        {
            validator.CanCreate(accountCreate).Returns(false);

            Object actual = (controller.Create(accountCreate) as ViewResult).Model;
            Object expected = accountCreate;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_Account()
        {
            validator.CanCreate(accountCreate).Returns(true);

            controller.Create(accountCreate);

            service.Received().Create(accountCreate);
        }

        [Fact]
        public void Create_RefreshesAuthorization()
        {
            validator.CanCreate(accountCreate).Returns(true);

            controller.Create(accountCreate);

            controller.AuthorizationProvider.Received().Refresh();
        }

        [Fact]
        public void Create_RedirectsToIndex()
        {
            validator.CanCreate(accountCreate).Returns(true);

            Object expected = RedirectIfAuthorized(controller, "Index");
            Object actual = controller.Create(accountCreate);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Details(Int32 id)

        [Fact]
        public void Details_ReturnsNotEmptyView()
        {
            service.Get<AccountView>(account.Id).Returns(account);

            Object expected = NotEmptyView(controller, account);
            Object actual = controller.Details(account.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Edit(Int32 id)

        [Fact]
        public void Edit_ReturnsNotEmptyView()
        {
            service.Get<AccountEditView>(accountEdit.Id).Returns(accountEdit);

            Object expected = NotEmptyView(controller, accountEdit);
            Object actual = controller.Edit(accountEdit.Id);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Edit(AccountEditView account)

        [Fact]
        public void Edit_CanNotEdit_ReturnsSameView()
        {
            validator.CanEdit(accountEdit).Returns(false);

            Object actual = (controller.Edit(accountEdit) as ViewResult).Model;
            Object expected = accountEdit;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_Account()
        {
            validator.CanEdit(accountEdit).Returns(true);

            controller.Edit(accountEdit);

            service.Received().Edit(accountEdit);
        }

        [Fact]
        public void Edit_RefreshesAuthorization()
        {
            validator.CanEdit(accountEdit).Returns(true);

            controller.Edit(accountEdit);

            controller.AuthorizationProvider.Received().Refresh();
        }

        [Fact]
        public void Edit_RedirectsToIndex()
        {
            validator.CanEdit(accountEdit).Returns(true);

            Object expected = RedirectIfAuthorized(controller, "Index");
            Object actual = controller.Edit(accountEdit);

            Assert.Same(expected, actual);
        }

        #endregion
    }
}
