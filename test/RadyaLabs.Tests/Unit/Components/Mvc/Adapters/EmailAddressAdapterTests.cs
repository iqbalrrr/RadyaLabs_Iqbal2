using RadyaLabs.Components.Mvc;
using RadyaLabs.Resources.Form;
using RadyaLabs.Tests.Objects;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class EmailAddressAdapterTests
    {
        private EmailAddressAttribute attribute;
        private EmailAddressAdapter adapter;
        private ModelMetadata metadata;

        public EmailAddressAdapterTests()
        {
            attribute = new EmailAddressAttribute();
            metadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AdaptersModel), "EmailAddress");
            adapter = new EmailAddressAdapter(metadata, new ControllerContext(), attribute);
        }

        #region EmailAddressAdapter(ModelMetadata metadata, ControllerContext context, EmailAddressAttribute attribute)

        [Fact]
        public void EmailAddressAdapter_SetsErrorMessage()
        {
            String actual = attribute.ErrorMessage;
            String expected = Validations.Email;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetClientValidationRules()

        [Fact]
        public void GetClientValidationRules_ReturnsEmailValidationRule()
        {
            String expectedMessage = String.Format(Validations.Email, metadata.GetDisplayName());
            ModelClientValidationRule actual = adapter.GetClientValidationRules().Single();

            Assert.Equal(expectedMessage, actual.ErrorMessage);
            Assert.Equal("email", actual.ValidationType);
            Assert.Empty(actual.ValidationParameters);
        }

        #endregion
    }
}
