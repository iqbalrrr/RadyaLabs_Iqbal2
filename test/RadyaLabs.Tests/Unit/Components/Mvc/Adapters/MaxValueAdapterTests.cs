using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class MaxValueAdapterTests
    {
        #region GetClientValidationRules()

        [Fact]
        public void GetClientValidationRules_ReturnsMaxRangeValidationRule()
        {
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(AdaptersModel), "MaxValue");
            MaxValueAdapter adapter = new MaxValueAdapter(metadata, new ControllerContext(), new MaxValueAttribute(128));

            String expectedMessage = new MaxValueAttribute(128).FormatErrorMessage(metadata.GetDisplayName());
            ModelClientValidationRule actual = adapter.GetClientValidationRules().Single();

            Assert.Equal(128M, actual.ValidationParameters["max"]);
            Assert.Equal(expectedMessage, actual.ErrorMessage);
            Assert.Equal("range", actual.ValidationType);
            Assert.Single(actual.ValidationParameters);
        }

        #endregion
    }
}
