using RadyaLabs.Components.Mvc;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class MvcSiteMapParserTests
    {
        #region GetNodeTree(XElement siteMap)

        [Fact]
        public void GetNodes_ReturnsAllSiteMapNodes()
        {
            List<MvcSiteMapNode> actual = ToList(new MvcSiteMapParser().GetNodeTree(CreateSiteMap()));
            List<MvcSiteMapNode> expected = ToList(GetExpectedNodeTree());

            for (Int32 i = 0; i < expected.Count || i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Controller, actual[i].Controller);
                Assert.Equal(expected[i].IconClass, actual[i].IconClass);
                Assert.Equal(expected[i].IsMenu, actual[i].IsMenu);
                Assert.Equal(expected[i].Action, actual[i].Action);
                Assert.Equal(expected[i].Area, actual[i].Area);

                if (expected[i].Parent != null || actual[i].Parent != null)
                {
                    Assert.Equal(expected[i].Parent.Controller, actual[i].Parent.Controller);
                    Assert.Equal(expected[i].Parent.IconClass, actual[i].Parent.IconClass);
                    Assert.Equal(expected[i].Parent.IsMenu, actual[i].Parent.IsMenu);
                    Assert.Equal(expected[i].Parent.Action, actual[i].Parent.Action);
                    Assert.Equal(expected[i].Parent.Area, actual[i].Parent.Area);
                }
            }
        }

        #endregion

        #region Test helpers

        private XElement CreateSiteMap()
        {
            return XElement.Parse(
            @"<siteMap>
              <siteMapNode controller=""Home"" action=""Index"" menu=""false"" icon=""fa fa-home"">
                <siteMapNode menu=""true"" area=""Administration"" icon=""fa fa-cogs"">
                  <siteMapNode controller=""Accounts"" action=""Index"" menu=""false"" area=""Administration"" icon=""fa fa-user"">
                    <siteMapNode controller=""Accounts"" action=""Create"" menu=""true"" area=""Administration"" icon=""far fa-file"" />
                  </siteMapNode>
                </siteMapNode>
              </siteMapNode>
            </siteMap>");
        }
        private MvcSiteMapNode[] GetExpectedNodeTree()
        {
            MvcSiteMapNode[] map =
            {
                new MvcSiteMapNode
                {
                    IconClass = "fa fa-home",

                    Controller = "Home",
                    Action = "Index",

                    Children = new[]
                    {
                        new MvcSiteMapNode
                        {
                            IconClass = "fa fa-cogs",
                            IsMenu = true,

                            Area = "Administration",

                            Children = new[]
                            {
                                new MvcSiteMapNode
                                {
                                    IconClass = "fa fa-user",
                                    IsMenu = false,

                                    Area = "Administration",
                                    Controller = "Accounts",
                                    Action = "Index",

                                    Children = new[]
                                    {
                                        new MvcSiteMapNode
                                        {
                                            IconClass = "far fa-file",
                                            IsMenu = true,

                                            Area = "Administration",
                                            Controller = "Accounts",
                                            Action = "Create",

                                            Children = new MvcSiteMapNode[0]
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            foreach (MvcSiteMapNode level1 in map)
            {
                foreach (MvcSiteMapNode level2 in level1.Children)
                {
                    level2.Parent = level1;

                    foreach (MvcSiteMapNode level3 in level2.Children)
                    {
                        level3.Parent = level2;

                        foreach (MvcSiteMapNode level4 in level3.Children)
                        {
                            level4.Parent = level3;
                        }
                    }
                }
            }

            return map;
        }
        private List<MvcSiteMapNode> ToList(IEnumerable<MvcSiteMapNode> nodes)
        {
            List<MvcSiteMapNode> list = new List<MvcSiteMapNode>();
            foreach (MvcSiteMapNode node in nodes)
            {
                list.Add(node);
                list.AddRange(ToList(node.Children));
            }

            return list;
        }

        #endregion
    }
}
