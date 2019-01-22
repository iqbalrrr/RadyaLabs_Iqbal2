using System.Collections.Generic;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class MinValueAdapter : DataAnnotationsModelValidator<MinValueAttribute>
    {
        public MinValueAdapter(ModelMetadata metadata, ControllerContext context, MinValueAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ValidationParameters["min"] = Attribute.Minimum;
            rule.ErrorMessage = ErrorMessage;
            rule.ValidationType = "range";

            return new[] { rule };
        }
    }
}
