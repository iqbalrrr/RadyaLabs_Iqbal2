using Datalist;
using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using NSubstitute;
using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class TrimmingModelBinderTests
    {
        private NameValueCollection collection;
        private ModelBindingContext binding;
        private TrimmingModelBinder binder;
        private ControllerContext context;

        public TrimmingModelBinderTests()
        {
            context = new ControllerContext();
            binder = new TrimmingModelBinder();
            binding = new ModelBindingContext();
            collection = new NameValueCollection();

            binding.ModelName = "StringField";
            context.Controller = Substitute.For<ControllerBase>();
            binding.ModelMetadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AllTypesView), "StringField");
        }

        #region BindModel(ControllerContext context, ModelBindingContext binding)

        [Fact]
        public void BindModel_DatalistFilter_ReturnsNull()
        {
            binding.ModelName = "Search";
            collection.Add(binding.ModelName, "  Trimmed text  ");
            binding.ModelMetadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(DatalistFilter), "Search");
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, null);

            Object actual = binder.BindModel(context, binding);
            Object expected = "  Trimmed text  ";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BindModel_NullValue_ReturnsNull()
        {
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, null);

            Assert.Null(binder.BindModel(context, binding));
        }

        [Fact]
        public void BindModel_DoesNotTrimValue()
        {
            binding.ModelName = "NotTrimmedStringField";
            collection.Add(binding.ModelName, "  Trimmed text  ");
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, null);
            binding.ModelMetadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AllTypesView), "NotTrimmedStringField");

            Object actual = binder.BindModel(context, binding);
            Object expected = "  Trimmed text  ";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BindModel_TrimsValue()
        {
            collection.Add(binding.ModelName, "  Trimmed text  ");
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, null);

            Object actual = binder.BindModel(context, binding);
            Object expected = "Trimmed text";

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
