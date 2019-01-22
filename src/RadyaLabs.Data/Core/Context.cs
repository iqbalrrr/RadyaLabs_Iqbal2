using RadyaLabs.Data.Mapping;
using RadyaLabs.Objects;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RadyaLabs.Data.Core
{
    public class Context : DbContext
    {
        #region Administration

        protected DbSet<Role> Roles { get; set; }
        protected DbSet<Account> Accounts { get; set; }
        protected DbSet<Permission> Permissions { get; set; }
        protected DbSet<RolePermission> RolePermissions { get; set; }

        protected DbSet<HouseType> HouseTypes { get; set; }
        protected DbSet<House> Houses { get; set; }
        protected DbSet<HouseCollectionHeader> HouseCollectionHeaders { get; set; }
        protected DbSet<HouseCollectionDetail> HouseCollectionDetails { get; set; }
        
        #endregion

        #region System

        protected DbSet<AuditLog> AuditLogs { get; set; }

        #endregion

        static Context()
        {
            ObjectMapper.MapObjects();
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Conventions.Remove<PluralizingTableNameConvention>();
            builder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            builder.Properties<DateTime>().Configure(config => config.HasColumnType("datetime2"));
            builder.Entity<Permission>().Property(model => model.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
