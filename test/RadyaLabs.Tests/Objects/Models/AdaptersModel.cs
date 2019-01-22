using RadyaLabs.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RadyaLabs.Tests.Objects
{
    public class AdaptersModel
    {
        [Range(0, 128)]
        public Int32? Range { get; set; }

        [Digits]
        public String Digits { get; set; }

        [EqualTo("StringLength")]
        public String EqualTo { get; set; }

        [Integer]
        public Int32? Integer { get; set; }

        [Required]
        public Int32? Required { get; set; }

        [MinValue(128)]
        public String MinValue { get; set; }

        [MaxValue(128)]
        public String MaxValue { get; set; }

        [MinLength(128)]
        public String MinLength { get; set; }

        [GreaterThan(128)]
        public String GreaterThan { get; set; }

        [EmailAddress]
        public String EmailAddress { get; set; }

        [StringLength(128)]
        public String StringLength { get; set; }

        [FileSize(12.25)]
        public HttpPostedFileBase FileSize { get; set; }

        [AcceptFiles(".docx,.rtf")]
        public HttpPostedFileBase AcceptFiles { get; set; }
    }
}
