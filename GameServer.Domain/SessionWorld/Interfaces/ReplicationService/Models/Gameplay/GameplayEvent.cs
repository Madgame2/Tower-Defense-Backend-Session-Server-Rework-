using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay
{
    public struct GameplayEvent
    {
        public uint objectId;
        public ObjectType objectType;
        public GameplayeEventPayload payload;



        public int GetSize()
        {
            return 5 + payload.GetSize(objectType);
        }

        public int WriteTo(Span<byte> destination)
        {
            int size = GetSize();

            if (destination.Length < size)
                throw new ArgumentException("Destination buffer is too small.");

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(0, 4), objectId);

            destination[4] = (byte)objectType;

            return 5 + payload.WriteTo(
                destination.Slice(5), objectType);
        }
    }
}
