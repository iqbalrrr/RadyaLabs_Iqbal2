using System;

namespace RadyaLabs.Services
{
    public interface IService : IDisposable
    {
        Int32 CurrentAccountId { get; set; }
    }
}
