using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Despawn
{
    public struct DespawnEvent
    {
        public const int ByteSize = 4;

        public uint objectId;

        public int WriteTo(Span<byte> destination)
        {
            if (destination.Length < ByteSize)
                throw new ArgumentException("Destination buffer is too small.");

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(0, 4), objectId);

            return ByteSize;
        }
    }
}
