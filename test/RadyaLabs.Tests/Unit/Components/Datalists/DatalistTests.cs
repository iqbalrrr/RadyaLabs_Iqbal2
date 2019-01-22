using RadyaLabs.Components.Datalists;
using RadyaLabs.Data.Core;
using RadyaLabs.Objects;
using RadyaLabs.Resources;
using RadyaLabs.Tests.Data;
using RadyaLabs.Tests.Objects;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Datalists
{
    public class MvcDatalistTests : IDisposable
    {
        private MvcDatalist<Role, RoleView> datalist;
        private UrlHelper url;

        public MvcDatalistTests()
        {
            HttpContext.Current = HttpContextFactory.CreateHttpContext();
            url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            datalist = new MvcDatalist<Role, RoleView>(url);
            using (TestingContext context = new TestingContext())
                context.DropData();
        }
        public void Dispose()
        {
            HttpContext.Current = null;
        }

        #region Datalist(UrlHelper url)

        [Fact]
        public void Datalist_SetsDialogTitle()
        {
            datalist = new MvcDatalist<Role, RoleView>(url);

            String expected = ResourceProvider.GetDatalistTitle(typeof(RoleView).Name.Replace("View", ""));
            String actual = datalist.Title;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Datalist_SetsDatalistUrl()
        {
            datalist = new MvcDatalist<Role, RoleView>(url);

            String expected = url.Action(typeof(Role).Name, "Datalist", new { area = "" });
            String actual = datalist.Url;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetColumnHeader(PropertyInfo property)

        [Fact]
        public void GetColumnHeader_ReturnsPropertyTitle()
        {
            String actual = datalist.GetColumnHeader(typeof(RoleView).GetProperty("Title"));
            String expected = ResourceProvider.GetPropertyTitle(typeof(RoleView), "Title");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetColumnHeader_ReturnsRelationPropertyTitle()
        {
            PropertyInfo property = typeof(AllTypesView).GetProperty("Child");

            String actual = datalist.GetColumnHeader(property);

            Assert.Empty(actual);
        }

        #endregion

        #region GetColumnCssClass(PropertyInfo property)

        [Theory]
        [InlineData("EnumField", "text-left")]
        [InlineData("SByteField", "text-right")]
        [InlineData("ByteField", "text-right")]
        [InlineData("Int16Field", "text-right")]
        [InlineData("UInt16Field", "text-right")]
        [InlineData("Int32Field", "text-right")]
        [InlineData("UInt32Field", "text-right")]
        [InlineData("Int64Field", "text-right")]
        [InlineData("UInt64Field", "text-right")]
        [InlineData("SingleField", "text-right")]
        [InlineData("DoubleField", "text-right")]
        [InlineData("DecimalField", "text-right")]
        [InlineData("BooleanField", "text-center")]
        [InlineData("DateTimeField", "text-center")]

        [InlineData("NullableEnumField", "text-left")]
        [InlineData("NullableSByteField", "text-right")]
        [InlineData("NullableByteField", "text-right")]
        [InlineData("NullableInt16Field", "text-right")]
        [InlineData("NullableUInt16Field", "text-right")]
        [InlineData("NullableInt32Field", "text-right")]
        [InlineData("NullableUInt32Field", "text-right")]
        [InlineData("NullableInt64Field", "text-right")]
        [InlineData("NullableUInt64Field", "text-right")]
        [InlineData("NullableSingleField", "text-right")]
        [InlineData("NullableDoubleField", "text-right")]
        [InlineData("NullableDecimalField", "text-right")]
        [InlineData("NullableBooleanField", "text-center")]
        [InlineData("NullableDateTimeField", "text-center")]

        [InlineData("StringField", "text-left")]
        [InlineData("Child", "text-left")]
        public void GetColumnCssClass_ReturnsCssClassForPropertyType(String propertyName, String cssClass)
        {
            PropertyInfo property = typeof(AllTypesView).GetProperty(propertyName);

            String actual = datalist.GetColumnCssClass(property);
            String expected = cssClass;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetModels()

        [Fact]
        public void GetModels_FromUnitOfWork()
        {
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.Select<Role>().To<RoleView>().Returns(new RoleView[0].AsQueryable());

            Object actual = new MvcDatalist<Role, RoleView>(unitOfWork).GetModels();
            Object expected = unitOfWork.Select<Role>().To<RoleView>();

            Assert.Same(expected, actual);
        }

        #endregion
    }
}
