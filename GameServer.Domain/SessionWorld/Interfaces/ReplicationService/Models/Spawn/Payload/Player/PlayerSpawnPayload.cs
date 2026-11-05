using GameServer.Domain.ValueObjects.Geometrics;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload.PlayerSpawnEventModel
{
    public struct PlayerSpawnPayload
    {
        public const int ByteSize = 4 + TransformSnapShoot.ByteSize;

        public uint playerID;
        public TransformSnapShoot Transforms;

        public int WriteTo(Span<byte> destination)
        {
            if (destination.Length < ByteSize)
                throw new ArgumentException("Destination buffer is too small.");

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(0, 4), playerID);

            return 4 + Transforms.WriteTo(destination.Slice(4));
        }
    }
}
