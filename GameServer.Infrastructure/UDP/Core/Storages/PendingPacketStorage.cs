using GameServer.Domain.UDP.Meta;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace GameServer.Infrastructure.UDP.Core.Storages
{

    internal sealed class PendingPacketStorage
    {
        internal readonly struct PacketLease
        {
            public readonly uint SequenceId;
            public readonly byte[] Buffer;
            public readonly int Length;
            public readonly EndPoint EndPoint;

            public PacketLease(
                uint sequenceId,
                byte[] buffer,
                int length,
                EndPoint endPoint)
            {
                SequenceId = sequenceId;
                Buffer = buffer;
                Length = length;
                EndPoint = endPoint;
            }
        }

        private struct Entry
        {
            public uint SequenceId;
            public byte[] Buffer;
            public int Length;
            public EndPoint EndPoint;

            public Action<PacketDeliveryResult> Callback;

            public long LastSentTimestamp;
            public int Attempts;
            public int InFlight;

            public bool Active;
            public bool Completed;

            public CancellationToken CancellationToken;
        }

        private readonly Entry[] _entries;
        private readonly object _sync = new();

        private bool _disposed;

        private const int MaxAttempts = 10;
        private const int RetryIntervalMs = 250;

        private static long RetryInterval =>
            Stopwatch.Frequency * RetryIntervalMs / 1000;

        public PendingPacketStorage(int capacity = 4096)
        {
            _entries = new Entry[capacity];
        }

        public bool TryAdd(
            uint sequenceId,
            byte[] buffer,
            int length,
            EndPoint endPoint,
            Action<PacketDeliveryResult> callback,
            CancellationToken cancellationToken,
            out PacketLease lease)
        {
            lock (_sync)
            {
                lease = default;

                if (_disposed || cancellationToken.IsCancellationRequested)
                    return false;

                for (int i = 0; i < _entries.Length; i++)
                {
                    ref Entry entry = ref _entries[i];

                    if (entry.Active)
                        continue;

                    entry = new Entry
                    {
                        SequenceId = sequenceId,
                        Buffer = buffer,
                        Length = length,
                        EndPoint = endPoint,
                        Callback = callback,
                        LastSentTimestamp = Stopwatch.GetTimestamp(),
                        Attempts = 1,
                        InFlight = 1,
                        Active = true,
                        CancellationToken = cancellationToken
                    };

                    lease = new PacketLease(
                        sequenceId,
                        buffer,
                        length,
                        endPoint);

                    return true;
                }

                return false;
            }
        }

        public bool Acknowledge(
            uint sequenceId,
            EndPoint endPoint,
            out Action<PacketDeliveryResult> callback,
            out PacketDeliveryResult result)
        {
            callback = null;
            result = default;

            lock (_sync)
            {
                for (int i = 0; i < _entries.Length; i++)
                {
                    ref Entry entry = ref _entries[i];

                    if (!entry.Active ||
                        entry.Completed ||
                        entry.SequenceId != sequenceId ||
                        !entry.EndPoint.Equals(endPoint))
                        continue;

                    return Complete(
                        ref entry,
                        PacketDeliveryStatus.Acknowledged,
                        null,
                        out callback,
                        out result);
                }
            }

            return false;
        }

        public bool TryAcquireDue(
            out PacketLease lease,
            out Action<PacketDeliveryResult> callback,
            out PacketDeliveryResult result)
        {
            lease = default;
            callback = null;
            result = default;

            lock (_sync)
            {
                if (_disposed)
                    return false;

                long now = Stopwatch.GetTimestamp();

                for (int i = 0; i < _entries.Length; i++)
                {
                    ref Entry entry = ref _entries[i];

                    if (!entry.Active || entry.Completed)
                        continue;

                    if (entry.CancellationToken.IsCancellationRequested)
                    {
                        Complete(
                            ref entry,
                            PacketDeliveryStatus.Cancelled,
                            null,
                            out callback,
                            out result);

                        return true;
                    }

                    if (entry.InFlight != 0)
                        continue;

                    if (now - entry.LastSentTimestamp < RetryInterval)
                        continue;

                    if (entry.Attempts >= MaxAttempts)
                    {
                        Complete(
                            ref entry,
                            PacketDeliveryStatus.TimedOut,
                            null,
                            out callback,
                            out result);

                        return true;
                    }

                    entry.Attempts++;
                    entry.InFlight++;
                    entry.LastSentTimestamp = now;

                    lease = new PacketLease(
                        entry.SequenceId,
                        entry.Buffer,
                        entry.Length,
                        entry.EndPoint);

                    return true;
                }
            }

            return false;
        }

        public void CompleteSend(uint sequenceId)
        {
            lock (_sync)
            {
                for (int i = 0; i < _entries.Length; i++)
                {
                    ref Entry entry = ref _entries[i];

                    if (!entry.Active ||
                        entry.SequenceId != sequenceId)
                        continue;

                    if (entry.InFlight > 0)
                        entry.InFlight--;

                    TryRelease(ref entry);
                    return;
                }
            }
        }

        public void FailSend(
            uint sequenceId,
            Exception exception,
            out Action<PacketDeliveryResult> callback)
        {
            callback = null;

            lock (_sync)
            {
                for (int i = 0; i < _entries.Length; i++)
                {
                    ref Entry entry = ref _entries[i];

                    if (!entry.Active ||
                        entry.SequenceId != sequenceId ||
                        entry.Completed)
                        continue;


                    Complete(
                            ref entry,
                            PacketDeliveryStatus.SendFailed,
                            exception,
                            out callback,
                            out _);

                    return;
                }
            }
        }

        public bool Fail(
            uint sequenceId,
            PacketDeliveryStatus status,
            Exception exception,
            out Action<PacketDeliveryResult> callback,
            out PacketDeliveryResult result)
        {
            callback = null;
            result = default;

            lock (_sync)
            {
                for (int i = 0; i < _entries.Length; i++)
                {
                    ref Entry entry = ref _entries[i];

                    if (!entry.Active ||
                        entry.SequenceId != sequenceId ||
                        entry.Completed)
                        continue;

                    return Complete(
                        ref entry,
                        status,
                        exception,
                        out callback,
                        out result);
                }
            }

            return false;
        }

        private static bool Complete(
            ref Entry entry,
            PacketDeliveryStatus status,
            Exception exception,
            out Action<PacketDeliveryResult> callback,
            out PacketDeliveryResult result)
        {
            callback = null;
            result = default;

            if (!entry.Active || entry.Completed)
                return false;

            entry.Completed = true;

            uint sequenceId = entry.SequenceId;

            callback = entry.Callback;
            entry.Callback = null;

            result = new PacketDeliveryResult(
                sequenceId,
                status,
                exception);

            TryRelease(ref entry);

            return true;
        }

        private static Action<PacketDeliveryResult> WrapCallback(
            Action<PacketDeliveryResult> callback,
            uint sequenceId,
            PacketDeliveryStatus status,
            Exception exception)
        {
            if (callback == null)
                return null;

            return result => callback(result);
        }

        private static void TryRelease(ref Entry entry)
        {
            if (!entry.Active ||
                !entry.Completed ||
                entry.InFlight != 0)
                return;

            if (entry.Buffer != null)
                ArrayPool<byte>.Shared.Return(entry.Buffer);

            entry = default;
        }

        public void Dispose(
            Action<Action<PacketDeliveryResult>, PacketDeliveryResult> dispatch)
        {
            var callbacks = new List<(
                Action<PacketDeliveryResult> Callback,
                PacketDeliveryResult Result)>();

            lock (_sync)
            {
                if (_disposed)
                    return;

                _disposed = true;

                for (int i = 0; i < _entries.Length; i++)
                {
                    ref Entry entry = ref _entries[i];

                    if (!entry.Active || entry.Completed)
                        continue;

                    entry.Completed = true;

                    if (entry.Callback != null)
                    {
                        callbacks.Add((
                            entry.Callback,
                            new PacketDeliveryResult(
                                entry.SequenceId,
                                PacketDeliveryStatus.TransportDisposed)));

                        entry.Callback = null;
                    }

                    TryRelease(ref entry);
                }
            }

            foreach (var item in callbacks)
            {
                dispatch(item.Callback, item.Result);
            }
        }
    }

}
