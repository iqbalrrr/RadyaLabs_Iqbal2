using RadyaLabs.Components.Mvc;
using RadyaLabs.Resources.Form;
using NSubstitute;
using System;
using System.Web;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class FileSizeAttributeTests
    {
        private FileSizeAttribute attribute;

        public FileSizeAttributeTests()
        {
            attribute = new FileSizeAttribute(12.25);
        }

        #region FileSizeAttribute(Double maximumMB)

        [Fact]
        public void FileSizeAttribute_SetsMaximumMB()
        {
            Decimal actual = new FileSizeAttribute(12.25).MaximumMB;
            Decimal expected = 12.25M;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region FormatErrorMessage(String name)

        [Fact]
        public void FormatErrorMessage_ForName()
        {
            attribute = new FileSizeAttribute(12.25);

            String expected = String.Format(Validations.FileSize, "File", attribute.MaximumMB);
            String actual = attribute.FormatErrorMessage("File");

            Assert.Equal(expected, actual);
        }

        #endregion

        #region IsValid(Object value)

        [Fact]
        public void IsValid_Null()
        {
            Assert.True(attribute.IsValid(null));
        }

        [Fact]
        public void IsValid_NotHttpPostedFileBaseValueIsValid()
        {
            Assert.True(attribute.IsValid("100"));
        }

        [Theory]
        [InlineData(240546)]
        [InlineData(12845056)]
        public void IsValid_LowerOrEqualFileSize(Int32 size)
        {
            HttpPostedFileBase file = Substitute.For<HttpPostedFileBase>();
            file.ContentLength.Returns(size);

            Assert.True(attribute.IsValid(file));
        }

        [Fact]
        public void IsValid_GreaterThanMaximumIsNotValid()
        {
            HttpPostedFileBase file = Substitute.For<HttpPostedFileBase>();
            file.ContentLength.Returns(12845057);

            Assert.False(attribute.IsValid(file));
        }

        [Theory]
        [InlineData(240546, 4574)]
        [InlineData(12840000, 5056)]
        public void IsValid_LowerOrEqualFileSizes(Int32 firstFileSize, Int32 secondFileSize)
        {
            HttpPostedFileBase[] files = { Substitute.For<HttpPostedFileBase>(), Substitute.For<HttpPostedFileBase>(), null };
            files[1].ContentLength.Returns(secondFileSize);
            files[0].ContentLength.Returns(firstFileSize);

            Assert.True(attribute.IsValid(files));
        }

        [Fact]
        public void IsValid_GreaterThanMaximumSizesAreNotValid()
        {
            HttpPostedFileBase[] files = { Substitute.For<HttpPostedFileBase>(), Substitute.For<HttpPostedFileBase>(), null };
            files[1].ContentLength.Returns(12840000);
            files[0].ContentLength.Returns(5057);

            Assert.False(attribute.IsValid(files));
        }

        #endregion
    }
}
