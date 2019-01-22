using RadyaLabs.Components.Alerts;
using RadyaLabs.Components.Security;
using RadyaLabs.Controllers;
using RadyaLabs.Objects;
using RadyaLabs.Resources.Views.Administration.Accounts.AccountView;
using RadyaLabs.Services;
using RadyaLabs.Validators;
using NSubstitute;
using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public class ProfileControllerTests : ControllerTests, IDisposable
    {
        private ProfileDeleteView profileDelete;
        private ProfileController controller;
        private ProfileEditView profileEdit;
        private IAccountValidator validator;
        private IAccountService service;

        public ProfileControllerTests()
        {
            validator = Substitute.For<IAccountValidator>();
            service = Substitute.For<IAccountService>();

            profileDelete = ObjectFactory.CreateProfileDeleteView();
            profileEdit = ObjectFactory.CreateProfileEditView();

            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            controller = Substitute.ForPartsOf<ProfileController>(validator, service);
            ReturnCurrentAccountId(controller, 1);
        }
        public void Dispose()
        {
            Authorization.Provider = null;
        }

        #region Edit()

        [Fact]
        public void Edit_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectIfAuthorized(controller, "Logout", "Auth");
            Object actual = controller.Edit();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_ReturnsProfileView()
        {
            service.Get<ProfileEditView>(controller.CurrentAccountId).Returns(profileEdit);
            service.IsActive(controller.CurrentAccountId).Returns(true);

            Object actual = (controller.Edit() as ViewResult).Model;
            Object expected = profileEdit;

            Assert.Same(expected, actual);
        }

        #endregion

        #region Edit(ProfileEditView profile)

        [Fact]
        public void Edit_ProtectsFromOverpostingId()
        {
            ProtectsFromOverposting(controller, "Edit", "Id");
        }

        [Fact]
        public void Edit_Post_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectIfAuthorized(controller, "Logout", "Auth");
            Object actual = controller.Edit(null);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_CanNotEdit_ReturnsSameView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanEdit(profileEdit).Returns(false);

            Object actual = (controller.Edit(profileEdit) as ViewResult).Model;
            Object expected = profileEdit;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_Profile()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanEdit(profileEdit).Returns(true);
            RedirectIfAuthorized(controller, "Edit");

            controller.Edit(profileEdit);

            service.Received().Edit(profileEdit);
        }

        [Fact]
        public void Edit_AddsUpdatedMessage()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanEdit(profileEdit).Returns(true);
            RedirectIfAuthorized(controller, "Edit");

            controller.Edit(profileEdit);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Messages.ProfileUpdated, actual.Message);
            Assert.Equal(AlertType.Success, actual.Type);
            Assert.Equal(4000, actual.Timeout);
        }

        [Fact]
        public void Edit_RedirectsToEdit()
        {
            validator.CanEdit(profileEdit).Returns(true);
            service.IsActive(controller.CurrentAccountId).Returns(true);

            Object expected = RedirectIfAuthorized(controller, "Edit");
            Object actual = controller.Edit(profileEdit);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Delete()

        [Fact]
        public void Delete_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectIfAuthorized(controller, "Logout", "Auth");
            Object actual = controller.Delete();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Delete_AddsDisclaimerMessage()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            controller.Delete();

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Messages.ProfileDeleteDisclaimer, actual.Message);
            Assert.Equal(AlertType.Warning, actual.Type);
            Assert.Equal(0, actual.Timeout);
        }

        [Fact]
        public void Delete_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            ViewResult actual = controller.Delete() as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion

        #region DeleteConfirmed(ProfileDeleteView profile)

        [Fact]
        public void DeleteConfirmed_ProtectsFromOverpostingId()
        {
            ProtectsFromOverposting(controller, "DeleteConfirmed", "Id");
        }

        [Fact]
        public void DeleteConfirmed_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectIfAuthorized(controller, "Logout", "Auth");
            Object actual = controller.DeleteConfirmed(profileDelete);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void DeleteConfirmed_CanNotDelete_AddsDisclaimerMessage()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(false);

            controller.DeleteConfirmed(profileDelete);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Messages.ProfileDeleteDisclaimer, actual.Message);
            Assert.Equal(AlertType.Warning, actual.Type);
            Assert.Equal(0, actual.Timeout);
        }

        [Fact]
        public void DeleteConfirmed_CanNotDelete_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(false);

            ViewResult actual = controller.DeleteConfirmed(profileDelete) as ViewResult;

            Assert.Null(actual.Model);
        }

        [Fact]
        public void DeleteConfirmed_DeletesProfile()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            RedirectIfAuthorized(controller, "Logout", "Auth");
            validator.CanDelete(profileDelete).Returns(true);

            controller.DeleteConfirmed(profileDelete);

            service.Received().Delete(controller.CurrentAccountId);
        }

        [Fact]
        public void DeleteConfirmed_RefreshesAuthorization()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            RedirectIfAuthorized(controller, "Logout", "Auth");
            validator.CanDelete(profileDelete).Returns(true);

            controller.DeleteConfirmed(profileDelete);

            controller.AuthorizationProvider.Received().Refresh();
        }

        [Fact]
        public void DeleteConfirmed_RedirectsToAuthLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(true);

            Object expected = RedirectIfAuthorized(controller, "Logout", "Auth");
            Object actual = controller.DeleteConfirmed(profileDelete);

            Assert.Same(expected, actual);
        }

        #endregion
    }
}
