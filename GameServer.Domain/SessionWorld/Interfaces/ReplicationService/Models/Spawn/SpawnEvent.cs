using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload.PlayerSpawnEventModel;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn
{
    public struct SpawnEvent
    {
        public const int HeaderSize = 5;

        public uint objectId;
        public ObjectType objectType;
        public SpawnPayload payload;

        public int GetSize()
        {
            return HeaderSize + PlayerSpawnPayload.ByteSize;
        }

        public int WriteTo(Span<byte> destination)
        {
            int size = GetSize();

            if (destination.Length < size)
                throw new ArgumentException("Destination buffer is too small.");

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(0, 4), objectId);

            destination[4] = (byte)objectType;

            return HeaderSize +
                   payload.WriteTo(destination.Slice(HeaderSize), objectType);
        }
    }
}
