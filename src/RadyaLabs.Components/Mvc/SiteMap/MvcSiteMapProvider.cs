using RadyaLabs.Components.Extensions;
using RadyaLabs.Components.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace RadyaLabs.Components.Mvc
{
    public class MvcSiteMapProvider : IMvcSiteMapProvider
    {
        private IEnumerable<MvcSiteMapNode> AllNodes { get; }
        private IEnumerable<MvcSiteMapNode> NodeTree { get; }

        public MvcSiteMapProvider(String path, IMvcSiteMapParser parser)
        {
            XElement siteMap = XElement.Load(path);
            NodeTree = parser.GetNodeTree(siteMap);
            AllNodes = ToList(NodeTree);
        }

        public IEnumerable<MvcSiteMapNode> GetSiteMap(ViewContext context)
        {
            Int32? account = context.HttpContext.User.Id();
            String area = context.RouteData.Values["area"] as String;
            String action = context.RouteData.Values["action"] as String;
            String controller = context.RouteData.Values["controller"] as String;
            IEnumerable<MvcSiteMapNode> nodes = CopyAndSetState(NodeTree, area, controller, action);

            return GetAuthorizedNodes(account, nodes);
        }
        public IEnumerable<MvcSiteMapNode> GetBreadcrumb(ViewContext context)
        {
            String area = context.RouteData.Values["area"] as String;
            String action = context.RouteData.Values["action"] as String;
            String controller = context.RouteData.Values["controller"] as String;

            MvcSiteMapNode current = AllNodes.SingleOrDefault(node =>
                String.Equals(node.Area, area, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(node.Action, action, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(node.Controller, controller, StringComparison.OrdinalIgnoreCase));

            List<MvcSiteMapNode> breadcrumb = new List<MvcSiteMapNode>();
            while (current != null)
            {
                breadcrumb.Insert(0, new MvcSiteMapNode
                {
                    IconClass = current.IconClass,

                    Controller = current.Controller,
                    Action = current.Action,
                    Area = current.Area
                });

                current = current.Parent;
            }

            return breadcrumb;
        }

        private IEnumerable<MvcSiteMapNode> CopyAndSetState(IEnumerable<MvcSiteMapNode> nodes, String area, String controller, String action)
        {
            List<MvcSiteMapNode> copies = new List<MvcSiteMapNode>();
            foreach (MvcSiteMapNode node in nodes)
            {
                MvcSiteMapNode copy = new MvcSiteMapNode();
                copy.IconClass = node.IconClass;
                copy.IsMenu = node.IsMenu;

                copy.Controller = node.Controller;
                copy.Action = node.Action;
                copy.Area = node.Area;

                copy.Children = CopyAndSetState(node.Children, area, controller, action);
                copy.HasActiveChildren = copy.Children.Any(child => child.IsActive || child.HasActiveChildren);
                copy.IsActive =
                    copy.Children.Any(child => child.IsActive && !child.IsMenu) ||
                    (
                        String.Equals(node.Area, area, StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(node.Action, action, StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(node.Controller, controller, StringComparison.OrdinalIgnoreCase)
                    );

                copies.Add(copy);
            }

            return copies;
        }
        private IEnumerable<MvcSiteMapNode> GetAuthorizedNodes(Int32? accountId, IEnumerable<MvcSiteMapNode> nodes)
        {
            List<MvcSiteMapNode> authorized = new List<MvcSiteMapNode>();
            foreach (MvcSiteMapNode node in nodes)
            {
                node.Children = GetAuthorizedNodes(accountId, node.Children);

                if (node.IsMenu && IsAuthorizedToView(accountId, node.Area, node.Controller, node.Action) && !IsEmpty(node))
                    authorized.Add(node);
                else
                    authorized.AddRange(node.Children);
            }

            return authorized;
        }

        private Boolean IsAuthorizedToView(Int32? accountId, String area, String controller, String action)
        {
            return action == null || Authorization.Provider?.IsAuthorizedFor(accountId, area, controller, action) != false;
        }
        private IEnumerable<MvcSiteMapNode> ToList(IEnumerable<MvcSiteMapNode> nodes)
        {
            List<MvcSiteMapNode> list = new List<MvcSiteMapNode>();
            foreach (MvcSiteMapNode node in nodes)
            {
                list.Add(node);
                list.AddRange(ToList(node.Children));
            }

            return list;
        }
        private Boolean IsEmpty(MvcSiteMapNode node)
        {
            return node.Action == null && !node.Children.Any();
        }
    }
}
