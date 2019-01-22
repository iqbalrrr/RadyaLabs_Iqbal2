using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RadyaLabs.Components.Mvc
{
    public class MvcSiteMapParser : IMvcSiteMapParser
    {
        public IEnumerable<MvcSiteMapNode> GetNodeTree(XElement siteMap)
        {
            return GetNodes(siteMap, null);
        }

        private IEnumerable<MvcSiteMapNode> GetNodes(XElement siteMap, MvcSiteMapNode parent)
        {
            List<MvcSiteMapNode> nodes = new List<MvcSiteMapNode>();
            foreach (XElement element in siteMap.Elements("siteMapNode"))
            {
                MvcSiteMapNode node = new MvcSiteMapNode();

                node.IsMenu = (Boolean?)element.Attribute("menu") == true;
                node.Controller = (String)element.Attribute("controller");
                node.IconClass = (String)element.Attribute("icon");
                node.Action = (String)element.Attribute("action");
                node.Area = (String)element.Attribute("area");
                node.Children = GetNodes(element, node);
                node.Parent = parent;

                nodes.Add(node);
            }

            return nodes;
        }
    }
}
