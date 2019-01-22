using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadyaLabs.Objects
{
    public class HouseCollectionHeader : BaseModel2
    {
        [Required]
        [StringLength(8)]
        public String HouseCollectionNumber { get; set; }

        [Required]
        public DateTime HouseCollectionDate { get; set; }
    }
}
