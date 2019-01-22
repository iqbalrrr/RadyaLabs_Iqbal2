using System;

namespace RadyaLabs.Components.Logging
{
    public interface ILogger
    {
        void Log(String message);
        void Log(Exception exception);
    }
}
