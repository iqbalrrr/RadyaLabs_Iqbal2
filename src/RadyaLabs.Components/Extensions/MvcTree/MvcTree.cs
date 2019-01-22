using System;
using System.Collections.Generic;

namespace RadyaLabs.Components.Extensions
{
    public class MvcTree
    {
        public List<MvcTreeNode> Nodes { get; set; }
        public HashSet<Int32> SelectedIds { get; set; }

        public MvcTree()
        {
            Nodes = new List<MvcTreeNode>();
            SelectedIds = new HashSet<Int32>();
        }
    }
}
