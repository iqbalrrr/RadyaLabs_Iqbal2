using System.Collections.Generic;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class AcceptFilesAdapter : DataAnnotationsModelValidator<AcceptFilesAttribute>
    {
        public AcceptFilesAdapter(ModelMetadata metadata, ControllerContext context, AcceptFilesAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ValidationParameters["extensions"] = Attribute.Extensions;
            rule.ValidationType = "acceptfiles";
            rule.ErrorMessage = ErrorMessage;

            return new[] { rule };
        }
    }
}
