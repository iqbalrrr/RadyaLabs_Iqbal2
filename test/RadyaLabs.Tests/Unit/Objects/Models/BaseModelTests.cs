using RadyaLabs.Objects;
using NSubstitute;
using System;
using Xunit;

namespace RadyaLabs.Tests.Unit.Objects
{
    public class BaseModelTests
    {
        private BaseModel model;

        public BaseModelTests()
        {
            model = Substitute.For<BaseModel>();
        }

        #region CreationDate

        [Fact]
        public void CreationDate_ReturnsSameValue()
        {
            DateTime expected = model.CreationDate;
            DateTime actual = model.CreationDate;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
