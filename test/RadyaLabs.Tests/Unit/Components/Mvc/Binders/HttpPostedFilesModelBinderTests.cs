using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using NSubstitute;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class HttpPostedFilesModelBinderTests
    {
        private HttpPostedFilesModelBinder binder;
        private NameValueCollection collection;
        private ModelBindingContext binding;
        private ControllerContext context;

        public HttpPostedFilesModelBinderTests()
        {
            context = new ControllerContext();
            binding = new ModelBindingContext();
            collection = new NameValueCollection();
            binder = new HttpPostedFilesModelBinder();
            binding.ModelName = "Files";

            binding.ModelMetadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AllTypesView), binding.ModelName);
        }

        #region Method: BindModel(ControllerContext context, ModelBindingContext binding)

        [Fact]
        public void BindModel_NoValueProvider_ReturnsNull()
        {
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, null);

            Assert.Null(binder.BindModel(context, binding));
        }

        [Fact]
        public void BindModel_NullValue_ReturnsNull()
        {
            collection.Add(binding.ModelName, null);
            binding.ValueProvider = new NameValueCollectionValueProvider(collection, null);

            Assert.Null(binder.BindModel(context, binding));
        }

        [Fact]
        public void BindModel_RemovesNullFiles()
        {
            HttpPostedFileBase file = Substitute.For<HttpPostedFileBase>();
            binding.ValueProvider = Substitute.For<IValueProvider>();
            IList<HttpPostedFileBase> files = new List<HttpPostedFileBase> { null, file, null };
            binding.ValueProvider.GetValue(binding.ModelName).Returns(new ValueProviderResult(files, "", null));

            IEnumerable<HttpPostedFileBase> actual = binder.BindModel(context, binding) as IList<HttpPostedFileBase>;
            IEnumerable<HttpPostedFileBase> expected = new[] { file };

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
