using RadyaLabs.Data.Logging;
using RadyaLabs.Objects;
using RadyaLabs.Tests.Data;
using RadyaLabs.Tests.Objects;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using Xunit;

namespace RadyaLabs.Tests.Unit.Data.Logging
{
    public class LoggableEntityTests : IDisposable
    {
        private DbEntityEntry<BaseModel> entry;
        private TestingContext context;
        private TestModel model;

        public LoggableEntityTests()
        {
            using (context = new TestingContext())
            {
                context.Set<TestModel>().RemoveRange(context.Set<TestModel>());
                context.Set<TestModel>().Add(ObjectFactory.CreateTestModel());
                context.Set<Role>().Add(ObjectFactory.CreateRole());
                context.DropData();
            }

            context = new TestingContext();
            model = context.Set<TestModel>().Single();
            entry = context.Entry<BaseModel>(model);
        }
        public void Dispose()
        {
            context.Dispose();
        }

        #region LoggableEntity(DbEntityEntry<BaseModel> entry)

        [Fact]
        public void LoggableEntity_CreatesPropertiesForAddedEntity()
        {
            entry.State = EntityState.Added;

            AsssertProperties(entry.CurrentValues);
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForModifiedEntity()
        {
            String title = model.Title;
            entry.State = EntityState.Modified;
            entry.CurrentValues["Title"] = "Role";
            entry.OriginalValues["Title"] = "Role";

            LoggableProperty expected = new LoggableProperty(entry.Property("Title"), title);
            LoggableProperty actual = new LoggableEntity(entry).Properties.Single();

            Assert.Equal(expected.IsModified, actual.IsModified);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForAttachedEntity()
        {
            context.Dispose();
            String title = model.Title;
            context = new TestingContext();
            context.Set<TestModel>().Attach(model);

            entry = context.Entry<BaseModel>(model);
            entry.OriginalValues["Title"] = "Role";
            entry.CurrentValues["Title"] = "Role";
            entry.State = EntityState.Modified;

            LoggableProperty expected = new LoggableProperty(entry.Property("Title"), title);
            LoggableProperty actual = new LoggableEntity(entry).Properties.Single();

            Assert.Equal(expected.IsModified, actual.IsModified);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForDeletedEntity()
        {
            entry.State = EntityState.Deleted;

            AsssertProperties(entry.GetDatabaseValues());
        }

        [Fact]
        public void LoggableEntity_SetsAction()
        {
            entry.State = EntityState.Deleted;

            String actual = new LoggableEntity(entry).Action;
            String expected = entry.State.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsEntityTypeNameFromProxy()
        {
            DbEntityEntry<BaseModel> proxy = context.Entry<BaseModel>(context.Set<Role>().Single());

            String actual = new LoggableEntity(proxy).Name;
            String expected = typeof(Role).Name;

            Assert.Equal("System.Data.Entity.DynamicProxies", proxy.Entity.GetType().Namespace);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsEntityTypeName()
        {
            entry = context.Entry<BaseModel>(context.Set<TestModel>().Add(new TestModel()));

            String actual = new LoggableEntity(entry).Name;
            String expected = typeof(TestModel).Name;

            Assert.NotEqual("System.Data.Entity.DynamicProxies", entry.Entity.GetType().Namespace);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsEntityId()
        {
            Int32 actual = new LoggableEntity(entry).Id();
            Int32 expected = model.Id;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region ToString()

        [Fact]
        public void ToString_FormsEntityChanges()
        {
            StringBuilder changes = new StringBuilder();
            LoggableEntity loggableEntity = new LoggableEntity(entry);
            foreach (LoggableProperty property in loggableEntity.Properties)
                changes.AppendFormat("{0}{1}", property, Environment.NewLine);

            String actual = loggableEntity.ToString();
            String expected = changes.ToString();

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Test helpers

        private void AsssertProperties(DbPropertyValues newValues)
        {
            LoggableProperty[] actual = new LoggableEntity(entry).Properties.ToArray();
            LoggableProperty[] expected = newValues.PropertyNames.Where(property => property != "Id")
                .Select(name => new LoggableProperty(entry.Property(name), newValues[name])).ToArray();

            for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
            {
                Assert.Equal(expected[i].IsModified, actual[i].IsModified);
                Assert.Equal(expected[i].ToString(), actual[i].ToString());
            }
        }

        #endregion
    }
}
