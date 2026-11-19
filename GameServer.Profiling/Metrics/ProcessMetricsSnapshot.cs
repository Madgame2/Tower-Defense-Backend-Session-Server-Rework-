using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Metrics
{
    public sealed record ProcessMetricsSnapshot(
        int ProcessId,
        TimeSpan Uptime,
        double CpuUsage,
        long WorkingSet,
        long PrivateMemory,
        long ManagedMemory,
        int ThreadCount,
        int Gen0Collections,
        int Gen1Collections,
        int Gen2Collections,
        double AllocatedBytesPerSecond,
        TimeSpan GcPauseTime,
        double GcPausePercentage);
}
