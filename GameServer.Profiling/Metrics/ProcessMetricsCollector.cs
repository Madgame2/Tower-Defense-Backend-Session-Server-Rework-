using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameServer.Profiling.Metrics
{
    public sealed class ProcessMetricsCollector : IDisposable
    {
        private readonly Process _process;

        private readonly TimeSpan _sampleInterval =
            TimeSpan.FromSeconds(1);

        private long _lastTimestamp;
        private TimeSpan _lastCpuTime;
        private long _lastAllocatedBytes;
        private TimeSpan _lastGcPause;

        private ProcessMetricsSnapshot? _snapshot;

        public ProcessMetricsCollector()
        {
            _process = Process.GetCurrentProcess();

            _lastTimestamp = Stopwatch.GetTimestamp();
            _lastCpuTime = _process.TotalProcessorTime;
            _lastAllocatedBytes = GC.GetTotalAllocatedBytes();
            _lastGcPause = GC.GetTotalPauseDuration();
        }

        public ProcessMetricsSnapshot Collect()
        {
            var now = Stopwatch.GetTimestamp();

            var elapsedSeconds =
                (double)(now - _lastTimestamp) / Stopwatch.Frequency;

            if (_snapshot != null && elapsedSeconds < _sampleInterval.TotalSeconds)
                return _snapshot;

            _process.Refresh();

            var cpuTime = _process.TotalProcessorTime;
            var allocatedBytes = GC.GetTotalAllocatedBytes();
            var gcPause = GC.GetTotalPauseDuration();

            var cpuElapsed = (cpuTime - _lastCpuTime).TotalSeconds;

            var cpuUsage = elapsedSeconds > 0
                ? cpuElapsed / (elapsedSeconds * Environment.ProcessorCount) * 100
                : 0;

            var allocatedDelta = Math.Max(
                0,
                allocatedBytes - _lastAllocatedBytes);

            var gcPauseDelta = gcPause - _lastGcPause;

            var pausePercentage = elapsedSeconds > 0
                ? gcPauseDelta.TotalSeconds / elapsedSeconds * 100
                : 0;

            _lastTimestamp = now;
            _lastCpuTime = cpuTime;
            _lastAllocatedBytes = allocatedBytes;
            _lastGcPause = gcPause;

            _snapshot = new ProcessMetricsSnapshot(
                ProcessId: _process.Id,
                Uptime: DateTime.Now - _process.StartTime,
                CpuUsage: Math.Max(0, cpuUsage),
                WorkingSet: _process.WorkingSet64,
                PrivateMemory: _process.PrivateMemorySize64,
                ManagedMemory: GC.GetTotalMemory(false),
                ThreadCount: _process.Threads.Count,
                Gen0Collections: GC.CollectionCount(0),
                Gen1Collections: GC.CollectionCount(1),
                Gen2Collections: GC.CollectionCount(2),
                AllocatedBytesPerSecond: allocatedDelta / elapsedSeconds,
                GcPauseTime: gcPause,
                GcPausePercentage: Math.Clamp(pausePercentage, 0, 100));

            return _snapshot;
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}
