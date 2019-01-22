using RadyaLabs.Resources;
using RadyaLabs.Resources.Form;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RadyaLabs.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EqualToAttribute : ValidationAttribute
    {
        public String OtherPropertyName { get; }
        public String OtherPropertyDisplayName { get; set; }

        public EqualToAttribute(String otherPropertyName)
            : base(() => Validations.EqualTo)
        {
            if (otherPropertyName == null)
                throw new ArgumentNullException(nameof(otherPropertyName));

            OtherPropertyName = otherPropertyName;
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, OtherPropertyDisplayName);
        }
        protected override ValidationResult IsValid(Object value, ValidationContext context)
        {
            PropertyInfo other = context.ObjectType.GetProperty(OtherPropertyName);
            if (other != null && Equals(value, other.GetValue(context.ObjectInstance)))
                return null;

            OtherPropertyDisplayName = ResourceProvider.GetPropertyTitle(context.ObjectType, OtherPropertyName) ?? OtherPropertyName;

            return new ValidationResult(FormatErrorMessage(context.DisplayName));
        }
    }
}
