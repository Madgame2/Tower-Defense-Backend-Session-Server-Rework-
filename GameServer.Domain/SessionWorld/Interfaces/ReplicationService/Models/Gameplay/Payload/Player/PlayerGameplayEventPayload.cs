using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload.Player
{
    public struct PlayerGameplayEventPayload
    {
        public const int ByteSize = 7;

        public uint PlayerId;
        public PlayerGameplayEventType EventType;
        public ushort ActionId;

        public int WriteTo(Span<byte> destination)
        {
            if (destination.Length < ByteSize)
                throw new ArgumentException("Destination buffer is too small.");

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(0, 4), PlayerId);

            destination[4] = (byte)EventType;

            BinaryPrimitives.WriteUInt16LittleEndian(
                destination.Slice(5, 2), ActionId);

            return ByteSize;
        }
    }
}
