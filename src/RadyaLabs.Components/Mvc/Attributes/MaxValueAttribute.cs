using RadyaLabs.Resources.Form;
using System;
using System.ComponentModel.DataAnnotations;

namespace RadyaLabs.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class MaxValueAttribute : ValidationAttribute
    {
        public Decimal Maximum { get; }

        public MaxValueAttribute(Double maximum)
            : base(() => Validations.MaxValue)
        {
            Maximum = Convert.ToDecimal(maximum);
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, Maximum);
        }
        public override Boolean IsValid(Object value)
        {
            if (value == null)
                return true;

            try
            {
                return Convert.ToDecimal(value) <= Maximum;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
