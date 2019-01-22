using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using NSubstitute;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class DateTimeModelBinderTests
    {
        private NameValueCollection collection;
        private ModelBindingContext binding;
        private DateTimeModelBinder binder;
        private ControllerContext context;

        public DateTimeModelBinderTests()
        {
            context = new ControllerContext();
            binder = new DateTimeModelBinder();
            binding = new ModelBindingContext();
            binding.ModelName = "DateTimeField";
            collection = new NameValueCollection();

            context.Controller = Substitute.For<ControllerBase>();
            binding.ModelMetadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AllTypesView), "DateTimeField");
        }

        #region BindModel(ControllerContext context, ModelBindingContext binding)

        [Fact]
        public void BindModel_NullValue_ReturnsNull()
        {
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, null);

            Assert.Null(binder.BindModel(context, binding));
        }

        [Fact]
        public void BindModel_DoesNotTruncateValue()
        {
            collection.Add(binding.ModelName, new DateTime(2017, 2, 3, 4, 5, 6).ToString());
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, CultureInfo.CurrentCulture);

            Object actual = binder.BindModel(context, binding);
            Object expected = new DateTime(2017, 2, 3, 4, 5, 6);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BindModel_TruncatesValue()
        {
            binding.ModelName = "TruncatedDateTimeField";
            collection.Add(binding.ModelName, new DateTime(2017, 2, 3, 4, 5, 6).ToString());
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, CultureInfo.CurrentCulture);
            binding.ModelMetadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AllTypesView), "TruncatedDateTimeField");

            Object actual = binder.BindModel(context, binding);
            Object expected = new DateTime(2017, 2, 3);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
