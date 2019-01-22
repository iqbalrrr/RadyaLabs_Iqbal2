using RadyaLabs.Data.Core;
using RadyaLabs.Services;
using NSubstitute;
using System;
using Xunit;

namespace RadyaLabs.Tests.Unit.Services
{
    public class BaseServiceTests : IDisposable
    {
        private IUnitOfWork unitOfWork;
        private BaseService service;

        public BaseServiceTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            service = Substitute.ForPartsOf<BaseService>(unitOfWork);
        }
        public void Dispose()
        {
            service.Dispose();
        }

        #region Dispose()

        [Fact]
        public void Dispose_UnitOfWork()
        {
            service.Dispose();

            unitOfWork.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            service.Dispose();
            service.Dispose();
        }

        #endregion
    }
}
