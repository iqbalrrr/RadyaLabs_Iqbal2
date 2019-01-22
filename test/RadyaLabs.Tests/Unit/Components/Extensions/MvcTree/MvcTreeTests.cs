using RadyaLabs.Components.Extensions;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Extensions
{
    public class MvcTreeTests
    {
        #region MvcTree()

        [Fact]
        public void MvcTree_CreatesEmpty()
        {
            MvcTree actual = new MvcTree();

            Assert.Empty(actual.Nodes);
            Assert.Empty(actual.SelectedIds);
        }

        #endregion
    }
}
