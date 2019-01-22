using RadyaLabs.Components.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class DataTypeValidatorProviderTests
    {
        private DataTypeValidatorProvider provider;

        public DataTypeValidatorProviderTests()
        {
            provider = new DataTypeValidatorProvider();
        }

        #region GetValidators(ModelMetadata metadata, ControllerContext context)

        [Fact]
        public void GetValidators_ReturnsEmpty()
        {
            ModelMetadata metadata = new ModelMetadata(new DataAnnotationsModelMetadataProvider(), null, null, typeof(String), "Id");

            Assert.Empty(provider.GetValidators(metadata, new ControllerContext()));
        }

        [Fact]
        public void GetValidators_ReturnsDateValidator()
        {
            ModelMetadata metadata = new ModelMetadata(new DataAnnotationsModelMetadataProvider(), null, null, typeof(DateTime), "Date");

            IEnumerable<Type> actual = provider.GetValidators(metadata, new ControllerContext()).Select(validator => validator.GetType());
            IEnumerable<Type> expected = new[] { typeof(DateValidator) };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidators_ReturnsNumericValidator()
        {
            ModelMetadata metadata = new ModelMetadata(new DataAnnotationsModelMetadataProvider(), null, null, typeof(Decimal), "Test");

            IEnumerable<Type> actual = provider.GetValidators(metadata, new ControllerContext()).Select(validator => validator.GetType());
            IEnumerable<Type> expected = new[] { typeof(NumberValidator) };

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
