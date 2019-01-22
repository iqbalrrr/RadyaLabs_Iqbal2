using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadyaLabs.Objects
{
    public class HouseCollectionDetail : BaseModel2
    {
        [Required]
        public int HouseCollectionHeaderId { get; set; }

        [Required]
        public int HouseId { get; set; }

        [Required]
        public decimal DistanceKM { get; set; }

        [Required]
        public decimal DistanceTime { get; set; }
    }
}
