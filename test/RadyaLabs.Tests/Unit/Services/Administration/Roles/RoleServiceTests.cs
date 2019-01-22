using AutoMapper;
using AutoMapper.QueryableExtensions;
using RadyaLabs.Components.Extensions;
using RadyaLabs.Data.Core;
using RadyaLabs.Objects;
using RadyaLabs.Resources;
using RadyaLabs.Resources.Permission;
using RadyaLabs.Services;
using RadyaLabs.Tests.Data;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RadyaLabs.Tests.Unit.Services
{
    public class RoleServiceTests : IDisposable
    {
        private TestingContext context;
        private RoleService service;
        private Role role;

        public RoleServiceTests()
        {
            context = new TestingContext();
            service = Substitute.ForPartsOf<RoleService>(new UnitOfWork(context));

            context.DropData();
            SetUpData();
        }
        public void Dispose()
        {
            context.Dispose();
            service.Dispose();
        }

        #region SeedPermissions(RoleView view)

        [Fact]
        public void SeedPermissions_FirstDepth()
        {
            RoleView view = ObjectFactory.CreateRoleView();

            service.SeedPermissions(view);

            List<MvcTreeNode> expected = CreatePermissions().Nodes;
            List<MvcTreeNode> actual = view.Permissions.Nodes;

            for (Int32 i = 0; i < expected.Count || i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Children.Count, actual[i].Children.Count);
            }
        }

        [Fact]
        public void SeedPermissions_SecondDepth()
        {
            RoleView view = ObjectFactory.CreateRoleView();

            service.SeedPermissions(view);

            List<MvcTreeNode> expected = CreatePermissions().Nodes.SelectMany(node => node.Children).ToList();
            List<MvcTreeNode> actual = view.Permissions.Nodes.SelectMany(node => node.Children).ToList();

            for (Int32 i = 0; i < expected.Count || i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Children.Count, actual[i].Children.Count);
            }
        }

        [Fact]
        public void SeedPermissions_ThirdDepth()
        {
            RoleView view = ObjectFactory.CreateRoleView();

            service.SeedPermissions(view);

            List<MvcTreeNode> expected = CreatePermissions().Nodes.SelectMany(node => node.Children).SelectMany(node => node.Children).ToList();
            List<MvcTreeNode> actual = view.Permissions.Nodes.SelectMany(node => node.Children).SelectMany(node => node.Children).ToList();

            for (Int32 i = 0; i < expected.Count || i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Children.Count, actual[i].Children.Count);
            }
        }

        [Fact]
        public void SeedPermissions_BranchesWithoutId()
        {
            RoleView view = ObjectFactory.CreateRoleView();

            service.SeedPermissions(view);

            IEnumerable<MvcTreeNode> nodes = view.Permissions.Nodes;
            IEnumerable<MvcTreeNode> branches = GetBranchNodes(nodes);

            Assert.Empty(branches.Where(branch => branch.Id != null));
        }

        [Fact]
        public void SeedPermissions_LeafsWithId()
        {
            RoleView view = ObjectFactory.CreateRoleView();

            service.SeedPermissions(view);

            IEnumerable<MvcTreeNode> nodes = view.Permissions.Nodes;
            IEnumerable<MvcTreeNode> leafs = GetLeafNodes(nodes);

            Assert.Empty(leafs.Where(leaf => leaf.Id == null));
        }

        #endregion

        #region GetViews()

        [Fact]
        public void GetViews_ReturnsRoleViews()
        {
            RoleView[] actual = service.GetViews().ToArray();
            RoleView[] expected = context
                .Set<Role>()
                .ProjectTo<RoleView>()
                .OrderByDescending(view => view.Id)
                .ToArray();

            for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
            {
                Assert.Equal(expected[i].Permissions.SelectedIds, actual[i].Permissions.SelectedIds);
                Assert.Equal(expected[i].CreationDate, actual[i].CreationDate);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Id, actual[i].Id);
            }
        }

        #endregion

        #region GetView(Int32 id)

        [Fact]
        public void GetView_NoRole_ReturnsNull()
        {
            Assert.Null(service.GetView(0));
        }

        [Fact]
        public void GetView_ReturnsViewById()
        {
            service.When(sub => sub.SeedPermissions(Arg.Any<RoleView>())).DoNotCallBase();

            RoleView expected = Mapper.Map<RoleView>(role);
            RoleView actual = service.GetView(role.Id);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.NotEmpty(actual.Permissions.SelectedIds);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void GetView_SetsSelectedIds()
        {
            service.When(sub => sub.SeedPermissions(Arg.Any<RoleView>())).DoNotCallBase();

            IEnumerable<Int32> expected = role.Permissions.Select(rolePermission => rolePermission.PermissionId).OrderBy(id => id);
            IEnumerable<Int32> actual = service.GetView(role.Id).Permissions.SelectedIds.OrderBy(id => id);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetView_SeedsPermissions()
        {
            service.When(sub => sub.SeedPermissions(Arg.Any<RoleView>())).DoNotCallBase();

            RoleView view = service.GetView(role.Id);

            service.Received().SeedPermissions(view);
        }

        #endregion

        #region Create(RoleView view)

        [Fact]
        public void Create_Role()
        {
            RoleView view = ObjectFactory.CreateRoleView(1);

            service.Create(view);

            Role actual = context.Set<Role>().AsNoTracking().Single(model => model.Id != role.Id);
            RoleView expected = view;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
        }

        [Fact]
        public void Create_RolePermissions()
        {
            RoleView view = ObjectFactory.CreateRoleView(1);
            view.Permissions = CreatePermissions();

            service.Create(view);

            IEnumerable<Int32> expected = view.Permissions.SelectedIds.OrderBy(permissionId => permissionId);
            IEnumerable<Int32> actual = context
                .Set<RolePermission>()
                .AsNoTracking()
                .Where(rolePermission => rolePermission.RoleId != role.Id)
                .Select(rolePermission => rolePermission.PermissionId)
                .OrderBy(permissionId => permissionId);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Edit(RoleView view)

        [Fact]
        public void Edit_Role()
        {
            RoleView view = ObjectFactory.CreateRoleView(role.Id);
            view.Title = role.Title += "Test";

            service.Edit(view);

            Role actual = context.Set<Role>().AsNoTracking().Single();
            Role expected = role;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Edit_RolePermissions()
        {
            Permission permission = ObjectFactory.CreatePermission();
            context.Set<Permission>().Add(permission);
            context.SaveChanges();

            RoleView view = ObjectFactory.CreateRoleView(role.Id);
            view.Permissions = CreatePermissions();
            view.Permissions.SelectedIds.Add(permission.Id);
            //view.Permissions.SelectedIds.RemoveAt(0);

            service.Edit(view);

            IEnumerable<Int32> actual = context.Set<RolePermission>().AsNoTracking().Select(rolePermission => rolePermission.PermissionId).OrderBy(permissionId => permissionId);
            IEnumerable<Int32> expected = view.Permissions.SelectedIds.OrderBy(permissionId => permissionId);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Delete(Int32 id)

        [Fact]
        public void Delete_NullsAccountRoles()
        {
            Account account = ObjectFactory.CreateAccount();
            account.RoleId = role.Id;
            account.Role = null;

            context.Set<Account>().Add(account);
            context.SaveChanges();

            service.Delete(role.Id);

            Assert.NotEmpty(context.Set<Account>().Where(model => model.Id == account.Id && model.RoleId == null));
        }

        [Fact]
        public void Delete_Role()
        {
            service.Delete(role.Id);

            Assert.Empty(context.Set<Role>());
        }

        [Fact]
        public void Delete_RolePermissions()
        {
            service.Delete(role.Id);

            Assert.Empty(context.Set<RolePermission>());
        }

        #endregion

        #region Test helpers

        private void SetUpData()
        {
            role = ObjectFactory.CreateRole();
            foreach (String controller in new[] { "Roles", "Profile" })
                foreach (String action in new[] { "Edit", "Delete" })
                {
                    RolePermission rolePermission = ObjectFactory.CreateRolePermission(role.Permissions.Count + 1);
                    rolePermission.Permission.Area = controller == "Roles" ? "Administration" : null;
                    rolePermission.Permission.Controller = controller;
                    rolePermission.Permission.Action = action;
                    rolePermission.RoleId = role.Id;
                    rolePermission.Role = null;

                    role.Permissions.Add(rolePermission);
                }

            using (TestingContext testingContext = new TestingContext())
            {
                testingContext.Set<Role>().Add(role);
                testingContext.SaveChanges();
            }
        }

        private MvcTree CreatePermissions()
        {
            MvcTreeNode root = new MvcTreeNode(Titles.All);
            MvcTree expectedTree = new MvcTree();

            expectedTree.Nodes.Add(root);
            expectedTree.SelectedIds = new HashSet<Int32>(role.Permissions.Select(rolePermission => rolePermission.PermissionId));

            IEnumerable<Permission> permissions = role
                .Permissions
                .Select(rolePermission => rolePermission.Permission)
                .Select(permission => new Permission
                {
                    Id = permission.Id,
                    Area = ResourceProvider.GetPermissionAreaTitle(permission.Area),
                    Controller = ResourceProvider.GetPermissionControllerTitle(permission.Area, permission.Controller),
                    Action = ResourceProvider.GetPermissionActionTitle(permission.Area, permission.Controller, permission.Action)
                });

            foreach (IGrouping<String, Permission> area in permissions.GroupBy(permission => permission.Area).OrderBy(permission => permission.Key ?? permission.FirstOrDefault().Controller))
            {
                MvcTreeNode areaNode = new MvcTreeNode(area.Key);
                foreach (IGrouping<String, Permission> controller in area.GroupBy(permission => permission.Controller).OrderBy(permission => permission.Key))
                {
                    MvcTreeNode controllerNode = new MvcTreeNode(controller.Key);
                    foreach (Permission permission in controller.OrderBy(permission => permission.Action))
                        controllerNode.Children.Add(new MvcTreeNode(permission.Id, permission.Action));

                    if (areaNode.Title == null)
                        root.Children.Add(controllerNode);
                    else
                        areaNode.Children.Add(controllerNode);
                }

                if (areaNode.Title != null)
                    root.Children.Add(areaNode);
            }

            return expectedTree;
        }

        private IEnumerable<MvcTreeNode> GetLeafNodes(IEnumerable<MvcTreeNode> nodes)
        {
            List<MvcTreeNode> leafs = nodes.Where(node => node.Children.Count == 0).ToList();
            foreach (MvcTreeNode branch in nodes.Where(node => node.Children.Count > 0))
                leafs.AddRange(GetLeafNodes(branch.Children));

            return leafs;
        }
        private IEnumerable<MvcTreeNode> GetBranchNodes(IEnumerable<MvcTreeNode> nodes)
        {
            List<MvcTreeNode> branches = nodes.Where(node => node.Children.Count > 0).ToList();
            foreach (MvcTreeNode branch in branches.ToArray())
                branches.AddRange(GetBranchNodes(branch.Children));

            return branches;
        }

        #endregion
    }
}
