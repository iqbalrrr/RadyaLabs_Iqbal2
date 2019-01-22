using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class MinValueAdapterTests
    {
        #region GetClientValidationRules()

        [Fact]
        public void GetClientValidationRules_ReturnsMinRangeValidationRule()
        {
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(AdaptersModel), "MinValue");
            MinValueAdapter adapter = new MinValueAdapter(metadata, new ControllerContext(), new MinValueAttribute(128));

            String expectedMessage = new MinValueAttribute(128).FormatErrorMessage(metadata.GetDisplayName());
            ModelClientValidationRule actual = adapter.GetClientValidationRules().Single();

            Assert.Equal(128M, actual.ValidationParameters["min"]);
            Assert.Equal(expectedMessage, actual.ErrorMessage);
            Assert.Equal("range", actual.ValidationType);
            Assert.Single(actual.ValidationParameters);
        }

        #endregion
    }
}
