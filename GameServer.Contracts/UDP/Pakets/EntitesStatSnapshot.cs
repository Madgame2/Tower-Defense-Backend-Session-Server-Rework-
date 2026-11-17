using GameServer.Contracts.UDP.Enums;
using GameServer.Contracts.WorldStates.EntityState;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.UDP.Pakets
{

    public static class EntitesStatSnapshotWriter
    {
        private const int HeaderSize = 7;

        public static int GetSize(ReadOnlySpan<EntityState> entries)
        {
            int size = HeaderSize;

            foreach (ref readonly var entry in entries)
                size = checked(size + entry.GetSize());

            return size;
        }

        public static int WriteSnapshot(
            ReadOnlySpan<EntityState> entries,
            uint tick,
            Span<byte> destination)
        {
            if (entries.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(entries));

            int requiredSize = GetSize(entries);

            if (destination.Length < requiredSize)
                throw new ArgumentException("Destination buffer is too small.");

            int offset = 0;

            destination[offset++] = (byte)PacketType.EntityStateSnapshot;

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(offset, 4), tick);

            offset += 4;

            BinaryPrimitives.WriteUInt16LittleEndian(
                destination.Slice(offset, 2), (ushort)entries.Length);

            offset += 2;

            foreach (ref readonly var entry in entries)
                offset += entry.WriteTo(destination.Slice(offset));

            return offset;
        }
    }


    public struct EntitesStatSnapshot
    {
        public PacketType Type => PacketType.EntityStateSnapshot;
        public uint ServerTick;
        public EntityState[] entityStates;
    }
}
