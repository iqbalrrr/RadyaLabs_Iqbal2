using RadyaLabs.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace RadyaLabs.Data.Logging
{
    public interface IAuditLogger : IDisposable
    {
        void Log(IEnumerable<DbEntityEntry<BaseModel>> entries);
        void Save();
    }
}
