using Datalist;
using RadyaLabs.Components.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace RadyaLabs.Objects
{
    public class RoleView : BaseView
    {
        [Required]
        [DatalistColumn]
        [StringLength(128)]
        public String Title { get; set; }

        public MvcTree Permissions { get; set; }

        public RoleView()
        {
            Permissions = new MvcTree();
        }
    }
}
