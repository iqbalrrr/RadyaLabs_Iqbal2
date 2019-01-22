using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadyaLabs.Objects
{
    public class HouseType : BaseModel2
    {
        [Required]
        [StringLength(5)]
        public String Code { get; set; }

        [Required]
        [StringLength(50)]
        public String Name { get; set; }
    }
}
