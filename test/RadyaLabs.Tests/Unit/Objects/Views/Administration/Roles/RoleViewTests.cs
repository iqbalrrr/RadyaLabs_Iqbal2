using RadyaLabs.Components.Extensions;
using RadyaLabs.Objects;
using Xunit;

namespace RadyaLabs.Tests.Unit.Objects
{
    public class RoleViewTests
    {
        #region RoleView()

        [Fact]
        public void RoleView_CreatesEmpty()
        {
            MvcTree actual = new RoleView().Permissions;

            Assert.Empty(actual.SelectedIds);
            Assert.Empty(actual.Nodes);
        }

        #endregion
    }
}
