using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Despawn;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload;
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

        [FieldOffset(1)]
        public GameplayEvent gameplayEvent;

        public int GetSize()
        {
            return 1 + eventType switch
            {
                EventType.SpawnEvent => spawnEvent.GetSize(),
                EventType.DespawnEvent => DespawnEvent.ByteSize,
                EventType.GamePlayEvent => gameplayEvent.GetSize(),

                _ => throw new NotSupportedException(
                    $"Unsupported event type: {eventType}")
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

                EventType.GamePlayEvent =>
                    gameplayEvent.WriteTo(destination.Slice(1)),

                _ => throw new NotSupportedException(
                    $"Unsupported event type: {eventType}")
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

        public static ServerEvent GamePlay(uint objectId, ObjectType objectType, GameplayeEventPayload payload)
        {
            return new ServerEvent
            {
                eventType = EventType.GamePlayEvent,
                gameplayEvent = new GameplayEvent
                {
                    objectId = objectId,
                    objectType = objectType,
                    payload = payload
                }
            };
        }
    }
}
