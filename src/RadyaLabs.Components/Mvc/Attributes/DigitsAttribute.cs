using RadyaLabs.Resources.Form;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RadyaLabs.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DigitsAttribute : ValidationAttribute
    {
        public DigitsAttribute()
            : base(() => Validations.Digits)
        {
        }

        public override Boolean IsValid(Object value)
        {
            return value == null || Regex.IsMatch(value.ToString(), "^[0-9]+$");
        }
    }
}
