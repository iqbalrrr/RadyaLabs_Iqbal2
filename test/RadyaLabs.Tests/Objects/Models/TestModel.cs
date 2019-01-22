using RadyaLabs.Objects;
using System;
using System.ComponentModel.DataAnnotations;

namespace RadyaLabs.Tests.Objects
{
    public class TestModel : BaseModel
    {
        [StringLength(128)]
        public String Title { get; set; }
    }
}
