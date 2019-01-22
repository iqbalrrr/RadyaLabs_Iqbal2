using System.Collections.Generic;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class DigitsAdapter : DataAnnotationsModelValidator<DigitsAttribute>
    {
        public DigitsAdapter(ModelMetadata metadata, ControllerContext context, DigitsAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ErrorMessage = ErrorMessage;
            rule.ValidationType = "digits";

            return new[] { rule };
        }
    }
}
