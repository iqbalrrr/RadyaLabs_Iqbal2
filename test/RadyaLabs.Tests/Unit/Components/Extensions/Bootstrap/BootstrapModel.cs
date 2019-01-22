using System;
using System.ComponentModel.DataAnnotations;

namespace RadyaLabs.Tests.Unit.Components.Extensions
{
    public class BootstrapModel
    {
        [Required]
        public String Required { get; set; }
        public virtual String NotRequired { get; set; }

        public Decimal Number { get; set; }
        public DateTime? Date { get; set; }

        public Boolean Boolean { get; set; }

        [Editable(true)]
        public Decimal EditableTrue { get; set; }

        [Editable(false)]
        public Decimal EditableFalse { get; set; }

        public BootstrapModel Relation { get; set; }
    }
}
