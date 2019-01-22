using RadyaLabs.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace RadyaLabs.Data.Logging
{
    public class LoggableEntity
    {
        public String Name { get; }
        public String Action { get; }
        public Func<Int32> Id { get; }
        private static String IdName { get; }
        public IEnumerable<LoggableProperty> Properties { get; }

        static LoggableEntity()
        {
            IdName = typeof(BaseModel).GetProperties().Single(property => property.IsDefined(typeof(KeyAttribute), false)).Name;
        }

        public LoggableEntity(DbEntityEntry<BaseModel> entry)
        {
            DbPropertyValues values =
                entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                    ? entry.GetDatabaseValues()
                    : entry.CurrentValues;

            Type type = entry.Entity.GetType();

            Properties = values.PropertyNames.Where(name => name != IdName).Select(name => new LoggableProperty(entry.Property(name), values[name]));
            Properties = entry.State == EntityState.Modified ? Properties.Where(property => property.IsModified) : Properties;
            Name = type.Namespace == "System.Data.Entity.DynamicProxies" ? type.BaseType.Name : type.Name;
            Properties = Properties.ToArray();
            Action = entry.State.ToString();
            Id = () => entry.Entity.Id;
        }

        public override String ToString()
        {
            StringBuilder changes = new StringBuilder();
            foreach (LoggableProperty property in Properties)
                changes.Append(property + Environment.NewLine);

            return changes.ToString();
        }
    }
}
