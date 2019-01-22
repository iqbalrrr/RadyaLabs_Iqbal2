using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class FileSizeAdapter : DataAnnotationsModelValidator<FileSizeAttribute>
    {
        public FileSizeAdapter(ModelMetadata metadata, ControllerContext context, FileSizeAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            Decimal bytes = Attribute.MaximumMB * 1024 * 1024;
            rule.ValidationParameters["max"] = bytes;
            rule.ErrorMessage = ErrorMessage;
            rule.ValidationType = "filesize";

            return new[] { rule };
        }
    }
}
