using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class FileSizeAdapterTests
    {
        #region GetClientValidationRules()

        [Fact]
        public void GetClientValidationRules_ReturnsFileSizeValidationRule()
        {
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(AdaptersModel), "FileSize");
            FileSizeAdapter adapter = new FileSizeAdapter(metadata, new ControllerContext(), new FileSizeAttribute(12.25));
            String errorMessage = new FileSizeAttribute(12.25).FormatErrorMessage(metadata.GetDisplayName());

            ModelClientValidationRule actual = adapter.GetClientValidationRules().Single();

            Assert.Equal(12845056.00M, actual.ValidationParameters["max"]);
            Assert.Equal(1, actual.ValidationParameters.Count);
            Assert.Equal("filesize", actual.ValidationType);
            Assert.Equal(errorMessage, actual.ErrorMessage);
        }

        #endregion
    }
}
