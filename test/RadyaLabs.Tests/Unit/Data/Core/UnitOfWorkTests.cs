using AutoMapper;
using RadyaLabs.Data.Core;
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
using Xunit;

namespace RadyaLabs.Tests.Unit.Data.Core
{
    public class UnitOfWorkTests : IDisposable
    {
        private TestingContext context;
        private UnitOfWork unitOfWork;
        private IAuditLogger logger;
        private TestModel model;

        public UnitOfWorkTests()
        {
            context = new TestingContext();
            model = ObjectFactory.CreateTestModel();
            logger = Substitute.For<IAuditLogger>();
            unitOfWork = new UnitOfWork(context, logger);

            context.Set<TestModel>().RemoveRange(context.Set<TestModel>());
            context.DropData();
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
            context.Dispose();
        }

        #region GetAs<TModel, TDestination>(Int32? id)

        [Fact]
        public void GetAs_Null_ReturnsDestinationDefault()
        {
            Assert.Null(unitOfWork.GetAs<TestModel, TestView>(null));
        }

        [Fact]
        public void GetAs_ReturnsModelAsDestinationModelById()
        {
            context.Set<TestModel>().Add(model);
            context.SaveChanges();

            TestView expected = Mapper.Map<TestView>(model);
            TestView actual = unitOfWork.GetAs<TestModel, TestView>(model.Id);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        #endregion

        #region Get<TModel>(Int32? id)

        [Fact]
        public void Get_Null_ReturnsNull()
        {
            Assert.Null(unitOfWork.Get<TestModel>(null));
        }

        [Fact]
        public void Get_ModelById()
        {
            context.Set<TestModel>().Add(model);
            context.SaveChanges();

            TestModel expected = context.Set<TestModel>().AsNoTracking().Single();
            TestModel actual = unitOfWork.Get<TestModel>(model.Id);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Get_NotFound_ReturnsNull()
        {
            Assert.Null(unitOfWork.Get<TestModel>(0));
        }

        #endregion

        #region To<TDestination>(Object source)

        [Fact]
        public void To_ConvertsSourceToDestination()
        {
            TestView actual = unitOfWork.To<TestView>(model);
            TestView expected = Mapper.Map<TestView>(model);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        #endregion

        #region Select<TModel>()

        [Fact]
        public void Select_FromSet()
        {
            context.Set<TestModel>().Add(model);
            context.SaveChanges();

            IEnumerable<TestModel> actual = unitOfWork.Select<TestModel>();
            IEnumerable<TestModel> expected = context.Set<TestModel>();

            Assert.Equal(expected, actual);
        }

        #endregion

        #region InsertRange<TModel>(IEnumerable<TModel> models)

        [Fact]
        public void InsertRange_AddsModelsToDbSet()
        {
            TestModel[] models = { ObjectFactory.CreateTestModel(1), ObjectFactory.CreateTestModel(2) };
            DbSet<TestModel> set = Substitute.For<DbSet<TestModel>, IQueryable>();
            TestingContext testingContext = Substitute.For<TestingContext>();
            testingContext.Set<TestModel>().Returns(set);
            unitOfWork.Dispose();

            unitOfWork = new UnitOfWork(testingContext);
            unitOfWork.InsertRange(models);

            set.Received().AddRange(models);
        }

        #endregion

        #region Insert<TModel>(TModel model)

        [Fact]
        public void Insert_AddsModelToDbSet()
        {
            unitOfWork.Insert(model);

            Object actual = context.ChangeTracker.Entries<TestModel>().Single().Entity;
            Object expected = model;

            Assert.Equal(EntityState.Added, context.Entry(model).State);
            Assert.Same(expected, actual);
        }

        #endregion

        #region Update<TModel>(TModel model)

        [Theory]
        [InlineData(EntityState.Added, EntityState.Modified)]
        [InlineData(EntityState.Deleted, EntityState.Modified)]
        [InlineData(EntityState.Detached, EntityState.Modified)]
        [InlineData(EntityState.Modified, EntityState.Modified)]
        [InlineData(EntityState.Unchanged, EntityState.Unchanged)]
        public void Update_Entry(EntityState initialState, EntityState state)
        {
            DbEntityEntry<TestModel> entry = context.Entry(model);
            entry.State = initialState;

            unitOfWork.Update(model);

            DbEntityEntry<TestModel> actual = entry;

            Assert.Equal(state, actual.State);
            Assert.False(actual.Property(prop => prop.CreationDate).IsModified);
        }

        #endregion

        #region DeleteRange<TModel>(IEnumerable<TModel> models)

        [Fact]
        public void DeleteRange_Models()
        {
            IEnumerable<TestModel> models = new[] { ObjectFactory.CreateTestModel(1), ObjectFactory.CreateTestModel(2) };

            context.Set<TestModel>().AddRange(models);
            context.SaveChanges();

            unitOfWork.DeleteRange(models);
            unitOfWork.Commit();

            Assert.Empty(context.Set<TestModel>());
        }

        #endregion

        #region Delete<TModel>(TModel model)

        [Fact]
        public void Delete_Model()
        {
            context.Set<TestModel>().Add(model);
            context.SaveChanges();

            unitOfWork.Delete(model);
            unitOfWork.Commit();

            Assert.Empty(context.Set<TestModel>());
        }

        #endregion

        #region Delete<TModel>(Int32 id)

        [Fact]
        public void Delete_ModelById()
        {
            context.Set<TestModel>().Add(model);
            context.SaveChanges();

            unitOfWork.Delete<TestModel>(model.Id);
            unitOfWork.Commit();

            Assert.Empty(context.Set<TestModel>());
        }

        #endregion

        #region Commit()

        [Fact]
        public void Commit_SavesChanges()
        {
            TestingContext testingContext = Substitute.For<TestingContext>();

            new UnitOfWork(testingContext).Commit();

            testingContext.Received().SaveChanges();
        }

        [Fact]
        public void Commit_Logs()
        {
            unitOfWork.Commit();

            logger.Received().Log(Arg.Any<IEnumerable<DbEntityEntry<BaseModel>>>());
            logger.Received().Save();
        }

        [Fact]
        public void Commit_Failed_DoesNotSaveLogs()
        {
            unitOfWork.Insert(new TestModel { Title = new String('X', 513) });
            Exception exception = Record.Exception(() => unitOfWork.Commit());

            logger.Received().Log(Arg.Any<IEnumerable<DbEntityEntry<BaseModel>>>());
            logger.DidNotReceive().Save();
            Assert.NotNull(exception);
        }

        #endregion

        #region Dispose()

        [Fact]
        public void Dispose_Logger()
        {
            unitOfWork.Dispose();

            logger.Received().Dispose();
        }

        [Fact]
        public void Dispose_Context()
        {
            TestingContext testingContext = Substitute.For<TestingContext>();

            new UnitOfWork(testingContext).Dispose();

            testingContext.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            unitOfWork.Dispose();
            unitOfWork.Dispose();
        }

        #endregion
    }
}
