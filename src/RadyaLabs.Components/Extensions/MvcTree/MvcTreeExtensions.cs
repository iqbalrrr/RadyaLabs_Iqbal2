using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace RadyaLabs.Components.Extensions
{
    public static class MvcTreeExtensions
    {
        public static MvcHtmlString TreeFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, MvcTree>> expression, Int32? hideDepth = null, Boolean readOnly = false)
        {
            MvcTree model = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model as MvcTree;
            String name = ExpressionHelper.GetExpressionText(expression) + ".SelectedIds";
            TagBuilder tree = new TagBuilder("div");

            tree.AddCssClass("mvc-tree");
            tree.Attributes["data-for"] = name;

            if (readOnly)
                tree.Attributes["class"] += " mvc-tree-readonly";

            tree.InnerHtml = IdsFor(name, model) + ViewFor(model, hideDepth);

            return new MvcHtmlString(tree.ToString());
        }

        private static String IdsFor(String name, MvcTree model)
        {
            StringBuilder inputs = new StringBuilder();
            TagBuilder input = new TagBuilder("input");
            input.Attributes["type"] = "hidden";
            input.Attributes["name"] = name;

            foreach (Int32 id in model.SelectedIds)
            {
                input.Attributes["value"] = id.ToString();
                inputs.Append(input.ToString(TagRenderMode.SelfClosing));
            }

            TagBuilder ids = new TagBuilder("div");
            ids.InnerHtml = inputs.ToString();
            ids.AddCssClass("mvc-tree-ids");

            return ids.ToString();
        }
        private static String ViewFor(MvcTree model, Int32? hideDepth)
        {
            TagBuilder root = new TagBuilder("ul");
            root.AddCssClass("mvc-tree-view");

            return Build(model, root, model.Nodes, 1, hideDepth).ToString();
        }
        private static TagBuilder Build(MvcTree model, TagBuilder branch, List<MvcTreeNode> nodes, Int32? depth, Int32? hideDepth)
        {
            StringBuilder nodeBuilder = new StringBuilder();

            foreach (MvcTreeNode node in nodes)
            {
                TagBuilder item = new TagBuilder("li");
                item.InnerHtml = "<i></i>";

                if (node.Id != null)
                {
                    if (model.SelectedIds.Contains(node.Id.Value))
                        item.AddCssClass("mvc-tree-checked");

                    item.Attributes["data-id"] = node.Id.ToString();
                }

                TagBuilder anchor = new TagBuilder("a");
                anchor.Attributes["href"] = "#";
                anchor.InnerHtml += node.Title;

                item.InnerHtml += anchor;

                if (node.Children.Count > 0)
                {
                    item.AddCssClass("mvc-tree-branch");

                    if (hideDepth <= depth)
                        item.AddCssClass("mvc-tree-collapsed");

                    item.InnerHtml += Build(model, new TagBuilder("ul"), node.Children, depth + 1, hideDepth).ToString();
                }

                nodeBuilder.Append(item);
            }

            branch.InnerHtml = nodeBuilder.ToString();

            return branch;
        }
    }
}
