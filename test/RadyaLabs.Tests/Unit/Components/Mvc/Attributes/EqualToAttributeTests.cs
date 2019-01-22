using RadyaLabs.Components.Mvc;
using RadyaLabs.Resources.Form;
using RadyaLabs.Tests.Objects;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class EqualToAttributeTests
    {
        private EqualToAttribute attribute;

        public EqualToAttributeTests()
        {
            attribute = new EqualToAttribute("EqualTo");
        }

        #region EqualToAttribute(String otherPropertyName)

        [Fact]
        public void EqualToAttribute_NullProperty_Throws()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new EqualToAttribute(null));
            Assert.Equal("otherPropertyName", exception.ParamName);
        }

        [Fact]
        public void EqualToAttribute_SetsOtherPropertyName()
        {
            String actual = new EqualToAttribute("Other").OtherPropertyName;
            String expected = "Other";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region FormatErrorMessage(String name)

        [Fact]
        public void FormatErrorMessage_ForProperty()
        {
            attribute.OtherPropertyDisplayName = "Other";

            String actual = attribute.FormatErrorMessage("EqualTo");
            String expected = String.Format(Validations.EqualTo, "EqualTo", attribute.OtherPropertyDisplayName);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetValidationResult(Object value, ValidationContext context)

        [Fact]
        public void GetValidationResult_EqualValue()
        {
            ValidationContext context = new ValidationContext(new AdaptersModel { EqualTo = "Test" });

            Assert.Null(attribute.GetValidationResult("Test", context));
        }

        [Fact]
        public void GetValidationResult_Property_Error()
        {
            ValidationContext context = new ValidationContext(new AdaptersModel());

            String actual = attribute.GetValidationResult("Test", context).ErrorMessage;
            String expected = String.Format(Validations.EqualTo, context.DisplayName, "EqualTo");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidationResult_NoProperty_Error()
        {
            attribute = new EqualToAttribute("Temp");
            ValidationContext context = new ValidationContext(new AdaptersModel());

            String actual = attribute.GetValidationResult("Test", context).ErrorMessage;
            String expected = String.Format(Validations.EqualTo, context.DisplayName, "Temp");

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
