namespace RadyaLabs.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Account",
            //    c => new
            //    {
            //        Id = c.Int(nullable: false, identity: true),
            //        Username = c.String(nullable: false, maxLength: 32),
            //        Passhash = c.String(nullable: false, maxLength: 64),
            //        Email = c.String(nullable: false, maxLength: 256),
            //        IsLocked = c.Boolean(nullable: false),
            //        RecoveryToken = c.String(maxLength: 36),
            //        RecoveryTokenExpirationDate = c.DateTime(precision: 7, storeType: "datetime2"),
            //        RoleId = c.Int(),
            //        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
            //    })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Role", t => t.RoleId)
            //    .Index(t => t.Username, unique: true)
            //    .Index(t => t.Email, unique: true)
            //    .Index(t => t.RoleId);

            //CreateTable(
            //    "dbo.Role",
            //    c => new
            //    {
            //        Id = c.Int(nullable: false, identity: true),
            //        Title = c.String(nullable: false, maxLength: 128),
            //        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
            //    })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.Title, unique: true);

            //CreateTable(
            //    "dbo.RolePermission",
            //    c => new
            //    {
            //        Id = c.Int(nullable: false, identity: true),
            //        RoleId = c.Int(nullable: false),
            //        PermissionId = c.Int(nullable: false),
            //        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
            //    })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Permission", t => t.PermissionId)
            //    .ForeignKey("dbo.Role", t => t.RoleId)
            //    .Index(t => t.RoleId)
            //    .Index(t => t.PermissionId);

            //CreateTable(
            //    "dbo.Permission",
            //    c => new
            //    {
            //        Id = c.Int(nullable: false),
            //        Area = c.String(maxLength: 64),
            //        Controller = c.String(nullable: false, maxLength: 64),
            //        Action = c.String(nullable: false, maxLength: 64),
            //        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
            //    })
            //    .PrimaryKey(t => t.Id);

            //CreateTable(
            //    "dbo.AuditLog",
            //    c => new
            //    {
            //        Id = c.Int(nullable: false, identity: true),
            //        AccountId = c.Int(),
            //        Action = c.String(nullable: false, maxLength: 16),
            //        EntityName = c.String(nullable: false, maxLength: 64),
            //        EntityId = c.Int(nullable: false),
            //        Changes = c.String(nullable: false),
            //        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
            //    })
            //    .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            //DropForeignKey("dbo.Account", "RoleId", "dbo.Role");
            //DropForeignKey("dbo.RolePermission", "RoleId", "dbo.Role");
            //DropForeignKey("dbo.RolePermission", "PermissionId", "dbo.Permission");
            //DropIndex("dbo.RolePermission", new[] { "PermissionId" });
            //DropIndex("dbo.RolePermission", new[] { "RoleId" });
            //DropIndex("dbo.Role", new[] { "Title" });
            //DropIndex("dbo.Account", new[] { "RoleId" });
            //DropIndex("dbo.Account", new[] { "Email" });
            //DropIndex("dbo.Account", new[] { "Username" });
            //DropTable("dbo.AuditLog");
            //DropTable("dbo.Permission");
            //DropTable("dbo.RolePermission");
            //DropTable("dbo.Role");
            //DropTable("dbo.Account");
        }
    }
}
