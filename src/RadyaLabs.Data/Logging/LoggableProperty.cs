using Newtonsoft.Json;
using System;
using System.Data.Entity.Infrastructure;

namespace RadyaLabs.Data.Logging
{
    public class LoggableProperty
    {
        private Object OldValue { get; }
        private Object NewValue { get; }
        private String Property { get; }
        public Boolean IsModified { get; }

        public LoggableProperty(DbPropertyEntry entry, Object newValue)
        {
            NewValue = newValue;
            Property = entry.Name;
            OldValue = entry.CurrentValue;
            IsModified = entry.IsModified && !Equals(NewValue, OldValue);
        }

        public override String ToString()
        {
            if (IsModified)
                return Property + ": " + Format(NewValue) + " => " + Format(OldValue);

            return Property + ": " + Format(NewValue);
        }

        private String Format(Object value)
        {
            if (value == null)
                return "null";

            if (value is DateTime)
                return "\"" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "\"";

            return JsonConvert.ToString(value);
        }
    }
}
