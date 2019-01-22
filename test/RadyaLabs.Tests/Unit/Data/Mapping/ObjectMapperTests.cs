using AutoMapper;
using RadyaLabs.Data.Mapping;
using RadyaLabs.Objects;
using Xunit;

namespace RadyaLabs.Tests.Unit.Data.Mapping
{
    public class ObjectMapperTests
    {
        static ObjectMapperTests()
        {
            ObjectMapper.MapObjects();
        }

        #region MapRoles()

        [Fact]
        public void MapRoles_Role_RoleView()
        {
            Role expected = ObjectFactory.CreateRole();
            RoleView actual = Mapper.Map<RoleView>(expected);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
            Assert.NotNull(actual.Permissions);
        }

        [Fact]
        public void MapRoles_RoleView_Role()
        {
            RoleView expected = ObjectFactory.CreateRoleView();
            Role actual = Mapper.Map<Role>(expected);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Empty(actual.Permissions);
        }

        #endregion
    }
}
