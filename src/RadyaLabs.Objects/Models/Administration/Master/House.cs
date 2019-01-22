using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadyaLabs.Objects
{
    public class House : BaseModel2
    {
        [Required]
        [StringLength(5)]
        public String Code { get; set; }

        [Required]
        [StringLength(150)]
        public String Name { get; set; }

        [Required]
        public Int32 HouseTypeId { get; set; }

        [Required]
        [StringLength(150)]
        public String Address { get; set; }

    }
}
