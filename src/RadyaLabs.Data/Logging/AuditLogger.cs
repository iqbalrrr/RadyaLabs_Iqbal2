using RadyaLabs.Components.Extensions;
using RadyaLabs.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace RadyaLabs.Data.Logging
{
    public class AuditLogger : IAuditLogger
    {
        private Int32? AccountId { get; }
        private DbContext Context { get; }
        private List<LoggableEntity> Entities { get; }

        public AuditLogger(DbContext context)
        {
            Context = context;
            Entities = new List<LoggableEntity>();
            Context.Configuration.AutoDetectChangesEnabled = false;
        }
        public AuditLogger(DbContext context, Int32? accountId)
            : this(context)
        {
            AccountId = accountId;
        }

        public void Log(IEnumerable<DbEntityEntry<BaseModel>> entries)
        {
            foreach (DbEntityEntry<BaseModel> entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        LoggableEntity entity = new LoggableEntity(entry);
                        if (entity.Properties.Any())
                            Log(entity);
                        break;
                }
            }
        }
        public void Log(LoggableEntity entity)
        {
            Entities.Add(entity);
        }
        public void Save()
        {
            Int32? accountId = AccountId ?? HttpContext.Current.User.Id();
            foreach (LoggableEntity entity in Entities)
            {
                AuditLog log = new AuditLog();
                log.Changes = entity.ToString();
                log.EntityName = entity.Name;
                log.Action = entity.Action;
                log.EntityId = entity.Id();
                log.AccountId = accountId;

                Context.Set<AuditLog>().Add(log);
            }

            Context.SaveChanges();
            Entities.Clear();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
