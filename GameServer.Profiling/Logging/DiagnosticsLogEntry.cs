using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Logging
{
    public sealed record DiagnosticsLogEntry(
        DateTimeOffset Timestamp,
        LogLevel Level,
        string Category,
        EventId EventId,
        string Message,
        Exception? Exception);
}
