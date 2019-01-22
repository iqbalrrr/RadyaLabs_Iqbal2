using RadyaLabs.Components.Extensions;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Extensions
{
    public class MvcTreeNodeTests
    {
        #region MvcTreeNode(String title)

        [Fact]
        public void MvcTreeNode_SetsTitle()
        {
            MvcTreeNode actual = new MvcTreeNode("Title");

            Assert.Equal("Title", actual.Title);
            Assert.Empty(actual.Children);
            Assert.Null(actual.Id);
        }

        #endregion

        #region MvcTreeNode(Int32? id, String title)

        [Fact]
        public void MvcTreeNode_SetsIdAndTitle()
        {
            MvcTreeNode actual = new MvcTreeNode(1, "Title");

            Assert.Equal("Title", actual.Title);
            Assert.Equal(1, actual.Id);
            Assert.Empty(actual.Children);
        }

        #endregion
    }
}
