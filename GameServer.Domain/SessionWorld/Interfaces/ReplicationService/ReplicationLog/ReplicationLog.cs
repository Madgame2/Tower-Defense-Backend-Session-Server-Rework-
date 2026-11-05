using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog
{
    public sealed class ReplicationLog
    {
        private ReplicationLogEntry[] _entries;

        private int _head;
        private int _tail;

        public int Count => _tail - _head;

        public ReadOnlyMemory<ReplicationLogEntry> Entries =>
                new ReadOnlyMemory<ReplicationLogEntry>(
                    _entries,
                    _head,
                    Count
                );
        public ReplicationLog(int initialCapacity = 4096)
        {
            _entries = new ReplicationLogEntry[initialCapacity];
        }

        public void AddEntry(uint tick, ServerEvent replicationEvent)
        {
            if (Count > 0 && tick < _entries[_tail - 1].Tick)
                throw new InvalidOperationException(
                    "Events must be added in tick order.");

            EnsureCapacity();

            _entries[_tail++] = new ReplicationLogEntry(tick, replicationEvent);
        }

        private void EnsureCapacity()
        {
            if (_tail < _entries.Length)
                return;

            int count = Count;


            if (_head > 0)
            {
                Array.Copy(_entries, _head, _entries, 0, count);

                _head = 0;
                _tail = count;

                return;
            }

            // Если места нет вообще — увеличиваем массив.
            int newCapacity = _entries.Length * 2;

            Array.Resize(ref _entries, newCapacity);
        }

        public void RemoveThrough(uint tick)
        {
            int left = _head;
            int right = _tail;

            while (left < right)
            {
                int mid = left + (right - left) / 2;

                if (_entries[mid].Tick <= tick)
                    left = mid + 1;
                else
                    right = mid;
            }

            _head = left;

            if (_head == _tail)
            {
                _head = 0;
                _tail = 0;
            }
        }
    }
}
