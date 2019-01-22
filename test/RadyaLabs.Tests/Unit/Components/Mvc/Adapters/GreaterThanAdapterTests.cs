using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class GreaterThanAdapterTests
    {
        #region GetClientValidationRules()

        [Fact]
        public void GetClientValidationRules_ReturnsGreaterValidationRule()
        {
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(AdaptersModel), "GreaterThan");
            GreaterThanAdapter adapter = new GreaterThanAdapter(metadata, new ControllerContext(), new GreaterThanAttribute(128));

            String expectedMessage = new GreaterThanAttribute(128).FormatErrorMessage(metadata.GetDisplayName());
            ModelClientValidationRule actual = adapter.GetClientValidationRules().Single();

            Assert.Equal(128M, actual.ValidationParameters["min"]);
            Assert.Equal(expectedMessage, actual.ErrorMessage);
            Assert.Equal("greater", actual.ValidationType);
            Assert.Single(actual.ValidationParameters);
        }

        #endregion
    }
}
