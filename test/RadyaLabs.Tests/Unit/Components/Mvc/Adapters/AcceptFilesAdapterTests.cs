using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class AcceptFilesAdapterTests
    {
        #region GetClientValidationRules()

        [Fact]
        public void GetClientValidationRules_ReturnsAcceptFilesValidationRule()
        {
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(AdaptersModel), "AcceptFiles");
            AcceptFilesAdapter adapter = new AcceptFilesAdapter(metadata, new ControllerContext(), new AcceptFilesAttribute(".docx,.rtf"));

            String expectedMessage = new AcceptFilesAttribute(".docx,.rtf").FormatErrorMessage(metadata.GetDisplayName());
            ModelClientValidationRule actual = adapter.GetClientValidationRules().Single();

            Assert.Equal(".docx,.rtf", actual.ValidationParameters["extensions"]);
            Assert.Equal("acceptfiles", actual.ValidationType);
            Assert.Equal(expectedMessage, actual.ErrorMessage);
            Assert.Single(actual.ValidationParameters);
        }

        #endregion
    }
}
