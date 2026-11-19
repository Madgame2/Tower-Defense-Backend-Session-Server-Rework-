using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameServer.Profiling.Metrics
{
    public sealed class TickMetricsCollector
    {
        private long _tickCount;
        private long _totalDurationTicks;
        private long _maxDurationTicks;

        private long _totalAllocatedBytes;
        private long _maxAllocatedBytes;
        private long _lastCollectTimestamp = Stopwatch.GetTimestamp();
        public readonly struct TickMeasurement
        {
            internal readonly long Timestamp;
            internal readonly long AllocatedBytes;

            internal TickMeasurement(long timestamp, long allocatedBytes)
            {
                Timestamp = timestamp;
                AllocatedBytes = allocatedBytes;
            }
        }

        public TickMeasurement BeginTick()
        {
            return new TickMeasurement(
                Stopwatch.GetTimestamp(),
                GC.GetAllocatedBytesForCurrentThread());
        }

        public void EndTick(in TickMeasurement measurement)
        {
            var duration = Stopwatch.GetTimestamp() - measurement.Timestamp;

            var allocatedBytes =
                GC.GetAllocatedBytesForCurrentThread() -
                measurement.AllocatedBytes;

            if (allocatedBytes < 0)
                allocatedBytes = 0;

            Interlocked.Increment(ref _tickCount);
            Interlocked.Add(ref _totalDurationTicks, duration);
            Interlocked.Add(ref _totalAllocatedBytes, allocatedBytes);

            UpdateMax(ref _maxDurationTicks, duration);
            UpdateMax(ref _maxAllocatedBytes, allocatedBytes);
        }

        public TickMetricsSnapshot Collect()
        {
            // Получаем накопленные показатели
            var tickCount = Interlocked.Exchange(ref _tickCount, 0);

            var totalDuration = Interlocked.Exchange(
                ref _totalDurationTicks, 0);

            var maxDuration = Interlocked.Exchange(
                ref _maxDurationTicks, 0);

            var totalAllocated = Interlocked.Exchange(
                ref _totalAllocatedBytes, 0);

            var maxAllocated = Interlocked.Exchange(
                ref _maxAllocatedBytes, 0);

            // Вычисляем продолжительность интервала сбора
            var now = Stopwatch.GetTimestamp();

            var elapsedTicks = now - Interlocked.Exchange(
                ref _lastCollectTimestamp,
                now);

            var elapsedSeconds = elapsedTicks / (double)Stopwatch.Frequency;

            // TPS
            var ticksPerSecond = elapsedSeconds > 0
                ? tickCount / elapsedSeconds
                : 0;

            // Аллокации за секунду
            var allocatedPerSecond = elapsedSeconds > 0
                ? (long)(totalAllocated / elapsedSeconds)
                : 0;

            // Средняя продолжительность тика
            var averageDuration = tickCount > 0
                ? totalDuration * 1000.0 / Stopwatch.Frequency / tickCount
                : 0;

            // Максимальная продолжительность тика
            var maxDurationMs =
                maxDuration * 1000.0 / Stopwatch.Frequency;

            // Средние аллокации на тик
            var allocatedPerTick = tickCount > 0
                ? (double)totalAllocated / tickCount
                : 0;

            return new TickMetricsSnapshot(
                tickCount,
                ticksPerSecond,
                averageDuration,
                maxDurationMs,
                allocatedPerTick,
                maxAllocated,
                allocatedPerSecond);
        }

        private static void UpdateMax(ref long location, long value)
        {
            long current;

            do
            {
                current = Volatile.Read(ref location);

                if (value <= current)
                    return;
            }
            while (Interlocked.CompareExchange(
                       ref location,
                       value,
                       current) != current);
        }
    }
}
