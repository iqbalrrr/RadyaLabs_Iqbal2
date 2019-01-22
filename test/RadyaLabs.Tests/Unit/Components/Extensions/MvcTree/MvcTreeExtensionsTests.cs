using RadyaLabs.Components.Extensions;
using System;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Extensions
{
    public class MvcTreeViewExtensionsTests
    {
        #region TreeFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, MvcTree>> expression, Int32? hideDepth = null, Boolean readOnly = false)

        [Fact]
        public void TreeFor_Expression()
        {
            MvcTreeView tree = new MvcTreeView();
            tree.MvcTree.SelectedIds.Add(123456);
            tree.MvcTree.Nodes.Add(new MvcTreeNode("Test"));
            tree.MvcTree.Nodes[0].Children.Add(new MvcTreeNode(4567, "Test2"));
            tree.MvcTree.Nodes[0].Children.Add(new MvcTreeNode(123456, "Test1"));
            HtmlHelper<MvcTreeView> html = HtmlHelperFactory.CreateHtmlHelper(tree);

            String actual = html.TreeFor(model => model.MvcTree).ToString();
            String expected =
                "<div class=\"mvc-tree\" data-for=\"MvcTree.SelectedIds\">" +
                    "<div class=\"mvc-tree-ids\">" +
                        "<input name=\"MvcTree.SelectedIds\" type=\"hidden\" value=\"123456\" />" +
                    "</div>" +
                    "<ul class=\"mvc-tree-view\">" +
                        "<li class=\"mvc-tree-branch\">" +
                            "<i></i><a href=\"#\">Test</a>" +
                            "<ul>" +
                                "<li data-id=\"4567\">" +
                                    "<i></i><a href=\"#\">Test2</a>" +
                                "</li>" +
                                "<li class=\"mvc-tree-checked\" data-id=\"123456\">" +
                                    "<i></i><a href=\"#\">Test1</a>" +
                                "</li>" +
                            "</ul>" +
                        "</li>" +
                    "</ul>" +
                "</div>";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TreeFor_CollapsedExpression()
        {
            MvcTreeView tree = new MvcTreeView();
            tree.MvcTree.SelectedIds.Add(123456);
            tree.MvcTree.Nodes.Add(new MvcTreeNode("Test"));
            tree.MvcTree.Nodes[0].Children.Add(new MvcTreeNode(4567, "Test2"));
            tree.MvcTree.Nodes[0].Children.Add(new MvcTreeNode(123456, "Test1"));
            HtmlHelper<MvcTreeView> html = HtmlHelperFactory.CreateHtmlHelper(tree);

            String actual = html.TreeFor(model => model.MvcTree, 1).ToString();
            String expected =
                "<div class=\"mvc-tree\" data-for=\"MvcTree.SelectedIds\">" +
                    "<div class=\"mvc-tree-ids\">" +
                        "<input name=\"MvcTree.SelectedIds\" type=\"hidden\" value=\"123456\" />" +
                    "</div>" +
                    "<ul class=\"mvc-tree-view\">" +
                        "<li class=\"mvc-tree-collapsed mvc-tree-branch\">" +
                            "<i></i><a href=\"#\">Test</a>" +
                            "<ul>" +
                                "<li data-id=\"4567\">" +
                                    "<i></i><a href=\"#\">Test2</a>" +
                                "</li>" +
                                "<li class=\"mvc-tree-checked\" data-id=\"123456\">" +
                                    "<i></i><a href=\"#\">Test1</a>" +
                                "</li>" +
                            "</ul>" +
                        "</li>" +
                    "</ul>" +
                "</div>";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TreeFor_ReadonlyExpression()
        {
            MvcTreeView tree = new MvcTreeView();
            tree.MvcTree.SelectedIds.Add(123456);
            tree.MvcTree.Nodes.Add(new MvcTreeNode("Test"));
            tree.MvcTree.Nodes[0].Children.Add(new MvcTreeNode(4567, "Test2"));
            tree.MvcTree.Nodes[0].Children.Add(new MvcTreeNode(123456, "Test1"));
            HtmlHelper<MvcTreeView> html = HtmlHelperFactory.CreateHtmlHelper(tree);

            String actual = html.TreeFor(model => model.MvcTree, null, true).ToString();
            String expected =
                "<div class=\"mvc-tree mvc-tree-readonly\" data-for=\"MvcTree.SelectedIds\">" +
                    "<div class=\"mvc-tree-ids\">" +
                        "<input name=\"MvcTree.SelectedIds\" type=\"hidden\" value=\"123456\" />" +
                    "</div>" +
                    "<ul class=\"mvc-tree-view\">" +
                        "<li class=\"mvc-tree-branch\">" +
                            "<i></i><a href=\"#\">Test</a>" +
                            "<ul>" +
                                "<li data-id=\"4567\">" +
                                    "<i></i><a href=\"#\">Test2</a>" +
                                "</li>" +
                                "<li class=\"mvc-tree-checked\" data-id=\"123456\">" +
                                    "<i></i><a href=\"#\">Test1</a>" +
                                "</li>" +
                            "</ul>" +
                        "</li>" +
                    "</ul>" +
                "</div>";

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
