using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Despawn;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ServerEvent
    {
        [FieldOffset(0)]
        public EventType eventType;

        [FieldOffset(1)]
        public SpawnEvent spawnEvent;

        [FieldOffset(1)]
        public DespawnEvent despawnEvent;

        public int GetSize()
        {
            return eventType switch
            {
                EventType.SpawnEvent => 1 + spawnEvent.GetSize(),
                EventType.DespawnEvent => 1 + DespawnEvent.ByteSize,
                _ => throw new NotSupportedException()
            };
        }

        public int WriteTo(Span<byte> destination)
        {
            int size = GetSize();

            if (destination.Length < size)
                throw new ArgumentException("Destination buffer is too small.");

            destination[0] = (byte)eventType;

            int written = eventType switch
            {
                EventType.SpawnEvent =>
                    spawnEvent.WriteTo(destination.Slice(1)),

                EventType.DespawnEvent =>
                    despawnEvent.WriteTo(destination.Slice(1)),

                _ => throw new NotSupportedException()
            };

            return 1 + written;
        }

        public static ServerEvent Created(uint objectId, ObjectType objectType, SpawnPayload payload)
        {
            return new ServerEvent
            {
                eventType = EventType.SpawnEvent,
                spawnEvent = new SpawnEvent
                {
                    objectId = objectId,
                    objectType = objectType,
                    payload = payload
                }
            };
        }

        public static ServerEvent Removed()
        {
            return new ServerEvent
            {
                eventType = EventType.DespawnEvent,
                despawnEvent = new DespawnEvent
                {

                }
            };
        }
    }
}
