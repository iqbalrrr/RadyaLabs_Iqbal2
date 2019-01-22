using RadyaLabs.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace RadyaLabs.Objects
{
    public class HouseInputView : BaseView
    {
        [Required]
        [StringLength(150)]
        public String Code { get; set; }

        [Required]
        [StringLength(150)]
        public String Name { get; set; }

        [Required]
        [StringLength(150)]
        public String Type { get; set; }
    }
}
