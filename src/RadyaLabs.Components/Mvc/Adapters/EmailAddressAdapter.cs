using RadyaLabs.Resources.Form;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class EmailAddressAdapter : DataAnnotationsModelValidator<EmailAddressAttribute>
    {
        public EmailAddressAdapter(ModelMetadata metadata, ControllerContext context, EmailAddressAttribute attribute)
            : base(metadata, context, attribute)
        {
            Attribute.ErrorMessage = Validations.Email;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ErrorMessage = ErrorMessage;
            rule.ValidationType = "email";

            return new[] { rule };
        }
    }
}
