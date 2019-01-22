using System;
using System.Collections.Generic;

namespace RadyaLabs.Components.Mvc
{
    public class MvcSiteMapNode
    {
        public Boolean IsMenu { get; set; }
        public String IconClass { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean HasActiveChildren { get; set; }

        public String Controller { get; set; }
        public String Action { get; set; }
        public String Area { get; set; }

        public MvcSiteMapNode Parent { get; set; }
        public IEnumerable<MvcSiteMapNode> Children { get; set; }
    }
}
