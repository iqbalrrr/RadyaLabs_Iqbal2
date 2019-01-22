using RadyaLabs.Components.Alerts;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RadyaLabs.Tests.Unit.Components.Alerts
{
    public class AlertsContainerTests
    {
        private AlertsContainer container;

        public AlertsContainerTests()
        {
            container = new AlertsContainer();
        }

        #region Merge(AlertsContainer alerts)

        [Fact]
        public void Merge_DoesNotMergeItself()
        {
            container.Add(new Alert());
            IEnumerable<Alert> alerts = container.ToArray();

            container.Merge(container);

            IEnumerable<Alert> actual = container;
            IEnumerable<Alert> expected = alerts;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Merge_Alerts()
        {
            AlertsContainer part = new AlertsContainer();
            container.AddError("FirstError");
            part.AddError("SecondError");

            IEnumerable<Alert> expected = container.Union(part);
            IEnumerable<Alert> actual = container;
            container.Merge(part);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AddInfo(String message, Int32 timeout = 0)

        [Fact]
        public void AddInfo_Message()
        {
            container.AddInfo("Message", 1);

            Alert actual = container.Single();

            Assert.Equal(AlertType.Info, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion

        #region AddError(String message, Int32 timeout = 0)

        [Fact]
        public void AddError_Message()
        {
            container.AddError("Message", 1);

            Alert actual = container.Single();

            Assert.Equal(AlertType.Danger, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion

        #region AddSuccess(String message, Int32 timeout = 0)

        [Fact]
        public void AddSuccess_Message()
        {
            container.AddSuccess("Message", 1);

            Alert actual = container.Single();

            Assert.Equal(AlertType.Success, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion

        #region AddWarning(String message, Int32 timeout = 0)

        [Fact]
        public void AddWarning_Message()
        {
            container.AddWarning("Message", 1);

            Alert actual = container.Single();

            Assert.Equal(AlertType.Warning, actual.Type);
            Assert.Equal("Message", actual.Message);
            Assert.Equal(1, actual.Timeout);
            Assert.Null(actual.Id);
        }

        #endregion
    }
}
