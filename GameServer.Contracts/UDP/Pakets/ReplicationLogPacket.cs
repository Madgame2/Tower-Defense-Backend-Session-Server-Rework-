using GameServer.Contracts.UDP.Enums;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.UDP.Pakets
{

    public static class ReplicationLogWriter
    {
        private const int HeaderSize = 6;

        public static int GetSize(ReadOnlyMemory<ReplicationLogEntry> entries)
        {
            int size = HeaderSize;

            foreach (ref readonly var entry in entries.Span)
            {
                size = checked(size + entry.GetSize());
            }

            return size;
        }

        public static int WriteSnapshot(
            ReadOnlyMemory<ReplicationLogEntry> entriesToSend,
            uint tick,
            Span<byte> destination)
        {
            ReadOnlySpan<ReplicationLogEntry> entries = entriesToSend.Span;

            if (entries.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(entriesToSend));

            int requiredSize = GetSize(entriesToSend);

            if (destination.Length < requiredSize)
                throw new ArgumentException("Destination buffer is too small.");

            int offset = 0;

            destination[offset] = (byte)PacketType.ReplicationLog;
            offset += 1;

            // ServerTick
            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(offset, 4), tick);

            offset += 4;

            // EntryCount
            BinaryPrimitives.WriteUInt16LittleEndian(
                destination.Slice(offset, 2), (ushort)entries.Length);

            offset += 2;

            // Entries
            foreach (ref readonly var entry in entries)
            {
                offset += entry.WriteTo(destination.Slice(offset));
            }

            return offset;
        }
    }

    public struct ReplicationLogPacket
    {
        public PacketType Type => PacketType.ReplicationLog;
        public uint ServerTick;
        public ReplicationLogEntry[] Entries;
    }
}
