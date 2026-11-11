using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload.Player
{
    public struct PlayerGameplayEventPayload
    {
        public const int ByteSize = 3;

        public PlayerGameplayEventType EventType;
        public ushort ActionId;

        public int WriteTo(Span<byte> destination)
        {
            if (destination.Length < ByteSize)
                throw new ArgumentException("Destination buffer is too small.");

            destination[0] = (byte)EventType;

            BinaryPrimitives.WriteUInt16LittleEndian(
                destination.Slice(1, 2), ActionId);

            return ByteSize;
        }
    }
}
