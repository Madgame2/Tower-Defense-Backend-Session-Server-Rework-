using GameServer.Profiling.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Storage
{
    public sealed class LogBuffer
    {
        private readonly ConcurrentQueue<DiagnosticsLogEntry> _entries = new();

        private readonly int _capacity;

        private int _count;

        public LogBuffer(int capacity = 1000)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _capacity = capacity;
        }

        public void Enqueue(DiagnosticsLogEntry entry)
        {
            _entries.Enqueue(entry);

            var count = Interlocked.Increment(ref _count);

            while (count > _capacity)
            {
                if (!_entries.TryDequeue(out _))
                    break;

                count = Interlocked.Decrement(ref _count);
            }
        }

        public DiagnosticsLogEntry[] GetSnapshot()
        {
            return _entries.ToArray();
        }
    }
}
