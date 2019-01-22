using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadyaLabs.Objects
{
    public class Role : BaseModel
    {
        [Required]
        [StringLength(128)]
        [Index(IsUnique = true)]
        public String Title { get; set; }

        public virtual List<Account> Accounts { get; set; }
        public virtual List<RolePermission> Permissions { get; set; }
    }
}
