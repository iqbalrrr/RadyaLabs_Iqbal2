using System.Collections.Generic;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class IntegerAdapter : DataAnnotationsModelValidator<IntegerAttribute>
    {
        public IntegerAdapter(ModelMetadata metadata, ControllerContext context, IntegerAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ErrorMessage = ErrorMessage;
            rule.ValidationType = "integer";

            return new[] { rule };
        }
    }
}
