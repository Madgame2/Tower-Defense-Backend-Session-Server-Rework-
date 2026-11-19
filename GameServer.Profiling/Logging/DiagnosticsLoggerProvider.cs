using GameServer.Profiling.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Logging
{
    public sealed class DiagnosticsLoggerProvider : ILoggerProvider
    {
        public LogBuffer Buffer { get; }

        public DiagnosticsLoggerProvider(LogBuffer buffer)
        {
            Buffer = buffer;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DiagnosticsLogger(categoryName, Buffer);
        }

        public void Dispose()
        {
        }
    }
}
