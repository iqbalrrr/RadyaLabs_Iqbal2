using RadyaLabs.Objects;
using System;
using System.ComponentModel.DataAnnotations;

namespace RadyaLabs.Tests.Objects
{
    public class TestView : BaseView
    {
        [StringLength(128)]
        public String Title { get; set; }
    }
}
