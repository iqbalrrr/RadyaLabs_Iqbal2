using RadyaLabs.Objects;
using RadyaLabs.Resources;
using RadyaLabs.Resources.Shared;
using RadyaLabs.Tests.Objects;
using System;
using System.Linq.Expressions;
using System.Web.Routing;
using Xunit;

namespace RadyaLabs.Tests.Unit.Resources
{
    public class ResourceProviderTests
    {
        #region GetDatalistTitle(String datalist)

        [Fact]
        public void GetDatalistTitle_IsCaseInsensitive()
        {
            String expected = RadyaLabs.Resources.Datalist.Titles.Role;
            String actual = ResourceProvider.GetDatalistTitle("role");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDatalistTitle_NotFound_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetDatalistTitle("Test"));
        }

        [Fact]
        public void GetDatalistTitle_NullDatalist_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetDatalistTitle(null));
        }

        #endregion

        #region GetPageTitle(RouteValueDictionary values)

        [Fact]
        public void GetPageTitle_IsCaseInsensitive()
        {
            RouteValueDictionary values = new RouteValueDictionary();
            values["area"] = "administration";
            values["controller"] = "roles";
            values["action"] = "details";

            String actual = ResourceProvider.GetPageTitle(values);
            String expected = Pages.AdministrationRolesDetails;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetPageTitle_WithoutArea(String area)
        {
            RouteValueDictionary values = new RouteValueDictionary();
            values["controller"] = "profile";
            values["action"] = "edit";
            values["area"] = area;

            String actual = ResourceProvider.GetPageTitle(values);
            String expected = Pages.ProfileEdit;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPageTitle_NotFound_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPageTitle(new RouteValueDictionary()));
        }

        #endregion

        #region GetSiteMapTitle(String area, String controller, String action)

        [Fact]
        public void GetSiteMapTitle_IsCaseInsensitive()
        {
            String actual = ResourceProvider.GetSiteMapTitle("administration", "roles", "index");
            String expected = RadyaLabs.Resources.SiteMap.Titles.AdministrationRolesIndex;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetSiteMapTitle_WithoutControllerAndAction()
        {
            String actual = ResourceProvider.GetSiteMapTitle("administration", null, null);
            String expected = RadyaLabs.Resources.SiteMap.Titles.Administration;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetSiteMapTitle_NotFound_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetSiteMapTitle("Test", "Test", "Test"));
        }

        #endregion

        #region GetPermissionAreaTitle(String area)

        [Fact]
        public void GetPermissionAreaTitle_IsCaseInsensitive()
        {
            String expected = RadyaLabs.Resources.Permission.Area.Titles.Administration;
            String actual = ResourceProvider.GetPermissionAreaTitle("administration");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPermissionAreaTitle_NotFound_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPermissionAreaTitle("Test"));
        }

        [Fact]
        public void GetPermissionAreaTitle_NullArea_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPermissionAreaTitle(null));
        }

        #endregion

        #region GetPermissionControllerTitle(String area, String controller)

        [Fact]
        public void GetPermissionControllerTitle_ReturnsTitle()
        {
            String expected = RadyaLabs.Resources.Permission.Controller.Titles.AdministrationRoles;
            String actual = ResourceProvider.GetPermissionControllerTitle("Administration", "Roles");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPermissionControllerTitle_NotFound_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPermissionControllerTitle("", ""));
        }

        #endregion

        #region GetPermissionActionTitle(String area, String controller, String action)

        [Fact]
        public void GetPermissionActionTitle_ReturnsTitle()
        {
            String actual = ResourceProvider.GetPermissionActionTitle("administration", "accounts", "index");
            String expected = RadyaLabs.Resources.Permission.Action.Titles.AdministrationAccountsIndex;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPermissionActionTitle_NotFound_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPermissionActionTitle("", "", ""));
        }

        #endregion

        #region GetPropertyTitle<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)

        [Fact]
        public void GetPropertyTitle_NotMemberLambdaExpression_ReturnNull()
        {
            Assert.Null(ResourceProvider.GetPropertyTitle<TestView, String>(view => view.ToString()));
        }

        [Fact]
        public void GetPropertyTitle_FromLambdaExpression()
        {
            String actual = ResourceProvider.GetPropertyTitle<AccountView, String>(account => account.Username);
            String expected = RadyaLabs.Resources.Views.Administration.Accounts.AccountView.Titles.Username;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertyTitle_FromLambdaExpressionRelation()
        {
            String actual = ResourceProvider.GetPropertyTitle<AccountEditView, Int32?>(account => account.RoleId);
            String expected = RadyaLabs.Resources.Views.Administration.Roles.RoleView.Titles.Id;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertyTitle_NotFoundLambdaExpression_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPropertyTitle<AccountView, Int32>(account => account.Id));
        }

        [Fact]
        public void GetPropertyTitle_NotFoundLambdaType_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPropertyTitle<TestView, String>(test => test.Title));
        }

        #endregion

        #region GetPropertyTitle(Type view, String property)

        [Fact]
        public void GetPropertyTitle_IsCaseInsensitive()
        {
            String expected = RadyaLabs.Resources.Views.Administration.Accounts.AccountView.Titles.Username;
            String actual = ResourceProvider.GetPropertyTitle(typeof(AccountView), "username");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertyTitle_FromRelation()
        {
            String expected = RadyaLabs.Resources.Views.Administration.Accounts.AccountView.Titles.Username;
            String actual = ResourceProvider.GetPropertyTitle(typeof(RoleView), "AccountUsername");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertyTitle_FromMultipleRelations()
        {
            String expected = RadyaLabs.Resources.Views.Administration.Accounts.AccountView.Titles.Username;
            String actual = ResourceProvider.GetPropertyTitle(typeof(RoleView), "AccountRoleAccountUsername");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertyTitle_NotFoundProperty_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPropertyTitle(typeof(AccountView), "Id"));
        }

        [Fact]
        public void GetPropertyTitle_NotFoundTypeProperty_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPropertyTitle(typeof(TestView), "Title"));
        }

        [Fact]
        public void GetPropertyTitle_NullKey_ReturnsNull()
        {
            Assert.Null(ResourceProvider.GetPropertyTitle(typeof(RoleView), null));
        }

        #endregion

        #region GetPropertyTitle(Expression property)

        [Fact]
        public void GetPropertyTitle_NotMemberExpression_ReturnNull()
        {
            Expression<Func<TestView, String>> lambda = (view) => view.ToString();

            Assert.Null(ResourceProvider.GetPropertyTitle(lambda.Body));
        }

        [Fact]
        public void GetPropertyTitle_FromExpression()
        {
            Expression<Func<AccountView, String>> lambda = (account) => account.Username;

            String expected = RadyaLabs.Resources.Views.Administration.Accounts.AccountView.Titles.Username;
            String actual = ResourceProvider.GetPropertyTitle(lambda.Body);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertyTitle_FromExpressionRelation()
        {
            Expression<Func<AccountEditView, Int32?>> lambda = (account) => account.RoleId;

            String expected = RadyaLabs.Resources.Views.Administration.Roles.RoleView.Titles.Id;
            String actual = ResourceProvider.GetPropertyTitle(lambda.Body);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertyTitle_NotFoundExpression_ReturnsNull()
        {
            Expression<Func<AccountView, Int32>> lambda = (account) => account.Id;

            Assert.Null(ResourceProvider.GetPropertyTitle(lambda.Body));
        }

        [Fact]
        public void GetPropertyTitle_NotFoundType_ReturnsNull()
        {
            Expression<Func<TestView, String>> lambda = (test) => test.Title;

            Assert.Null(ResourceProvider.GetPropertyTitle(lambda.Body));
        }

        #endregion
    }
}
