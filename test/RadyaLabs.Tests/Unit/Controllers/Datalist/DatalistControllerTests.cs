using Datalist;
using RadyaLabs.Components.Datalists;
using RadyaLabs.Controllers;
using RadyaLabs.Data.Core;
using RadyaLabs.Objects;
using NSubstitute;
using System;
using System.Web;
using System.Web.Mvc;
using Xunit;

namespace RadyaLabs.Tests.Unit.Controllers
{
    public class DatalistControllerTests : IDisposable
    {
        private DatalistController controller;
        private IUnitOfWork unitOfWork;
        private DatalistFilter filter;
        private MvcDatalist datalist;

        public DatalistControllerTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            controller = Substitute.ForPartsOf<DatalistController>(unitOfWork);

            HttpContext.Current = HttpContextFactory.CreateHttpContext();
            datalist = Substitute.For<MvcDatalist>();
            filter = new DatalistFilter();
        }
        public void Dispose()
        {
            HttpContext.Current = null;
        }

        #region GetData(AbstractDatalist datalist, DatalistFilter filter)

        [Fact]
        public void GetData_SetsCurrentFilter()
        {
            controller.GetData(datalist, filter);

            DatalistFilter actual = datalist.Filter;
            DatalistFilter expected = filter;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetData_ReturnsPublicJson()
        {
            datalist.GetData().Returns(new DatalistData());

            JsonResult actual = controller.GetData(datalist, filter);
            Object expectedData = datalist.GetData();

            Assert.Equal(JsonRequestBehavior.AllowGet, actual.JsonRequestBehavior);
            Assert.Same(expectedData, actual.Data);
        }

        #endregion

        #region Role(DatalistFilter filter)

        [Fact]
        public void Role_ReturnsRolesData()
        {
            Object expected = GetData<MvcDatalist<Role, RoleView>>(controller);
            Object actual = controller.Role(filter);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Dispose()

        [Fact]
        public void Dispose_UnitOfWork()
        {
            controller.Dispose();

            unitOfWork.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            controller.Dispose();
            controller.Dispose();
        }

        #endregion

        #region Test helpers

        private JsonResult GetData<TDatalist>(DatalistController datalistController) where TDatalist : MvcDatalist
        {
            datalistController.When(sub => sub.GetData(Arg.Any<TDatalist>(), filter)).DoNotCallBase();
            datalistController.GetData(Arg.Any<TDatalist>(), filter).Returns(new JsonResult());

            return datalistController.GetData(null, filter);
        }

        #endregion
    }
}
