using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Metrics
{
    public readonly record struct TickMetricsSnapshot(
        long TickCount,
        double TicksPerSecond,
        double AverageDurationMs,
        double MaxDurationMs,
        double AllocatedBytesPerTick,
        long MaxAllocatedBytesPerTick,
        long AllocatedBytesPerSecond);
}
