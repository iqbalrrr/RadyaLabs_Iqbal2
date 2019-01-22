using RadyaLabs.Components.Mvc;
using RadyaLabs.Resources.Form;
using RadyaLabs.Tests.Objects;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class StringLengthAdapterTests
    {
        #region StringLengthAdapter(ModelMetadata metadata, ControllerContext context, StringLengthAttribute attribute)

        [Fact]
        public void StringLengthAdapter_SetsExceededErrorMessage()
        {
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AdaptersModel), "StringLength");
            StringLengthAttribute attribute = new StringLengthAttribute(128);

            new StringLengthAdapter(metadata, new ControllerContext(), attribute);

            String expected = Validations.StringLength;
            String actual = attribute.ErrorMessage;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringLengthAdapter_SetsRangeErrorMessage()
        {
            StringLengthAttribute attribute = new StringLengthAttribute(128) { MinimumLength = 4 };
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AdaptersModel), "StringLength");

            new StringLengthAdapter(metadata, new ControllerContext(), attribute);

            String expected = Validations.StringLengthRange;
            String actual = attribute.ErrorMessage;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
