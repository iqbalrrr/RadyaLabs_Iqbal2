using RadyaLabs.Components.Mvc;
using RadyaLabs.Tests.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Mvc
{
    public class ModelStateDictionaryExtensionsTests
    {
        private ModelStateDictionary modelState;

        public ModelStateDictionaryExtensionsTests()
        {
            modelState = new ModelStateDictionary();
        }

        #region Errors(this ModelStateDictionary modelState)

        [Fact]
        public void Errors_FromModelState()
        {
            modelState.AddModelError("Empty", "");
            modelState.AddModelError("Error", "Error");
            modelState.AddModelError("EmptyErrors", "");
            modelState.AddModelError("EmptyErrors", "E");
            modelState.Add("NoErrors", new ModelState());
            modelState.AddModelError("TwoErrors", "Error1");
            modelState.AddModelError("TwoErrors", "Error2");
            modelState.AddModelError("NullError", (String)null);
            modelState.AddModelError("NullErrors", (String)null);
            modelState.AddModelError("NullErrors", "NotNullError");
            modelState.AddModelError("WhitespaceErrors", "       ");
            modelState.AddModelError("WhitespaceErrors", "Whitespace");

            Dictionary<String, String> actual = modelState.Errors();

            Assert.Equal("       ", actual["WhitespaceErrors"]);
            Assert.Equal("NotNullError", actual["NullErrors"]);
            Assert.Equal("Error1", actual["TwoErrors"]);
            Assert.Equal("E", actual["EmptyErrors"]);
            Assert.Equal("Error", actual["Error"]);
            Assert.Null(actual["NullError"]);
            Assert.Equal(7, actual.Count);
            Assert.Null(actual["Empty"]);
        }

        #endregion

        #region AddModelError<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, Object>> expression, String message)

        [Fact]
        public void AddModelError_Key()
        {
            modelState.AddModelError<AllTypesView>(model => model.Child.NullableByteField, "Test error");

            String expected = "Child.NullableByteField";
            String actual = modelState.Single().Key;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddModelError_Message()
        {
            modelState.AddModelError<AllTypesView>(model => model.Child.NullableByteField, "Test error");

            String actual = modelState.Single().Value.Errors.Single().ErrorMessage;
            String expected = "Test error";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AddModelError<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, Object>> expression, String format, Object[] args)

        [Fact]
        public void AddModelError_FormattedKey()
        {
            modelState.AddModelError<AllTypesView>(model => model.Int32Field, "Test {0}", "error");

            String actual = modelState.Single().Key;
            String expected = "Int32Field";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddModelError_FormattedMessage()
        {
            modelState.AddModelError<AllTypesView>(model => model.Int32Field, "Test {0}", "error");

            String actual = modelState.Single().Value.Errors.Single().ErrorMessage;
            String expected = "Test error";

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
