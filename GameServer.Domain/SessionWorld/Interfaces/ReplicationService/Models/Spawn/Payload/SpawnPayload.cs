using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload.PlayerSpawnEventModel;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SpawnPayload
    {
        public const int ByteSize = PlayerSpawnPayload.ByteSize;

        [FieldOffset(0)]
        public PlayerSpawnPayload playerPayload;
        [FieldOffset(0)]
        public ProjectilePayload projectilePayload;

        public int WriteTo(Span<byte> destination, ObjectType objectType)
        {
            return objectType switch
            {
                ObjectType.Player => playerPayload.WriteTo(destination),
                ObjectType.Projectile => projectilePayload.WriteTo(destination),
                _ => throw new NotSupportedException(
                    $"Unsupported object type: {objectType}")
            };
        }
    }
}
