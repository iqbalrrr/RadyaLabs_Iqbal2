using RadyaLabs.Data.Logging;
using RadyaLabs.Objects;
using RadyaLabs.Tests.Data;
using RadyaLabs.Tests.Objects;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Xunit;

namespace RadyaLabs.Tests.Unit.Data.Logging
{
    public class AuditLoggerTests : IDisposable
    {
        private DbEntityEntry<BaseModel> entry;
        private TestingContext context;
        private AuditLogger logger;

        public AuditLoggerTests()
        {
            context = new TestingContext();
            logger = new AuditLogger(context, 1);
            TestModel model = ObjectFactory.CreateTestModel();
            TestingContext dataContext = new TestingContext();

            entry = dataContext.Entry<BaseModel>(dataContext.Set<TestModel>().Add(model));
            dataContext.Set<AuditLog>().RemoveRange(dataContext.Set<AuditLog>());
            dataContext.DropData();
        }
        public void Dispose()
        {
            HttpContext.Current = null;
            context.Dispose();
            logger.Dispose();
        }

        #region AuditLogger(DbContext context)

        [Fact]
        public void AuditLogger_DisablesChangesDetection()
        {
            TestingContext testingContext = new TestingContext();
            testingContext.Configuration.AutoDetectChangesEnabled = true;

            using (new AuditLogger(testingContext))
                Assert.False(testingContext.Configuration.AutoDetectChangesEnabled);
        }

        #endregion

        #region AuditLogger(DbContext context, Int32? accountId)

        [Fact]
        public void AuditLogger_AccountId_DisablesChangesDetection()
        {
            TestingContext testingContext = new TestingContext();
            testingContext.Configuration.AutoDetectChangesEnabled = true;

            using (new AuditLogger(testingContext, 1))
                Assert.False(testingContext.Configuration.AutoDetectChangesEnabled);
        }

        #endregion

        #region Log(IEnumerable<DbEntityEntry<BaseModel>> entries)

        [Fact]
        public void Log_Added()
        {
            entry.State = EntityState.Added;

            logger.Log(new[] { entry });
            logger.Save();

            LoggableEntity expected = new LoggableEntity(entry);
            AuditLog actual = context.Set<AuditLog>().Single();

            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
            Assert.Equal(1, actual.AccountId);
        }

        [Fact]
        public void Log_Modified()
        {
            (entry.Entity as TestModel).Title += "Test";
            entry.State = EntityState.Modified;

            logger.Log(new[] { entry });
            logger.Save();

            LoggableEntity expected = new LoggableEntity(entry);
            AuditLog actual = context.Set<AuditLog>().Single();

            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
            Assert.Equal(1, actual.AccountId);
        }

        [Fact]
        public void Log_NoChanges_DoesNotLog()
        {
            entry.State = EntityState.Modified;

            logger.Log(new[] { entry });
            logger.Save();

            Assert.Empty(context.Set<AuditLog>());
        }

        [Fact]
        public void Log_Deleted()
        {
            entry.State = EntityState.Deleted;

            logger.Log(new[] { entry });
            logger.Save();

            LoggableEntity expected = new LoggableEntity(entry);
            AuditLog actual = context.Set<AuditLog>().Single();

            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
            Assert.Equal(1, actual.AccountId);
        }

        [Fact]
        public void Log_UnsupportedState_DoesNotLog()
        {
            IEnumerable<EntityState> unsupportedStates = Enum
                .GetValues(typeof(EntityState))
                .Cast<EntityState>()
                .Where(state =>
                    state != EntityState.Added &&
                    state != EntityState.Modified &&
                    state != EntityState.Deleted);

            foreach (EntityState usupportedState in unsupportedStates)
            {
                entry.State = usupportedState;
                logger.Log(new[] { entry });
            }

            Assert.Empty(context.ChangeTracker.Entries<AuditLog>());
        }

        [Fact]
        public void Log_DoesNotSaveChanges()
        {
            entry.State = EntityState.Added;

            logger.Log(new[] { entry });

            Assert.Empty(context.Set<AuditLog>());
        }

        #endregion

        #region Log(LoggableEntity entity)

        [Fact]
        public void Log_Entity()
        {
            LoggableEntity entity = new LoggableEntity(entry);

            logger.Log(entity);
            logger.Save();

            AuditLog actual = context.Set<AuditLog>().Single();
            LoggableEntity expected = entity;

            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
            Assert.Equal(1, actual.AccountId);
        }

        [Fact]
        public void Log_DoesNotSave()
        {
            entry.State = EntityState.Added;

            logger.Log(new LoggableEntity(entry));

            Assert.Empty(context.Set<AuditLog>());
        }

        #endregion

        #region Save()

        [Theory]
        [InlineData(1, "", 1)]
        [InlineData(1, "2", 1)]
        [InlineData(1, null, 1)]
        [InlineData(null, "2", 2)]
        [InlineData(null, "", null)]
        [InlineData(null, null, null)]
        public void Save_LogsOnce(Int32? accountId, String identity, Int32? expectedAccountId)
        {
            HttpContext.Current = HttpContextFactory.CreateHttpContext();
            HttpContext.Current.User.Identity.Name.Returns(identity);
            LoggableEntity entity = new LoggableEntity(entry);
            logger = new AuditLogger(context, accountId);

            logger.Log(entity);
            logger.Save();
            logger.Save();

            AuditLog actual = context.Set<AuditLog>().Single();
            LoggableEntity expected = entity;

            Assert.Equal(expectedAccountId, actual.AccountId);
            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
        }

        #endregion

        #region Dispose()

        [Fact]
        public void Dispose_Context()
        {
            TestingContext testingContext = Substitute.For<TestingContext>();

            new AuditLogger(testingContext).Dispose();

            testingContext.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            logger.Dispose();
            logger.Dispose();
        }

        #endregion
    }
}
