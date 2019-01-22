using RadyaLabs.Resources.Form;
using System;
using System.ComponentModel.DataAnnotations;

namespace RadyaLabs.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class MinValueAttribute : ValidationAttribute
    {
        public Decimal Minimum { get; }

        public MinValueAttribute(Double minimum)
            : base(() => Validations.MinValue)
        {
            Minimum = Convert.ToDecimal(minimum);
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, Minimum);
        }
        public override Boolean IsValid(Object value)
        {
            if (value == null)
                return true;

            try
            {
                return Convert.ToDecimal(value) >= Minimum;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
